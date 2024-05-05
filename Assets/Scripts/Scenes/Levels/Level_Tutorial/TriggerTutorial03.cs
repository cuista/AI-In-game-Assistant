using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial03 : MonoBehaviour
{
    [SerializeField] private SceneController_Tutorial controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.ShowClonesTutorial();
            Destroy(this.gameObject);
        }
    }
}
