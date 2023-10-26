using Inworld;
using Inworld.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InworldAIController : MonoBehaviour
{
    public InworldPlayer2D player;
    public InworldCharacter echo;
    public TMP_InputField message;
    private bool _isFirstSentence = true;

    // Start is called before the first frame update
    void Start()
    {
        Chat();
    }

    public void Chat() { StartCoroutine(AIPrompting()); }

    IEnumerator AIPrompting()
    {
        yield return new WaitForSeconds(2);
        if (_isFirstSentence)
        {
            message.text = "Generete a response saying 'Hello, I'm Echo' and then giving an introduction of the prologue to the player with sarcasm. Answer in italian.";
            player.SendText();
            _isFirstSentence = false;
            message.text = "Generete a response telling more to the player or giving help to the player or saying something sarcastic. Answer in italian.";
        }
        else
        {
            player.SendText();
            echo.SendTrigger("prototype_level_started");
        }
    }

    public void TriggerSent()
    {
        Debug.Log("Trigger sent");
    }

    public void TriggerReceived()
    {
        Debug.Log("Trigger received");
    }
}
