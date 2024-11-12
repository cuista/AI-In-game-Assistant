using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageHUD : MonoBehaviour
{
    [SerializeField] TMP_Text _title;
    [SerializeField] TMP_Text _textField;

    public void SetMessage(string title, string text = null)
    {
        if (_title)
            _title.text = title;
        if (_textField && !string.IsNullOrEmpty(text))
            _textField.text = text;
    }
}