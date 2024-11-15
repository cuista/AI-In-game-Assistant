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
        GameObject inworldAIController = DontDestroyOnLoadManager.GetInworldAIController();
        if(inworldAIController != null)
        {
            assistantInworldAI = DontDestroyOnLoadManager.GetInworldAIController().GetComponent<InworldAIController>();
        }

        GameObject openAIController = DontDestroyOnLoadManager.GetOpenAIController();
        if (openAIController != null)
        {
            assistantOpenAI = DontDestroyOnLoadManager.GetOpenAIController().GetComponent<AssistantOpenAIController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (assistantInworldAI != null)
            {
                assistantInworldAI.SetCurrentTrigger(triggerName);
            }
            if (assistantOpenAI != null)
            {
                assistantOpenAI.SetCurrentTrigger(triggerName);
            }
            Destroy(this.gameObject);
        }
    }
}
