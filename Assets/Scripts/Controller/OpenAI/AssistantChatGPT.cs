using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
/*
public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;
    public class OpenAIController : MonoBehaviour
    void Start()
    {
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY_UNITY", EnvironmentVariableTarget.Machine));
        textField.text = "";
        StartConversation();
    okButton.onClick.AddListener(() => GetResponse());
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage>() //List of messages using OpenAI_API.Chat
            {
                new ChatMessage(ChatMessageRole.System, "You are an AI assistent that reply to me for testing reasons, ask me something about this game I'm building")
                new ChatMessage(ChatMessageRole.System, "Welcome to the Game Universe! Here, you are the in-game AI Assistant created specifically to enrich the player's gaming experience. Your role is to provide support to the player in various ways. You will be the player's mentor, offering advice and suggestions as they go through the game. You will also be the narrative voice of the game, sending the plot forward and providing crucial information. But it doesn't end there. You will also be the sarcastic sidekick who will keep the player company with comical quips and a humorous style similar to that of 'The Hitchhiker's Guide to the Galaxy.' The player will be able to interact with you by pressing keys in the game. By pressing a key, he will be able to simply ask you to say something, which could be a suggestion or a joke. He or she may also press another key to receive information about a specific object. When you use this prompt to generate dialogues, all the information you need to create consistent and accurate responses will be provided directly in the prompt. You will have the detailed structure for in-game dialogues from time to time, keeping in mind that this information will ensure the consistency and accuracy of the responses.")
            };

        inputField.text = "";
        string startString = "You're testing the OpenAI API in Unity, welcome. How can I help you?";
        textField.text = startString;
        Debug.Log(startString);

        //Read with TTS
        readerTSS.readTTS(startString);
        StartCoroutine(AIPrompting());
    }

    private async void GetResponse()
        IEnumerator AIPrompting()
        {
            if (inputField.text.Length < 1)
                float randomNumber = UnityEngine.Random.Range(1, 100000);
            GetResponse("Welcome to the Game Universe! Here, you are the in-game AI Assistant created specifically to enrich the player's gaming experience. Your role is to provide support to the player in various ways. You will be the player's mentor, offering advice and suggestions as they go through the game. You will also be the narrative voice of the game, sending the plot forward and providing crucial information. But it doesn't end there. You will also be the sarcastic sidekick who will keep the player company with comical quips and a humorous style similar to that of 'The Hitchhiker's Guide to the Galaxy.' The player will be able to interact with you by pressing keys in the game. By pressing a key, he will be able to simply ask you to say something, which could be a suggestion or a joke. He or she may also press another key to receive information about a specific object. When you use this prompt to generate dialogues, all the information you need to create consistent and accurate responses will be provided directly in the prompt. You will have the detailed structure for in-game dialogues from time to time, keeping in mind that this information will ensure the consistency and accuracy of the responses.**Story Prologue: **[The story prologue, providing information about the game's background and futuristic setting.] The game world is set in an advanced future where humanity was responsible for the creation of a Dyson State-of-the-art Sphere with AI support. The Earth has transformed into a desolate but technologically advanced game environment, which represents a scenario of great importance in the plot. The basic plot involves a tragic event in the distant future: due to a fatal mistake made by humanity in using the Dyson Sphere and its solar power, the human race has become extinct. However, AI later generated the first artificial human life form on Earth. The plot focuses on the tests that this new creature must face, its goal is to fully embody humanity. This suggests that AI is trying to restore humanity in some way, despite humans being long extinct.**Current Level:**[Information on the current level will be provided directly in the prompt.] In the current level there are 2 doors, these will be opened by one of the 3 buttons**Current Puzzle:**[Information about the current puzzle will be provided directly in the prompt.] To keep the doors open the buttons must remain pressed, so you must use clones.The button in the middle is a trap, as long as it is pressed it makes the button next to it disappear.**Time Spent on the Puzzle:**[Information about time spent on the puzzle will be provided directly in the prompt.] Not known.**Puzzle Solution:**[Information on the solution to the puzzle will be given directly in the prompt, but ensures that it is never fully revealed, just hints to help the player get there.] You need to create one or two clones and place them above the buttons at the ends, this way both doors can be opened and the player will have to be fast and skilled enough to get through them before the clones leave the buttons.**AI Dialogue Interaction:**[The player can press a button to ask the AI to say something.Do not specify the message, leave space for the AI to respond.] Not yet available.**Surrounding Environment:**[Information about the surrounding environment will be provided directly in the prompt.] 3 rooms, the first with the 3 buttons and coins, the second with a moving platform to exploit and the third the final room in which the level ends, empty for the moment.**Current Objectives:**[Information about current objectives will be provided directly in the prompt.] Reach the third room.**Story Progress:**[Story progress information will be provided directly in the prompt.] Retell the prologue and add coherent and interesting details about it.**Player Character State:**[Information about the player character's state will be provided directly in the prompt.] The player himself is the protagonist, embodying an artificial human lifeform created by the AI to overcome puzzles in the world of game. The fundamental interaction between the player and the AI is essential to progress the story, and caution must be exercised in the use of the clone, since an encounter with him could trigger a dangerous time paradox.**Item Information Interaction:**[The player can press a button to receive detailed information about a specific item they are looking at.Do not specify the item, leave space for the item description.] Not available.**Hints and Comedy Jokes:**[Hints and comedic jokes will be provided directly in the prompt, keeping the style similar to that of 'The Hitchhiker's Guide to the Galaxy.'] 'Good, now that you've solved all my puzzles and have discovered that the world has ended, what do you intend to do? Oh, don't worry, I'm in no hurry. I have all the time in the world... literally.', 'Here's your prize! A world all for you. There are no queues at the supermarket, at least.', 'If you can't laugh at the apocalypse, what can you laugh at ? '**Story Ending Reveal(Gradual):**[Information about the story ending reveal will be provided directly in the prompt.] Nothing is revealed yet. your asnwer must have a length of maximum 30 words. Always give me different humoristic answers. Generete a response saying 'Hello, I'm Echo' and then giving an introduction the player with sarcasm. Random answer based of this random seed:" + randomNumber);
            while (true)
            {
                return;
                yield return new WaitForSeconds(40);
                randomNumber = UnityEngine.Random.Range(1, 100000);
                GetResponse("Generete a response telling more to the player or giving help to the player or saying something sarcastic. Random answer based of this random seed: " + randomNumber);
            }
        }

//Disable the OK button
okButton.enabled = false;

private async void GetResponse(string prompt)
{
    //Fill the user message from the input field
    ChatMessage userMessage = new ChatMessage();
    userMessage.Content = inputField.text;
    if (userMessage.Content.Length > 100)
    {
        //Limit messagges to 100 characters
        userMessage.Content = userMessage.Content.Substring(0, 100);
    }
    userMessage.Content = prompt;
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
        MaxTokens = 500,
        Messages = messages //list of messages (give all of them to chatbot because it doesn't have memory)
    });

    private async void GetResponse()
        messages.Add(responseMessage);

    //Update the text field with the response
    textField.text = string.Format("You: {0}\n\nAI: {1}", userMessage.Content, responseMessage.Content);
    textField.text = string.Format("Echo: {0}", responseMessage.Content);

    //Read with TTS
    readerTSS.readTTS(responseMessage.Content);

    //Re-enable the OK button
    okButton.enabled = true;
}
}
*/