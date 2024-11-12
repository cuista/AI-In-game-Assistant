using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAIOneShot : MonoBehaviour
{
    private InworldAIController controllerAI;
    public string triggerName;

    private void Awake()
    {
        controllerAI = DontDestroyOnLoadManager.GetInworldAIController().GetComponent<InworldAIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controllerAI.OneShotTrigger(triggerName);
            Destroy(this.gameObject);
        }
    }
}
