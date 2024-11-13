using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class MessagePanelHandler : MonoBehaviour
{
    [SerializeField] GameObject messageHUDPrefab;
    [SerializeField] GameObject content;

    public int fequencyInSeconds = 3;
    private bool _cancelResponse;
    private bool _isAppending;

    private Queue<string> _buffer; //FIFO
    private int _capacity = 1;

    [SerializeField] TTSOpenAIController ttsOpenAiController;

    private AudioSource audioSource;
    private const bool deleteCachedFile = true;

    // Start is called before the first frame update
    void Start()
    {
        _cancelResponse = false;
        _isAppending = false;
        _buffer = new Queue<string>(_capacity);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isAppending && _buffer.Count > 0)
        {
            StartCoroutine(AppendMessage(_buffer.Dequeue()));
        }
    }

    public void AddMessage(string message)
    {
        //Remove oldest message
        if (_buffer.Count >= _capacity)
        {
            _buffer.Dequeue();
        }

        _buffer.Enqueue(message);
    }

    public IEnumerator AppendMessage(string message)
    {
        _isAppending = true;

        //Clean and split the string
        Regex.Replace(message, "[אטלעש—_@#]", match => match.Value switch
        {
            "_" or "@" or "#" => "",
            "—" => ". ",
            "א" => "a'",
            "ט" => "e'",
            "ל" => "i'",
            "ע" => "o'",
            "ש" => "u'",
            _ => match.Value
        });
        string[] sentences = Regex.Split(message, @"(?<=[.!?–])\s+");

        //Show in the HUD the new messages
        foreach (string sentence in sentences)
        {
            if(_cancelResponse)
            {
                _cancelResponse = false;
                break;
            }

            //TTS con OpenAI
            Task task = ttsOpenAiController.TextToSpeechAsync(sentence);
            yield return new WaitUntil(() => task.IsCompleted);
            byte[] currentTTSGeneration = ttsOpenAiController.GetCurrentTTSGeneration();

            if (currentTTSGeneration != null)
            {
                //Process the audio bytes
                string filePath = Path.Combine(Application.persistentDataPath, "audio.mp3");
                File.WriteAllBytes(filePath, currentTTSGeneration);

                //Obtain the AudioClip
                using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG);
                yield return www.SendWebRequest();

                if(www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    if (audioSource != null)
                    {
                        //Append message
                        GameObject messageHUD = Instantiate(messageHUDPrefab);
                        messageHUD.GetComponent<MessageHUD>().SetMessage("ECHO", sentence);
                        if (content != null)
                        {
                            messageHUD.transform.SetParent(content.transform);
                        }
                        
                        //Play audioclip
                        audioSource.clip = audioClip;
                        audioSource.Play();

                        while (audioSource.isPlaying)
                        {
                            yield return null;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Audio file loading error: " + www.error);
                }

                if (deleteCachedFile)
                {
                    File.Delete(filePath);
                }
            }
        }

        _isAppending = false;
    }

    public void CancelResponse()
    {
        _cancelResponse = true;
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
    }
}
