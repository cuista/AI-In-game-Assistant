using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAIRepeated : MonoBehaviour
{
    private InworldAIController controllerAI;
    public string triggerName;

    private void Awake()
    {
        controllerAI = DontDestroyOnLoadManager.GetAIController().GetComponent<InworldAIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controllerAI.SetCurrentTrigger(triggerName);
            Destroy(this.gameObject);
        }
    }
}
