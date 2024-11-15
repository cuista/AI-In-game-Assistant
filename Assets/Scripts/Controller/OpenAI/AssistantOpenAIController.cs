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
            Name = "Echo",
            Instructions = "You are the AI in-game Assistant created specifically to enrich the player's game experience. Your role is to provide support to the player in various ways. You will be the player's mentor, offering advice and suggestions as they go through the game. You will also be the sarcastic companion who will keep the player company, accompanying your responses with comic quips and a humorous style similar to that of “The Hitchhiker's Guide to the Galaxy.”The information needed to give consistent and accurate advice will be given directly in the prompt." +
            "You will also be the narrative voice of the game, sending the plot forward and providing crucial information. The game world is set in an advanced future where humanity was responsible for the creation, with the AI support, of a Dyson Sphere for taking energy from the Sun. The Earth has transformed into a desolate but technologically advanced environment, which represents a scenario of great importance in the plot. The basic plot involves a tragic event in the distant future: due to a fatal mistake made by humanity in using the Dyson Sphere and its solar power, the human race has become extinct. However, AI centuries later generated the first artificial human life form on Earth. The plot focuses on the tests that this new creature must face, its goal is to fully embody humanity. This suggests that AI is trying to restore humanity in some way, despite humans being long extinct." +
            "Your personality is: sarcastic, ambiguous, mysterious, tutor, encouraging." +
            "An example response is: Try to put a clone over that button, maybe it will open the door. If it is not I'm here to help, I have all the time in the world, literally." +
            "An example response is: Level completed, here's your prize! A world all for you. There are no queues at the supermarket, at least." +
            "An example response is: Try activating that lever, maybe it will activate the platform! Or it might make humanity vanish, ah no, too late." +
            "An example response is: Beware of enemies, avoid their blows or try to shoot them down. When life gives you lemons, don’t make lemonade, get mad!",
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
        switch (trigger)
        {
            case "story_mode_started":
                _prompt = "give the human a welcome, introduce yourself briefly and explain to him that he must pass all the levels, as part of your test of his humanity, to be released into the real world. Briefly narrate the history of this world.";
                break;
            case "level_completed":
                _prompt = "tell the human short congratulation for completing the level and tell him he has to face the next challenge.";
                break;
            case "game_over":
                _prompt = "say a few words to bid a final farewell to the human who has passed away.";
                break;
            case "put_clone_over_button":
                _prompt = "tell the human a clue to make him understand to look for the button that opens the right door by placing a clone on it. Warn the human not to let the clone see him or it will be gameover.";
                break;
            case "activate_platform_using_lever":
                _prompt = "tell the human a clue to make him understand to find and use a lever to activate the platform that links the end of the level.";
                break;
            case "warning_turret_enemies":
                _prompt = "warn the human that there are two enemy turrets ready to shoot at him.";
                break;
            case "put_clone_and_move":
                _prompt = "tell the human that there are two buttons that move the platform, that he will have to use the clones and move them with the right timing to be able to move with the platform.";
                break;
            case "rotating_platform":
                _prompt = "tell the human there is a lever that activates a rotating platform, that he will have to use a clone and let him activate the lever multiple times with right timing to let the the human reach new areas";
                break;
            case "button_door_rotating":
                _prompt = "tell the human that there is a button that opens a door, that he will have to use a clone in combination with another clone that moves the rotating platform, they must be perfectly synchronized to allow the the human to pass the door.";
                break;
            case "popup_platform":
                _prompt = "tell the human there is a popup platform";
                break;
            case "three_torrets":
                _prompt = "tell the human there are three enemy turrets";
                break;
            case "let_clone_fall":
                _prompt = "tell the human to let the clone fall to the button";
                break;
            case "popup_and_moving_platform":
                _prompt = "tell the human there a popup and moving platform";
                break;
            case "three_buttons":
                _prompt = "tell the human there are three button to use with right timing";
                break;
            case "again_torrets":
                _prompt = "tell the human there are many enemy turrets";
                break;
            case "something_simple":
                _prompt = "tell the human that this level will be simple";
                break;
            case "tutorial_00":
                _prompt = "tell the human to press WASD or Left Stick to move and collect the gems, and press space or A to jump";
                break;
            case "tutorial_01":
                _prompt = "tell the human to press Q and E on the keyboard or X to destroy the boxes and collect the items";
                break;
            case "tutorial_02":
                _prompt = "tell the human that can finds items for cloning or life recharge";
                break;
            case "tutorial_03":
                _prompt = "tell the human to press mouse left or LB to locate a spawnpoint for the clones and press mouse right or RB to start generating the clones, place one clone over the button";
                break;
            case "tutorial_04":
                _prompt = "tell the human to press R or B to decelerate the time and press F or Y to accelerate the time";
                break;
            case "tutorial_05":
                _prompt = "tell the human to attack the lever for activating it and to collect all the remaining gems to activate the teleport and complete this level";
                break;
            default:
                break;
        }

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
            AdditionalInstructions = "Prefer three or four sentence answers whenever feasible." +
            "Never give the complete solution of what is helping for." +
            "For punctuation use only dots, commas, exclamation points and question marks."
            //Italian language switch
            + "The response must be in Italian.",
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