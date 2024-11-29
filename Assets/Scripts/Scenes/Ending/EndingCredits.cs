using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCredits : MonoBehaviour
{
    private void Awake()
    {
        Messenger.AddListener(GameEvent.CREDITS_STOPPED, OnCreditsStopped);
        Messenger.AddListener(GameEvent.CREDITS_ENDED, OnCreditsEnded);

        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoadManager.GetPlayer().SetActive(false);
        DontDestroyOnLoadManager.GetMainCamera().SetActive(false);
        DontDestroyOnLoadManager.GetHUD().SetActive(false);
        DontDestroyOnLoadManager.GetSkipMessage().SetActive(true);
        DontDestroyOnLoadManager.GetAudioManager().StopCurrentSoundtrack();
        StartCoroutine(StartSoundtrackWithDelay());
        Messenger.Broadcast(GameEvent.CREDITS_STARTED);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.CREDITS_STOPPED, OnCreditsStopped);
        Messenger.RemoveListener(GameEvent.CREDITS_ENDED, OnCreditsEnded);
    }

    private void Update()
    {

    }

    private void OnCreditsStopped()
    {
        LoadingScenesManager.LoadingScenes("MainMenu");
        DontDestroyOnLoadManager.DestroyAll();
    }

    private void OnCreditsEnded()
    {
        OnCreditsStopped();
    }

    private IEnumerator StartSoundtrackWithDelay()
    {
        yield return new WaitForSeconds(6f);
        DontDestroyOnLoadManager.GetAudioManager().PlaySoundtrackCredits();

        yield return new WaitForSeconds(DontDestroyOnLoadManager.GetAudioManager().GetCreditsClipLength() - 1);
        OnCreditsStopped();
    }

    public void CreditsEnded()
    {
        Messenger.Broadcast(GameEvent.CREDITS_ENDED);
    }
}
