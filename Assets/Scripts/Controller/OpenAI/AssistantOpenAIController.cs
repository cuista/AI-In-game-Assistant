using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using OpenAI.Assistants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.ClientModel;
using OpenAI.Files;

public class AssistantOpenAIController : MonoBehaviour
{
    private OpenAIClient _openAIClient;
    private AssistantClient _assistantClient;

    private Assistant _assistant;
    private AssistantThread _thread;

    private VisionOpenAIController vision;

    [SerializeField] MessagePanelHandler messagePanelHandler;

    //To handle current trigger event
    private string _currentTrigger;
    private bool _triggerReceived;

    //To handle expired trigger
    public float currentTriggerDuration = 60f;
    private float _currentTriggerTime;

    //Prompt for the assistant
    private string _prompt;

    private bool _isMuted;

    public TextAsset settingsJson;
    private AssistantSettings _assistantSettings;

    [System.Serializable]
    public class Trigger
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    private class AssistantSettings
    {
        public string assistantName;
        public string language;
        public string[] assistantInstructions;
        public string[] additionalInstructions;
        public Trigger[] triggers;

        public string GetAll(string[] instructions)
        {
            string result = "";
            for(int i=0; i < instructions.Length; i++)
            {
                result += (i<instructions.Length-1)? instructions[i] + " " : instructions[i];
            }
            return result;
        }

        public string GetPromptForTrigger(string key)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.key == key)
                    return trigger.value;
            }
            return null;
        }

        public void DebugPrintAll()
        {
            Debug.Log(assistantName);
            Debug.Log(language);
            Debug.Log(assistantInstructions);
            Debug.Log(additionalInstructions);

            foreach (var trigger in triggers)
            {
                Debug.Log(trigger.key + " -> " + trigger.value);
            }
        }
    }

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAMEOVER, GameOverTrigger);

        //Import settings
        _assistantSettings = new();
        JsonUtility.FromJsonOverwrite(settingsJson.text, _assistantSettings);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAMEOVER, GameOverTrigger);
    }

    void Start()
    {
        _openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));
        _assistantClient = _openAIClient.GetAssistantClient();

        vision = GetComponent<VisionOpenAIController>();
        _prompt = "";

        CreateAssistantAndThread();

        _isMuted = false;
    }

    private void CreateAssistantAndThread()
    {
        //Create the assistant
        _assistant = _assistantClient.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
        {
            Name = _assistantSettings.assistantName,
            Instructions = _assistantSettings.GetAll(_assistantSettings.assistantInstructions),
        });

        //Create the thread
        _thread = _assistantClient.CreateThread();
    }

    private void FixedUpdate()
    {
        //Handle a new trigger received
        if (_triggerReceived)
        {
            if (_currentTrigger != null)
            {
                SendTrigger(_currentTrigger); // SendTrigger by default interrupt if AI is speaking
            }
            _triggerReceived = false;
            _currentTriggerTime = 0;
        }
        else if(_currentTriggerTime >= currentTriggerDuration) //When timer expired vision is triggered
        {
            vision.VisionTrigger();
            _triggerReceived = false;
            _currentTriggerTime = 0;
        }

        _currentTriggerTime += Time.fixedDeltaTime; //Update currentTriggerTime
    }

    public void HintTrigger()
    {
        _currentTriggerTime = currentTriggerDuration;
    }

    public void SendTrigger(string trigger)
    {
        StartCoroutine(AIPrompting(trigger));
    }

    IEnumerator AIPrompting(string trigger)
    {
        string triggerPrompt = _assistantSettings.GetPromptForTrigger(trigger);
        _prompt = triggerPrompt != null? "Generate a message based on this guidance: " + triggerPrompt : _prompt;

        GetResponse(_prompt);

        yield return null;
    }

    private async void GetResponse(string prompt)
    {
        //Add the message to the thread
        await _assistantClient.CreateMessageAsync(_thread.Id, MessageRole.User, new List<MessageContent>() { MessageContent.FromText(prompt) });

        //Create a run
        AsyncCollectionResult<StreamingUpdate> streamingUpdates = _assistantClient.CreateRunStreamingAsync(_thread.Id, _assistant.Id, new RunCreationOptions()
        {
            //Language
            AdditionalInstructions = _assistantSettings.GetAll(_assistantSettings.additionalInstructions) + " The response must be in " + _assistantSettings.language + ".",
        });

        string result = "";
        await foreach (StreamingUpdate streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                result += contentUpdate.Text;
            }
        }

        messagePanelHandler.AddMessage(result);
    }

    public void Mute()
    {
        _isMuted = !_isMuted;
        messagePanelHandler.Mute(_isMuted);
        messagePanelHandler.CancelResponse(_isMuted);

        if (!_isMuted)
        {
            HintTrigger();
        }
    }

    public bool IsMuted() => _isMuted;

    public void SetCurrentTrigger(string triggerName)
    {
        Debug.Log("Current triggerAI: " + triggerName);
        _currentTrigger = triggerName;
        _triggerReceived = true;
    }

    public void OneShotTrigger(string triggerName)
    {
        _currentTriggerTime = 0;
        SendTrigger(triggerName);
        Debug.Log("One Shot TriggerAI: " + triggerName);
    }

    public void GameOverTrigger()
    {
        OneShotTrigger("game_over");
    }

    private string TakeScreencapture()
    {
        // the project folder path
        string folderPath = "Assets/Screencaptures/";

        // if this path does not exist yet it will get created
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        // puts the current time right into the screenshot name and the data format is .png
        var screencaptureName = "Screencapture_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".jpg";
        string filePath = System.IO.Path.Combine(folderPath, screencaptureName);

        ScreenCapture.CaptureScreenshot(filePath);

        return filePath;
    }

}