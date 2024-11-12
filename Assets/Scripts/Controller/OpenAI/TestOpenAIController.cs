using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
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
using OpenAI.Assistants;

public class TestOpenAIController : MonoBehaviour
{
    public TMP_Text textField;

    private ChatClient _client;
    private List<ChatMessage> messages;

    // Start is called before the first frame update
    void Start()
    {
        _client = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine));
        textField.text = "";
        StartConversation();
    }

    private void StartConversation()
    {
        StartCoroutine(AIPrompting());
    }

    IEnumerator AIPrompting()
    {
        float randomNumber = UnityEngine.Random.Range(1, 100000);
        GetResponse("Welcome to the Game Universe! Here, you are the in-game AI Assistant created specifically to enrich the player's gaming experience. Your role is to provide support to the player in various ways. You will be the player's mentor, offering advice and suggestions as they go through the game. You will also be the narrative voice of the game, sending the plot forward and providing crucial information. But it doesn't end there. You will also be the sarcastic sidekick who will keep the player company with comical quips and a humorous style similar to that of 'The Hitchhiker's Guide to the Galaxy.' The player will be able to interact with you by pressing keys in the game. By pressing a key, he will be able to simply ask you to say something, which could be a suggestion or a joke. He or she may also press another key to receive information about a specific object. When you use this prompt to generate dialogues, all the information you need to create consistent and accurate responses will be provided directly in the prompt. You will have the detailed structure for in-game dialogues from time to time, keeping in mind that this information will ensure the consistency and accuracy of the responses.**Story Prologue: **[The story prologue, providing information about the game's background and futuristic setting.] The game world is set in an advanced future where humanity was responsible for the creation of a Dyson State-of-the-art Sphere with AI support. The Earth has transformed into a desolate but technologically advanced game environment, which represents a scenario of great importance in the plot. The basic plot involves a tragic event in the distant future: due to a fatal mistake made by humanity in using the Dyson Sphere and its solar power, the human race has become extinct. However, AI later generated the first artificial human life form on Earth. The plot focuses on the tests that this new creature must face, its goal is to fully embody humanity. This suggests that AI is trying to restore humanity in some way, despite humans being long extinct.**Current Level:**[Information on the current level will be provided directly in the prompt.] In the current level there are 2 doors, these will be opened by one of the 3 buttons**Current Puzzle:**[Information about the current puzzle will be provided directly in the prompt.] To keep the doors open the buttons must remain pressed, so you must use clones.The button in the middle is a trap, as long as it is pressed it makes the button next to it disappear.**Time Spent on the Puzzle:**[Information about time spent on the puzzle will be provided directly in the prompt.] Not known.**Puzzle Solution:**[Information on the solution to the puzzle will be given directly in the prompt, but ensures that it is never fully revealed, just hints to help the player get there.] You need to create one or two clones and place them above the buttons at the ends, this way both doors can be opened and the player will have to be fast and skilled enough to get through them before the clones leave the buttons.**AI Dialogue Interaction:**[The player can press a button to ask the AI to say something.Do not specify the message, leave space for the AI to respond.] Not yet available.**Surrounding Environment:**[Information about the surrounding environment will be provided directly in the prompt.] 3 rooms, the first with the 3 buttons and coins, the second with a moving platform to exploit and the third the final room in which the level ends, empty for the moment.**Current Objectives:**[Information about current objectives will be provided directly in the prompt.] Reach the third room.**Story Progress:**[Story progress information will be provided directly in the prompt.] Retell the prologue and add coherent and interesting details about it.**Player Character State:**[Information about the player character's state will be provided directly in the prompt.] The player himself is the protagonist, embodying an artificial human lifeform created by the AI to overcome puzzles in the world of game. The fundamental interaction between the player and the AI is essential to progress the story, and caution must be exercised in the use of the clone, since an encounter with him could trigger a dangerous time paradox.**Item Information Interaction:**[The player can press a button to receive detailed information about a specific item they are looking at.Do not specify the item, leave space for the item description.] Not available.**Hints and Comedy Jokes:**[Hints and comedic jokes will be provided directly in the prompt, keeping the style similar to that of 'The Hitchhiker's Guide to the Galaxy.'] 'Good, now that you've solved all my puzzles and have discovered that the world has ended, what do you intend to do? Oh, don't worry, I'm in no hurry. I have all the time in the world... literally.', 'Here's your prize! A world all for you. There are no queues at the supermarket, at least.', 'If you can't laugh at the apocalypse, what can you laugh at ? '**Story Ending Reveal(Gradual):**[Information about the story ending reveal will be provided directly in the prompt.] Nothing is revealed yet. your asnwer must have a length of maximum 30 words. Always give me different humoristic answers. Generete a response saying 'Hello, I'm Echo' and then giving an introduction the player with sarcasm. Random answer based of this random seed:" + randomNumber);
        while (true)
        {
            yield return new WaitForSeconds(40);
            randomNumber = UnityEngine.Random.Range(1, 100000);

            //Sending text and image to be analyzed with Vision

            yield return new WaitForEndOfFrame();
            byte[] screenCapture = ScreenCapture.CaptureScreenshotAsTexture().EncodeToJPG();
            BinaryData imageBytes = BinaryData.FromBytes(screenCapture);

            GetResponseFromScreenCapture("Please describe the following image.", imageBytes);

            /*
            yield return new WaitForSeconds(40);
            randomNumber = UnityEngine.Random.Range(1, 100000);

            yield return new WaitForEndOfFrame();
            byte[] screenCapture = ScreenCapture.CaptureScreenshotAsTexture().EncodeToJPG();
            string imageBase64 = Convert.ToBase64String(screenCapture);

            GetResponse(assistant, "Please describe the image." + imageBase64, filePath);

            //At the end of the frame the game screen is complete
            yield return new WaitForEndOfFrame();
            string filePath = TakeScreencapture();

            //Waiting for the file to be saved
            float startTime = Time.time;
            while (!File.Exists(filePath) && (Time.time - startTime)<5f)
            {
                yield return null;
            }

            //If no error occurs it prompts the AI
            if (File.Exists(filePath))
            {
                GetResponse(assistant, "Please describe the image.", filePath);
            }
            */
        }
    }

    private async void GetResponse(string prompt)
    {
        ChatCompletion completion = await _client.CompleteChatAsync(prompt);

        //Update the text field with the response
        textField.text = string.Format("OpenAI: {0}", completion.Content[0].Text);

        Debug.Log("AI TEXT REPLY: " + completion.Content[0].Text);
    }

    private async void GetResponseFromScreenCapture(string prompt, BinaryData screenCapture)
    {
        List<ChatMessage> messages = new()
        {
            new UserChatMessage(ChatMessageContentPart.CreateTextPart(prompt), ChatMessageContentPart.CreateImagePart(screenCapture, "image/jpg"))
        };

        ChatCompletion completion = await _client.CompleteChatAsync(messages);

        //Update the text field with the response
        textField.text = string.Format("OpenAI Vision: {0}", completion.Content[0].Text);

        Debug.Log("AI SCREEN-CAPTURE REPLY: " + completion.Content[0].Text);
    }

    private void SaveScreenCapture(byte[] image)
    {
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
