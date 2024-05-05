using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial01 : MonoBehaviour
{
    [SerializeField] private SceneController_Tutorial controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.ShowItemsTutorial();
            Destroy(this.gameObject);
        }
    }
}
