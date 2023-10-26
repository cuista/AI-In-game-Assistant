using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPopup : MonoBehaviour
{
    private bool _isGameOver = false;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAMEOVER, OnGameOver);
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
        //LoadingScenesManager.LoadingScenes("InitialMenu");
        //DontDestroyOnLoadManager.DestroyAll();
    }

    //Pause game, stop frames with timeScale = 0
    public void PauseGame()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    //Resume Game
    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void OnSpeedValue(float speed)
    {
        Messenger<float>.Broadcast(GameEvent.SPEED_CHANGED, speed);
    }

    public void OnGameOver()
    {
        _isGameOver = true;
    }

}
