/*
using ReadSpeaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reader : MonoBehaviour
{
    public TTSSpeaker speaker;

    // Start is called before the first frame update
    void Start()
    {
        TTS.Init();
        waitIfReading();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TTS.InterruptAll();
            StopAllCoroutines();
            waitIfReading();
        }
    }

    public void readTTS(string text)
    {
        waitIfReading();
        TTS.Say(text, speaker);
    }

    IEnumerator waitIfReading()
    {
        yield return new WaitUntil(() => !speaker.audioSource.isPlaying);
    }
}
*/