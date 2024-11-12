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
    private bool _isAppending;

    private Queue<string> _buffer; //FIFO
    private int _capacity = 3;

    // Start is called before the first frame update
    void Start()
    {
        _cancelResponse = false;
        _isAppending = false;
        _buffer = new Queue<string>(_capacity);
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isAppending && _buffer.Count > 0)
        {
            AppendMessage(_buffer.Dequeue());
        }
    }

    public void AddMessage(string message)
    {
        //Remove oldest message
        if (_buffer.Count >= _capacity)
        {
            _buffer.Dequeue();
        }

        _buffer.Enqueue(message);
    }

    public async void AppendMessage(string message)
    {
        _isAppending = true;

        //Clean and split the string
        Regex.Replace(message, "[-_@#]", "");
        string[] sentences = Regex.Split(message, @"(?<=[.!?–])\s+");

        //Show in the HUD the new messages
        foreach (string sentence in sentences)
        {
            if(_cancelResponse)
            {
                _cancelResponse = false;
                break;
            }
            GameObject messageHUD = Instantiate(messageHUDPrefab);
            messageHUD.GetComponent<MessageHUD>().SetMessage("NOVA", sentence);
            if(content != null)
            {
                messageHUD.transform.SetParent(content.transform);
            }
            await Task.Delay(fequencyInSeconds * 2000);
        }

        _isAppending = false;
    }

    public void CancelResponse()
    {
        _cancelResponse = true;
    }
}
