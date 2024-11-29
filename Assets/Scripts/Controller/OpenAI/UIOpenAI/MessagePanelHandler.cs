using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

[RequireComponent(typeof(AudioSource))]
public class MessagePanelHandler : MonoBehaviour
{
    [SerializeField] GameObject messageHUDPrefab;
    [SerializeField] GameObject content;
    [SerializeField] TTSOpenAIController ttsOpenAiController;

    private const int QUEUE_CAPACITY = 1;
    private Queue<string> _MessagesQueue;
    private Task ttsTask;

    private bool _cancelResponse;
    private bool _skipResponse;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _MessagesQueue = new Queue<string>(QUEUE_CAPACITY);
        ttsTask = null;

        _cancelResponse = false;
        _skipResponse = false;

        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (ttsTask == null && _MessagesQueue.Count > 0)
        {
            ttsTask = AppendMessage(_MessagesQueue.Dequeue());
        }

        if(ttsTask != null && ttsTask.GetAwaiter().IsCompleted)
        {
            _skipResponse = false;
            ttsTask = null;
        }
    }

    public void AddMessage(string message)
    {
        if (_MessagesQueue.Count >= QUEUE_CAPACITY)
        {
            _MessagesQueue.Dequeue();
        }

        if(ttsTask != null)
        {
            _skipResponse = true;
        }

        _MessagesQueue.Enqueue(message);
    }

    private async Task AppendMessage(string message)
    {
        //Clean and split the string
        Regex.Replace(message, "[אטלעש—_@#]", match => match.Value switch
        {
            "_" or "@" or "#" => "",
            "—" => ", ",
            "א" => "a'",
            "ט" => "e'",
            "ל" => "i'",
            "ע" => "o'",
            "ש" => "u'",
            _ => match.Value
        });
        string[] sentences = Regex.Split(message, @"(?<=[.!?\—])\s+");

        //Show in the HUD the new messages
        foreach (string sentence in sentences)
        {
            if (_cancelResponse || _skipResponse)
            {
                break;
            }

            await ttsOpenAiController.TextToSpeechAsync(sentence);
            audioSource.PlayOneShot(ttsOpenAiController.GetAudioClip());

            //Append message
            GameObject messageHUD = Instantiate(messageHUDPrefab);
            messageHUD.GetComponent<MessageHUD>().SetMessage("ECHO", sentence);
            if (content != null)
            {
                messageHUD.transform.SetParent(content.transform);
            }

            await WaitForAudioToBePlayed();
        }
    }

    private async Task WaitForAudioToBePlayed()
    {
        while(audioSource.isPlaying)
        {
            await Task.Delay(100);
        }
    }

    public void CancelResponse(bool cancel)
    {
        _cancelResponse = cancel;
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
    }
}
