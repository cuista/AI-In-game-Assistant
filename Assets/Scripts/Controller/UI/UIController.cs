using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private SettingsPopup settingsPopup;

    private bool isPlayingCutscene = false;

    //[SerializeField] private GameObject target;

    //[SerializeField] private Text targetCount;

    //[SerializeField] private Text targetTotal;


    void Awake()
    {
        Messenger.AddListener(GameEvent.ENEMY_KILLED, OnEnemyHit);
        Messenger.AddListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.AddListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
        /*Messenger<int>.AddListener(GameEvent.TARGET_TOTAL, OnTargetTotal);
        Messenger.AddListener(GameEvent.TARGET_ELIMINATED, OnTargetEliminated);*/
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_KILLED, OnEnemyHit);
        Messenger.RemoveListener(GameEvent.CUTSCENE_STARTED, OnCutsceneStarted);
        Messenger.RemoveListener(GameEvent.CUTSCENE_ENDED, OnCutsceneEnded);
        /*Messenger<int>.RemoveListener(GameEvent.TARGET_TOTAL, OnTargetTotal);
        Messenger.RemoveListener(GameEvent.TARGET_ELIMINATED, OnTargetEliminated);*/
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

    private void OnEnemyHit()
    {
        Debug.Log("Enemy hit");
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

    /*
    public void OnTargetTotal(int total)
    {
        target.SetActive(true);
        targetCount.text = "0";
        targetTotal.text = total.ToString();
    }

    public void OnTargetEliminated()
    {
        targetCount.text = (int.Parse(targetCount.text) + 1).ToString();
    }
    */
}
