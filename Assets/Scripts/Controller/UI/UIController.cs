using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private SettingsPopup settingsPopup;

    private bool isPlayingCutscene = false;
    private bool isPlayingCredits = false;


    void Awake()
    {
        Messenger.AddListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger.AddListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.AddListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
        Messenger.AddListener(GameEvent.CREDITS_STARTED, OnCreditsStarted);
        Messenger.AddListener(GameEvent.CREDITS_ENDED, OnCreditsEnded);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger.RemoveListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.RemoveListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
        Messenger.RemoveListener(GameEvent.CREDITS_STARTED, OnCreditsStarted);
        Messenger.RemoveListener(GameEvent.CREDITS_ENDED, OnCreditsEnded);
    }

    // Start is called before the first frame update
    void Start()
    {
        settingsPopup.Close();
    }

    // Update is called once per frame
    void Update()
    {
        // handle ESC or Start button
        if (Input.GetButtonDown("Escape"))
        {
            if (isPlayingCutscene)
                Messenger.Broadcast(GameEvent.CUTSCENE_STOPPED);
            else if (isPlayingCredits)
                Messenger.Broadcast(GameEvent.CREDITS_STOPPED);
            else
                settingsPopup.Operate();
        }
    }

    private void OnEnemyKilled()
    {
        Debug.Log("Enemy killed");
    }

    public void OnOpenSettings()
    {
        settingsPopup.Open();
    }

    public void OnCutsceneStarted()
    {
        isPlayingCutscene = true;
    }

    public void OnCutsceneEnded()
    {
        isPlayingCutscene = false;
    }

    public void OnCreditsStarted()
    {
        isPlayingCredits = true;
    }

    public void OnCreditsEnded()
    {
        isPlayingCredits = false;
    }
}
