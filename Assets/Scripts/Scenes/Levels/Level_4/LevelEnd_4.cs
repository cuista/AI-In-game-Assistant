using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd_4 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() != null)
        {
            DontDestroyOnLoadManager.RestorePlayerDDOLScene(); // Player in DDOL scene, Platform parenting could move Player to other scene
            DontDestroyOnLoadManager.GetPlayer().GetComponent<CloningSystem>().ClearClones();
            ExplosionController.ClearExplosions();
            LoadingScenesManager.LoadingScenes("Level_5");
        }
    }
}
