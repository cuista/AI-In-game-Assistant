using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd_2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() != null)
        {
            LoadingScenesManager.LoadingScenes("ReturnToMenuScene");
        }
    }
}
