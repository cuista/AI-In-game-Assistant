using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelOneWayPlatformMovement : MonoBehaviour
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
            if (GetComponent<OneWayPlatformMovement>().isOperating)
            {
                labelOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM TRIGGERED AND IS MOVING TO THE OTHER SIDE";
            }
            else
            {
                labelOneWayPlatformMovement.GetComponent<TMPro.TMP_Text>().text = "PLATFORM NEEDS TO BE TRIGGERED TO MOVE TO THE OTHER SIDE";
            }
        }
    }
}
