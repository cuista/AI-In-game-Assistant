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
        //Wait end of frame and sending text and image to be analyzed with Vision
        yield return new WaitForEndOfFrame();
        byte[] screencapture = ScreenCapture.CaptureScreenshotAsTexture().EncodeToJPG();
        BinaryData imageBytes = BinaryData.FromBytes(screencapture);

        //FOR DEBUG
        SaveScreencapture(screencapture, false);

        List<ChatMessage> messages = new()
        {
            //Model instructions
            new SystemChatMessage(ChatMessageContentPart.CreateTextPart(
                "You are the in-game AI Assistant of a Unity platform-puzzle game, your name is Nova and you have to help the player advance through the levels." +
                "Your role is to analyze the game screenshot and give assistance to the player to advance, offering useful tips and suggestions." +
                "Player goal: To pass each level, it is mandatory to collect all the gems scattered around and reach the platform that will light up when we have collected them." +
                "Player controls: can move and jump, melee attack (to open item boxes or eliminate enemies), place a clone that repeats its movements and attacks for about 20 seconds, after which it vanishes and regenerates again from its point of origin repeating same actions." +
                "Dangers to the player: if the player enters the red field of view of a clone it is gameover, if the player falls into the void it is gameover, if the player loses all life it is gameover(if hit by an enemy turret's bullet it loses one notch of life)." +
                "Consumables: the player can collect gems, red ammunition to generate clones and green potions to replenish life." +
                "HUD: Top left) the green bar is life, the notches below if red are available ammunition. Top right) there is the number of gems collected with the total number to be collected next to it." +
                "Interplayable elements: the player can attack boxes to obtain consumables, can hold onto a button to open a nearby door, can activate a lever that moves a flying platform or the floor. Buttons and levers can also be activated by clones; it is essential to take advantage of them for this reason.")),
            //Test: Language switch
            //new SystemChatMessage((ChatMessageContentPart.CreateTextPart("The response must be in Italian. All accented letters should be written with the letter without the accent plus the apostrophe (e.g. à becomes a')."))),
            //Model replies type
            new AssistantChatMessage(ChatMessageContentPart.CreateTextPart("Human here's an idea! Try putting a clone on that button nearby. It may opens the door on your right and you can go through the door. Collect all the gems along the way, you have 6 out of 20.")),
            new AssistantChatMessage(ChatMessageContentPart.CreateTextPart("Oh no, human! You don't seem to have refills to place clones, try opening some item crates. Once done try placing a clone on the button so that you can open and go through the door.")),
            new AssistantChatMessage(ChatMessageContentPart.CreateTextPart("Human I have a suggestion for you! Try generating a clone to put over the button below and another clone to activate the lever. Timing is crucial for the platform to move and the door to open afterwards.")),
            //User prompt
            new UserChatMessage(ChatMessageContentPart.CreateTextPart("Analyze the following image and give brief advice to the player to advance in the level."),
            ChatMessageContentPart.CreateImagePart(imageBytes, "image/jpg")),
        };
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
