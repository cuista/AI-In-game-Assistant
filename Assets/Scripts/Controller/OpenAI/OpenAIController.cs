using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    public Reader readerTSS;

    // Start is called before the first frame update
    void Start()
    {
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY_UNITY", EnvironmentVariableTarget.Machine));
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage>() //List of messages using OpenAI_API.Chat
        {
            new ChatMessage(ChatMessageRole.System, "You are an AI assistent that reply to me for testing reasons, ask me something about this game I'm building")
        };

        inputField.text = "";
        string startString = "You're testing the OpenAI API in Unity, welcome. How can I help you?";
        textField.text = startString;
        Debug.Log(startString);

        //Read with TTS
        readerTSS.readTTS(startString);
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        //Disable the OK button
        okButton.enabled = false;

        //Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Content = inputField.text;
        if (userMessage.Content.Length > 100)
        {
            //Limit messagges to 100 characters
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        Debug.Log(string.Format("{0}: {1}", userMessage.Role, userMessage.Content));

        //Add the message to the list
        messages.Add(userMessage);

        //Update the text field with the user message
        textField.text = string.Format("You: {0}", userMessage.Content);

        //Clear the input field
        inputField.text = "";

        //Send the entire chat to OpenAI to get the next message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        { 
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 50,
            Messages = messages //list of messages (give all of them to chatbot because it doesn't have memory)
        });

        //Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.Role, responseMessage.Content));

        //Add the response to the list of messages
        messages.Add(responseMessage);

        //Update the text field with the response
        textField.text = string.Format("You: {0}\n\nAI: {1}", userMessage.Content, responseMessage.Content);

        //Read with TTS
        readerTSS.readTTS(responseMessage.Content);

        //Re-enable the OK button
        okButton.enabled = true;
    }
}
