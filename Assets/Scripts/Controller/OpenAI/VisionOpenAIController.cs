using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VisionOpenAIController : MonoBehaviour
{
    private ChatClient _client;

    [SerializeField] MessagePanelHandler messagePanelHandler;

    public TextAsset settingsJson;
    private VisionSettings _visionSettings;

    [System.Serializable]
    private class VisionSettings
    {
        public string assistantName;
        public string language;
        public string[] systemMessages;
        public string[] assistantMessages;

        public string GetAll(string[] messages)
        {
            string result = "";
            for (int i = 0; i < messages.Length; i++)
            {
                result += (i < messages.Length - 1) ? messages[i] + " " : messages[i];
            }
            return result;
        }

        public void DebugPrintAll()
        {
            Debug.Log(assistantName);
            Debug.Log(language);
            Debug.Log(GetAll(systemMessages));
            Debug.Log(GetAll(assistantMessages));
        }
    }

    private void Awake()
    {
        //Import settings
        _visionSettings = new();
        JsonUtility.FromJsonOverwrite(settingsJson.text, _visionSettings);

        _visionSettings.DebugPrintAll();
    }

    // Start is called before the first frame update
    void Start()
    {
        _client = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));
    }

    public void VisionTrigger()
    {
        StartCoroutine(AIPrompting());
    }

    IEnumerator AIPrompting()
    {
        yield return new WaitForEndOfFrame();
        Camera screenshotCamera = new GameObject("VisionCamera").AddComponent<Camera>();
        screenshotCamera.CopyFrom(Camera.main);
        screenshotCamera.cullingMask |= (1 << LayerMask.NameToLayer("VisionLabel"));
        screenshotCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UIAssistantMessage"));
        screenshotCamera.enabled = false;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenshotCamera.targetTexture = renderTexture;

        screenshotCamera.Render();

        // Cattura i dati dalla render texture
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        byte[] screencapture = screenshot.EncodeToJPG();
        BinaryData imageBytes = BinaryData.FromBytes(screencapture);

        /*
        //Wait end of frame and sending text and image to be analyzed with Vision
        yield return new WaitForEndOfFrame();
        byte[] screencapture = ScreenCapture.CaptureScreenshotAsTexture().EncodeToJPG();
        BinaryData imageBytes = BinaryData.FromBytes(screencapture);
        */

        //FOR DEBUG
        SaveScreencapture(screencapture, true);

        List<ChatMessage> messages = new();

        for(int i = 0; i < _visionSettings.systemMessages.Length; i++)
        {
            messages.Add(new SystemChatMessage(ChatMessageContentPart.CreateTextPart(_visionSettings.systemMessages[i])));
        }

        for (int i = 0; i < _visionSettings.assistantMessages.Length; i++)
        {
            messages.Add(new AssistantChatMessage(ChatMessageContentPart.CreateTextPart(_visionSettings.assistantMessages[i])));
        }

        //Screencapture evaluation
        messages.Add(new UserChatMessage(ChatMessageContentPart.CreateTextPart("Analyze the following image and give brief advice to the player to advance in the level.")));
        messages.Add(new UserChatMessage(ChatMessageContentPart.CreateImagePart(imageBytes, "image/jpg")));

        //Language
        messages.Add(new UserChatMessage(ChatMessageContentPart.CreateTextPart("The response must be in " + _visionSettings.language + ".")));

        GetResponse(messages);

        yield return null;
    }

    private async void GetResponse(List<ChatMessage> messages)
    {
        ChatCompletion completion = await _client.CompleteChatAsync(messages, new ChatCompletionOptions(){ Seed = UnityEngine.Random.Range(1, 100000), });

        //Show new response messages in the hud
        messagePanelHandler.AddMessage(completion.Content[0].Text);
    }

    private void SaveScreencapture(byte[] image, bool activeSaving)
    {
        if (!activeSaving)
            return;

        // the project folder path
        string folderPath = "Assets/Screencaptures/";

        // if this path does not exist yet it will get created
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        // puts the current time right into the screenshot name and the data format is .png
        var screenshotName = "Screencapture_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";

        File.WriteAllBytes(System.IO.Path.Combine(folderPath, screenshotName), image);
    }
}
