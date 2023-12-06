using Ai.Inworld.Studio.V1Alpha;
using Google.Protobuf.WellKnownTypes;
using Inworld;
using Inworld.Sample;
using Inworld.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InworldAIController : MonoBehaviour
{
    public InworldCharacter echo;
    public TMP_InputField message;

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

    // Start is called before the first frame update
    void Start()
    {
        echo = (echo == null) ? FindObjectOfType<InworldCharacter>() : echo;
        echo.ResetCharacter(); //Reset memeory of previous session
        //echo.EndInteraction(); //Disable audio capture
        echoIsMuted = false;
        echoAudioSource = echo.GetComponent<AudioSource>();

        _triggerReceived = false;
        _currentTriggerTime = 0;
    }

    private void FixedUpdate()
    {
        //Handle a new trigger received or a timer expired
        if (_triggerReceived || _currentTriggerTime >= currentTriggerDuration)
        {
            if (_currentTrigger != null)
            {
                echo.SendTrigger(_currentTrigger); //After duration time echo is triggered again
                Debug.Log("Triggered again: " + _currentTrigger + " -> TIMER EXPIRED");
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

    public void Mute()
    {
        echoIsMuted = !echoIsMuted;
        echoAudioSource.mute = echoIsMuted;
    }

    public void SetCurrentTrigger(string triggerName)
    {
        Debug.Log("Current triggerAI: " + triggerName);
        _currentTrigger = triggerName;
    }

    public void OneShotTrigger(string triggerName)
    {
        _currentTriggerTime = 0;
        echo.SendTrigger(triggerName);
        Debug.Log("One Shot Trigger: " + triggerName);
    }

    public void GameOverTrigger()
    {
        _currentTriggerTime = 0;
        echo.SendTrigger("game_over");
        Debug.Log("Game Over Trigger");
    }
}
