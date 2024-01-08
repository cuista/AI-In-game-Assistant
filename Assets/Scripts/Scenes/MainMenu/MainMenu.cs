using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;

    [SerializeField] private AudioClip clickSound;

    [SerializeField] private GameObject initialMenu;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject videoMenu;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private GameObject firstSelectedGameObject;

    private void Awake()
    {
        audioManager = DontDestroyOnLoadManager.GetAudioManager();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DontDestroyOnLoadManager.GetAudioManager().PlaySoundtrackMenuClip();
        
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
        initialMenu.SetActive(true);
        settingsMenu.SetActive(false);
        exitMenu.SetActive(false);
        gameModeMenu.SetActive(false);
        audioMenu.SetActive(false);
        videoMenu.SetActive(false);
    }
    public void PlayGame()
    {
        audioManager.PlaySound(clickSound);
        HideMenu();
        LoadingScenesManager.LoadingScenes("Gameplay", "Level_0");
    }

    public void LoadGame()
    {
        audioManager.PlaySound(clickSound);
        HideMenu();
        LoadingScenesManager.LoadingScenes("Gameplay", "Level_5");
    }

    public void QuitGame()
    {
        audioManager.PlaySound(clickSound);
        Application.Quit();
    }

    public void HideMenu()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
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

    public void SetActiveGameModeMenu(bool active)
    {
        audioManager.PlaySound(clickSound);
        initialMenu.SetActive(!active);
        gameModeMenu.SetActive(active);
        SetButtonSelected(active, gameModeMenu, initialMenu);
    }

    public void SetActiveSettingsMenu(bool active)
    {
        audioManager.PlaySound(clickSound);
        initialMenu.SetActive(!active);
        settingsMenu.SetActive(active);
        videoMenu.SetActive(false);
        audioMenu.SetActive(false);
        SetButtonSelected(active, settingsMenu, initialMenu);
    }

    public void SetActiveExitMenu(bool active)
    {
        audioManager.PlaySound(clickSound);
        initialMenu.SetActive(!active);
        exitMenu.SetActive(active);
        SetButtonSelected(active, exitMenu, initialMenu);
    }

    public void ToggleAudioMenu()
    {
        audioManager.PlaySound(clickSound);
        SetButtonSelected(!audioMenu.activeInHierarchy, audioMenu, settingsMenu);
        audioMenu.SetActive(!audioMenu.activeInHierarchy);
        videoMenu.SetActive(false);
    }

    public void ToggleVideoMenu()
    {
        audioManager.PlaySound(clickSound);
        if(!videoMenu.activeInHierarchy)
        {
            SetButtonSelected(true, videoMenu, settingsMenu);
        }
        else
        {
            settingsMenu.GetComponentsInChildren<Button>()[1].Select();
        }
        videoMenu.SetActive(!videoMenu.activeInHierarchy);
        audioMenu.SetActive(false);
    }

    private void SetButtonSelected(bool currentMenuActivated, GameObject currentMenu, GameObject previousMenu)
    {
        if (currentMenuActivated)
        {
            currentMenu.GetComponentInChildren<Button>().Select();
        }
        else
        {
            previousMenu.GetComponentInChildren<Button>().Select();
        }
    }

    public void OnFullScreenToggle()
    {
        audioManager.PlaySound(clickSound);
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetScreenSize(int width)
    {
        audioManager.PlaySound(clickSound);
        bool fullscreen = Screen.fullScreen;
        switch (width)
        {
            case 1920: Screen.SetResolution(width, 1080, fullscreen); break;
            case 1280: Screen.SetResolution(width, 720, fullscreen); break;
            case 1024: Screen.SetResolution(width, 576, fullscreen); break;
            default: Screen.SetResolution(1920, 1080, fullscreen); break;
        }
    }

    public void ClickedButton()
    {
        audioManager.PlaySound(clickSound);
    }
}
