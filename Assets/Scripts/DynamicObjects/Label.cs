using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : MonoBehaviour
{
    [SerializeField] GameObject label;
    private Camera _camera;

    private void Start()
    {
        label.GetComponent<TMPro.TMP_Text>().text = "LABEL TO SHOW";
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null)
        {
            label.transform.rotation = Quaternion.LookRotation(label.transform.position - Camera.main.transform.position);
        }
    }
}
