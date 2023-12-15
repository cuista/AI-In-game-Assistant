using Inworld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InworldAIController : MonoBehaviour
{
    public InworldCharacter echoCharacter;
    public AudioCapture echoAudio;

    //To handle current trigger event
    private string _currentTrigger;
    private bool _triggerReceived;

    //To handle expired trigger
    public float currentTriggerDuration = 90f;
    private float _currentTriggerTime;

    private bool echoIsMuted;
    private AudioSource echoAudioSource;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAMEOVER, GameOverTrigger);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAMEOVER, GameOverTrigger);
    }

    // Start is called before the first frame update
    void Start()
    {
        echoCharacter = (echoCharacter == null) ? FindObjectOfType<InworldCharacter>() : echoCharacter;
        echoIsMuted = false;
        echoAudioSource = echoCharacter.GetComponent<AudioSource>();

        echoAudio.StopRecording(); //Not recording with mic
        
        _triggerReceived = false;
        _currentTriggerTime = 0;
    }

    private void FixedUpdate()
    {
        //Handle a new trigger received or a timer expired (after duration time echo is triggered again)
        if (_triggerReceived || _currentTriggerTime >= currentTriggerDuration)
        {
            if (_currentTrigger != null)
            {
                echoCharacter.SendTrigger(_currentTrigger); // SendTrigger by default interrupt if Echo is speaking
                Debug.Log("AI Triggered: " + _currentTrigger);
            }
            _triggerReceived = false;
            _currentTriggerTime = 0;
        }

        _currentTriggerTime += Time.fixedDeltaTime; //Update currentTriggerTime
    }

    public void HintTrigger()
    {
        _currentTriggerTime = currentTriggerDuration;
    }

    public void CancelResponse()
    {
        try
        {
            echoCharacter.CancelResponse(); //Interrupt if Echo is speaking
        }
        catch (System.Exception) { }
    }

    public void Mute()
    {
        echoIsMuted = !echoIsMuted;
        echoAudioSource.mute = echoIsMuted;

        if (echoIsMuted)
        {
            CancelResponse();
        }
        else
        {
            HintTrigger();
        }
    }

    public bool IsMuted() => echoIsMuted;

    public void SetCurrentTrigger(string triggerName)
    {
        Debug.Log("Current triggerAI: " + triggerName);
        _currentTrigger = triggerName;
        _triggerReceived = true;
    }

    public void OneShotTrigger(string triggerName)
    {
        _currentTriggerTime = 0;
        echoCharacter.SendTrigger(triggerName);
        Debug.Log("One Shot TriggerAI: " + triggerName);
    }

    public void GameOverTrigger()
    {
        OneShotTrigger("game_over");
    }
}
