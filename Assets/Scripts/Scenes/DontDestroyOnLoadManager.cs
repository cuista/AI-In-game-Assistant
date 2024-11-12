using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class DontDestroyOnLoadManager
{
    //Permanent never be destroyed, ddol go through the scene but can be destroyed
    private static List<GameObject> _permanentObjects = new List<GameObject>();
    private static List<GameObject> _ddolObjects = new List<GameObject>();

    public static void PermanentObject(this GameObject go)
    {
        UnityEngine.Object.DontDestroyOnLoad(go);
        _permanentObjects.Add(go);
    }

    public static void DontDestroyOnLoad(this GameObject go)
    {
        UnityEngine.Object.DontDestroyOnLoad(go);
        _ddolObjects.Add(go);
    }

    //destroy all ddol
    public static void DestroyAll()
    {
        foreach (var go in _ddolObjects)
        {
            if (go != null)
                UnityEngine.Object.Destroy(go);
        }
        _ddolObjects.Clear();
    }

    public static GameObject GetPlayer()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.GetComponent<PlayerCharacter>() != null)
                return go;
        }
        return null;
    }

    public static GameObject GetLoadingScreen()
    {
        foreach (var go in _permanentObjects)
        {
            if (go.tag == "LoadingScreen")
                return go;
        }
        return null;
    }

    public static GameObject GetMainCamera()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "MainCamera")
                return go;
        }
        return null;
    }

    public static GameObject GetHUD()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "HUD")
                return go;
        }
        return null;
    }

    public static GameObject GetSkipMessage()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "SkipMessage")
                return go;
        }
        return null;
    }

    public static GameObject GetInworldAIController()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "InworldAIController")
                return go;
        }
        return null;
    }

    public static GameObject GetOpenAIController()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "OpenAIController")
                return go;
        }
        return null;
    }

    public static GameObject GetEchoAI()
    {
        foreach (var go in _ddolObjects)
        {
            if (go.tag == "EchoAI")
                return go;
        }
        return null;
    }

    public static AudioManager GetAudioManager()
    {
        foreach (var go in _permanentObjects)
        {
            if (go.tag == "AudioManager")
                return go.GetComponent<AudioManager>();
        }
        return null;
    }

    public static GameObject GetRestorePlayerDDOL()
    {
        foreach (var go in _permanentObjects)
        {
            if (go.tag == "RestorePlayerDDOL")
                return go;
        }
        return null;
    }

    public static void RestorePlayerDDOLScene()
    {
        Transform playerTransform = GetPlayer().transform;
        playerTransform.SetParent(GetRestorePlayerDDOL().transform);
        playerTransform.transform.parent = null;
    }
}
