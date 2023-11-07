using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPopup : MonoBehaviour
{
    private bool _isGameOver = false;

    [SerializeField] private AudioClip openPopupSound;
    [SerializeField] private AudioClip clickSound;

    private AudioManager audioManager;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAMEOVER, OnGameOver);
        audioManager = DontDestroyOnLoadManager.GetAudioManager();
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAMEOVER, OnGameOver);
    }

    public void Operate()
    {
        if(!gameObject.activeInHierarchy)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    //Show up the setting window
    public void Open()
    {
        gameObject.SetActive(true);
        if (!_isGameOver)
        {
            PauseGame();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //Close the setting window
    public void Close()
    {
        gameObject.SetActive(false);
        if (!_isGameOver)
            UnPauseGame();
    }

    //Back to initial menu
    public void ExitGame()
    {
        LoadingScenesManager.LoadingScenes("MainMenu");
        DontDestroyOnLoadManager.DestroyAll();
    }

    //Pause game, stop frames with timeScale = 0
    public void PauseGame()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        GetComponentInChildren<Button>().Select();
    }

    //Resume Game
    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void OnGameOver()
    {
        _isGameOver = true;
    }
    public void OnSoundToggle()
    {
        audioManager.soundMute = !audioManager.soundMute;
        audioManager.PlaySound(clickSound);
    }

    public void OnSoundValue(float volume)
    {
        audioManager.soundVolume = volume;
    }

    public void OnMusicToggle()
    {
        audioManager.musicMute = !audioManager.musicMute;
        audioManager.PlaySound(clickSound);
    }

    public void OnMusicValue(float volume)
    {
        audioManager.musicVolume = volume;
    }

}
