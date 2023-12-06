using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantSystem : MonoBehaviour
{
    [SerializeField] private InworldAIController assistantController;
    private bool hintPressed;
    private bool mutePressed;

    // Start is called before the first frame update
    void Start()
    {
        hintPressed = false;
        mutePressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(assistantController != null)
        {
            if(Input.GetButtonDown("HintAssistant"))
            {
                hintPressed = true;
            }
            if(Input.GetButtonDown("MuteAssistant"))
            {
                mutePressed = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (assistantController != null)
        {
            if (hintPressed)
            {
                assistantController.HintTrigger();
                hintPressed = false;
            }
            else if (mutePressed)
            {
                assistantController.Mute();
                mutePressed = false;
            }
        }
    }
}
