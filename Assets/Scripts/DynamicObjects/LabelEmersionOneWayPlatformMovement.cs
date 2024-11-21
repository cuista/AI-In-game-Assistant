using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelEmersionOneWayPlatformMovement : MonoBehaviour
{
    [SerializeField] GameObject labelEmersionOneWayPlatformMovement;
    private Camera _camera;

    private void Start()
    {
        labelEmersionOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "";
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null)
        {
            labelEmersionOneWayPlatformMovement.transform.rotation = Quaternion.LookRotation(labelEmersionOneWayPlatformMovement.transform.position - Camera.main.transform.position);
            if (GetComponent<OneWayPlatformMovement>().isEmerged())
            {
                labelEmersionOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM TRIGGERED AND IS EMERGED TO THE SURFACE";
            }
            else
            {
                labelEmersionOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM NEEDS TO BE TRIGGERED TO EMERGE TO THE SURFACE";
            }
        }
    }
}
