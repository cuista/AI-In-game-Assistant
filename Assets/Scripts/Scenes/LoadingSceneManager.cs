using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScenesManager : MonoBehaviour
{

    private static LoadingScenesManager _loadingSM;

    public GameObject loadingInterface;
    public Image loadingProgressBar;

    private static List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    void Awake()
    {
        _loadingSM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public static void LoadingScenes(params string[] scenes)
    {
        _loadingSM.loadingInterface.gameObject.SetActive(true);
        foreach (string scene in scenes)
        {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(scene));
            Debug.Log(scene);
        }
        _loadingSM.StartCoroutine(LoadingScreen());
    }

    public static void LoadingScenesAdditive(params string[] scenes)
    {
        foreach (string scene in scenes)
        {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
            Debug.Log(scene);
        }
    }

    private static IEnumerator LoadingScreen()
    {
        float totalProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                _loadingSM.loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }
        _loadingSM.loadingProgressBar.fillAmount = 1;
        _loadingSM.loadingInterface.gameObject.SetActive(false); //_loadingSM.Invoke("DeactivateLoadingScreen", 0.5f);
    }

    private void DeactivateLoadingScreen()
    {
        _loadingSM.loadingInterface.gameObject.SetActive(false);
    }
}
