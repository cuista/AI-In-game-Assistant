using Ai.Inworld.Studio.V1Alpha;
using Inworld;
using Inworld.Sample;
using Inworld.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InworldAIController : MonoBehaviour
{
    public InworldPlayer2D player;
    public InworldCharacter echo;
    public TMP_InputField message;
    private bool _isFirstSentence = true;

    //A dictionary that maps plot events to trigger events
    private List<string> levelTriggers = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        player = (player==null)?FindObjectOfType<InworldPlayer2D>():player;
        echo = (echo == null) ? FindObjectOfType<InworldCharacter>() : echo;
        LoadLevelTriggers();
    }

    private void FixedUpdate()
    {
        Invoke("EchoTriggering",2f);
    }

    public void LoadLevelTriggers()
    {
        // Initialize level events triggers
        levelTriggers.Add("prototype_level_started");
        levelTriggers.Add("put_clone_over_button");
        levelTriggers.Add("overcome_first_door");
        levelTriggers.Add("prototype_level_completed");
    }

    private void EchoTriggering()
    {
        float posZ = player.transform.position.z;
        if (_isFirstSentence && posZ < 76f)
        {
            // Trigger the level event
            echo.SendTrigger(levelTriggers[0]);
            Debug.Log("COMPLETED GOAL:" + GetNextPuzzleEvent());
            _isFirstSentence = false;
        }
        else
        {
            if (levelTriggers[0] == "put_clone_over_button" && posZ >= 76f)
            {
                Debug.Log("COMPLETED GOAL:" + GetNextPuzzleEvent());
                echo.SendTrigger(levelTriggers[0]);
            }
            else if (levelTriggers[0] == "overcome_first_door" && posZ >= 102f)
            {
                Debug.Log("COMPLETED GOAL:" + GetNextPuzzleEvent());
                echo.SendTrigger(levelTriggers[0]);
            }
            else if (levelTriggers[0] == "prototype_level_completed")
            {
                _isFirstSentence = true;
                LoadLevelTriggers();
            }
        }
    }

    private string GetNextPuzzleEvent()
    {
        //Get the first available hint from the puzzle hints dictionary
        string levelEvent = levelTriggers[0];
        levelTriggers.RemoveAt(0);

        return levelEvent;
    }
}
