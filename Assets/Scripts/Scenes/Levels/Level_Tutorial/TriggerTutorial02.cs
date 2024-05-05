using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTutorial02 : MonoBehaviour
{
    [SerializeField] private SceneController_Tutorial controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.ShowEchoTutorial();
            Destroy(this.gameObject);
        }
    }
}
