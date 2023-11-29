using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleport : MonoBehaviour
{
    [SerializeField] private string _levelSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() != null)
        {
            if(_levelSceneName!=null)
            {
                LoadingScenesManager.LoadingScenes(_levelSceneName);
            }
        }
    }
}
