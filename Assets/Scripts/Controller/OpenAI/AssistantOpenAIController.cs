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

    [SerializeField] MessagePanelHandler messagePanelHandler;

    //To handle current trigger event
    private string _currentTrigger;
    private bool _triggerReceived;

    //To handle expired trigger
    public float currentTriggerDuration = 90f;
    private float _currentTriggerTime;

    private bool _isMuted;
    private AudioSource audioSource;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAMEOVER, GameOverTrigger);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAMEOVER, GameOverTrigger);
    }

    void Start()
    {
        _openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));
        _assistantClient = _openAIClient.GetAssistantClient();

        CreateAssistantAndThread();

        _isMuted = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void CreateAssistantAndThread()
    {
        _assistant = _assistantClient.CreateAssistant("gpt-4o-mini", new AssistantCreationOptions()
        {
            Instructions = "When asked a question, attempt to answer very concisely. " + "Prefer one-sentence answers whenever feasible."
        });

        _thread = _assistantClient.CreateThread();
    }

    private void FixedUpdate()
    {
        //Handle a new trigger received or a timer expired (after duration time echo is triggered again)
        if (_triggerReceived || _currentTriggerTime >= currentTriggerDuration)
        {
            if (_currentTrigger != null)
            {
                SendTrigger(_currentTrigger); // SendTrigger by default interrupt if AI is speaking
                Debug.Log("OpenAI Triggered: " + _currentTrigger);
            }
            _triggerReceived = false;
            _currentTriggerTime = 0;
        }

        _currentTriggerTime += Time.fixedDeltaTime; //Update currentTriggerTime
    }

    public void HintTrigger()
    {
        Debug.Log("Mi pianto qui 0");
        _currentTriggerTime = currentTriggerDuration;
    }

    public void SendTrigger(string trigger)
    {
        StartCoroutine(AIPrompting(trigger));
    }

    IEnumerator AIPrompting(string trigger)
    {
        string prompt = trigger; //TODO prompt depends from the trigger
        float randomNumber = UnityEngine.Random.Range(1, 100000);
        prompt = "Tells a joke about tennis player Jannik Sinner";

        GetResponse(prompt);

        yield return null;
    }

    private async void GetResponse(string prompt)
    {
        AsyncCollectionResult<StreamingUpdate> streamingUpdates = _assistantClient.CreateRunStreamingAsync(_thread.Id, _assistant.Id, new RunCreationOptions()
        {
            AdditionalInstructions = prompt + "Start the reply with: Jannik Sinner Chicken Winner Joke:",
        });

        string result = "";
        await foreach (StreamingUpdate streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
            {
                Debug.Log($"--- Run started! ---");
            }
            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                result += contentUpdate.Text;
            }
        }

        Debug.Log("ASSISTANT OPENAI RESULT: " + result);
        messagePanelHandler.AppendMessage(result);
    }

    public void Mute()
    {
        _isMuted = !_isMuted;
        audioSource.mute = _isMuted;

        if (_isMuted)
        {
            messagePanelHandler.CancelResponse();
        }
        else
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