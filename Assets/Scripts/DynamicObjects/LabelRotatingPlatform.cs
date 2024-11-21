using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelRotatingPlatform : MonoBehaviour
{
    [SerializeField] GameObject labelOneWayPlatformMovement;
    private Camera _camera;

    private void Start()
    {
        labelOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "";
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null)
        {
            labelOneWayPlatformMovement.transform.rotation = Quaternion.LookRotation(labelOneWayPlatformMovement.transform.position - Camera.main.transform.position);
            if (GetComponent<RotatingPlatform>().isOperating)
            {
                labelOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM TRIGGERED AND ROTATING";
            }
            else
            {
                labelOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM NEEDS TO BE TRIGGERED TO ROTATE";
            }
        }
    }
}
