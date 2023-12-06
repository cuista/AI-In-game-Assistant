using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InworldAIController : MonoBehaviour
{
    public Inworld.InworldCharacter echo;
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
        echo = (echo == null) ? FindObjectOfType<Inworld.InworldCharacter>() : echo;
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
                try
                {
                    echo.CancelResponse(); //Interrupt if Echo is speaking
                } catch (System.Exception) { }
                echo.SendTrigger(_currentTrigger); //After duration time echo is triggered again
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

    public void Mute()
    {
        echoIsMuted = !echoIsMuted;
        echoAudioSource.mute = echoIsMuted;
    }

    public void SetCurrentTrigger(string triggerName)
    {
        Debug.Log("Current triggerAI: " + triggerName);
        _currentTrigger = triggerName;
        _triggerReceived = true;
    }

    public void OneShotTrigger(string triggerName)
    {
        _currentTriggerTime = 0;
        try
        {
            echo.CancelResponse(); //Interrupt if Echo is speaking
        }
        catch (System.Exception) { }
        echo.SendTrigger(triggerName);
        Debug.Log("One Shot TriggerAI: " + triggerName);
    }

    public void GameOverTrigger()
    {
        OneShotTrigger("game_over");
    }
}
