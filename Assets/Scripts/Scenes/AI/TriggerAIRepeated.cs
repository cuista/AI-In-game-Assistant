using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAIRepeated : MonoBehaviour
{
    private InworldAIController assistantInworldAI;
    private AssistantOpenAIController assistantOpenAI;
    public string triggerName;

    private void Awake()
    {
        assistantInworldAI = DontDestroyOnLoadManager.GetInworldAIController().GetComponent<InworldAIController>();
        assistantOpenAI = DontDestroyOnLoadManager.GetOpenAIController().GetComponent<AssistantOpenAIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            assistantInworldAI.SetCurrentTrigger(triggerName);
            assistantOpenAI.SetCurrentTrigger(triggerName);
            Destroy(this.gameObject);
        }
    }
}
