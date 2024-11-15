using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAIOneShot : MonoBehaviour
{
    private InworldAIController assistantInworldAI;
    private AssistantOpenAIController assistantOpenAI;
    public string triggerName;

    private void Awake()
    {
        GameObject inworldAIController = DontDestroyOnLoadManager.GetInworldAIController();
        if (inworldAIController != null)
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
            if(assistantInworldAI != null)
            {
                assistantInworldAI.OneShotTrigger(triggerName);
            }
            if(assistantOpenAI != null)
            {
                assistantOpenAI.OneShotTrigger(triggerName);
            }
            Destroy(this.gameObject);
        }
    }
}
