using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedGameObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
    }

    //Restart level
    public void Retry()
    {
        DontDestroyOnLoadManager.GetPlayer().GetComponent<CloningSystem>().ClearClones();
        ExplosionController.ClearExplosions();
        DontDestroyOnLoadManager.DestroyAll();
        LoadingScenesManager.LoadingScenes("Gameplay", SceneManager.GetActiveScene().name);
    }

    //Back to MainMenu
    public void Exit()
    {
        DontDestroyOnLoadManager.GetPlayer().GetComponent<CloningSystem>().ClearClones();
        ExplosionController.ClearExplosions();
        DontDestroyOnLoadManager.DestroyAll();
        LoadingScenesManager.LoadingScenes("MainMenu");
        Time.timeScale = 1f;
    }
}
