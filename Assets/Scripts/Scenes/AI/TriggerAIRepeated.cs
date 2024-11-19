using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAIRepeated : MonoBehaviour
{
    private AssistantOpenAIController assistantOpenAI;
    public string triggerName;

    private void Awake()
    {
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
            if (assistantOpenAI != null)
            {
                assistantOpenAI.SetCurrentTrigger(triggerName);
            }
            Destroy(this.gameObject);
        }
    }
}
