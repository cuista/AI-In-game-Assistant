using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelPlatformMovement : MonoBehaviour
{
    [SerializeField] GameObject labelPlatformMovement;
    private Camera _camera;

    private void Start()
    {
        labelPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "";
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null)
        {
            labelPlatformMovement.transform.rotation = Quaternion.LookRotation(labelPlatformMovement.transform.position - Camera.main.transform.position);
            if(GetComponent<PlatformMovement>().isOperating)
            {
                labelPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM IS MOVING";
            }
            else
            {
                labelPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM IS STATIONARY";
            }
        }
    }
}
