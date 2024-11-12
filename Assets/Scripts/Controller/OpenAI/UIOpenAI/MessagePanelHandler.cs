using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class MessagePanelHandler : MonoBehaviour
{
    [SerializeField] GameObject messageHUDPrefab;
    [SerializeField] GameObject content;

    public int fequencyInSeconds = 3;
    private bool _cancelResponse;

    // Start is called before the first frame update
    void Start()
    {
        _cancelResponse = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void AppendMessage(string message)
    {
        //Showing in the HUD new messages with the response
        string[] sentences = Regex.Split(message, @"(?<=\.)\s+");
        foreach (string sentence in sentences)
        {
            if(_cancelResponse)
            {
                _cancelResponse = false;
                break;
            }
            GameObject messageHUD = Instantiate(messageHUDPrefab);
            messageHUD.GetComponent<MessageHUD>().SetMessage("NOVA", sentence);
            messageHUD.transform.SetParent(content.transform);
            await Task.Delay(fequencyInSeconds * 1000);
        }
    }

    public void CancelResponse()
    {
        _cancelResponse = true;
    }
}
