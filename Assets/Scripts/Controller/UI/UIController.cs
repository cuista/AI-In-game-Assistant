using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private SettingsPopup settingsPopup;

    private bool isPlayingCutscene = false;


    void Awake()
    {
        Messenger.AddListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger.AddListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.AddListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger.RemoveListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.RemoveListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
    }

    // Start is called before the first frame update
    void Start()
    {
        settingsPopup.Close();
    }

    // Update is called once per frame
    void Update()
    {
        // handle ESC
        if (Input.GetButtonDown("Escape"))
        {
            if (isPlayingCutscene)
                Messenger.Broadcast(GameEvent.CUTSCENE_STOPPED);
            /*else if (isPlayingCredits)
                Messenger.Broadcast(GameEvent.CREDITS_STOPPED);*/
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
}
