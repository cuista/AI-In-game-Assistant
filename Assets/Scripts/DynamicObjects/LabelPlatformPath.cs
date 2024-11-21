using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelPlatformPath : MonoBehaviour
{
    [SerializeField] GameObject labelPlatformPath;
    private Camera _camera;

    private void Start()
    {
        labelPlatformPath.GetComponent<TMPro.TMP_Text>().text = "";
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null)
        {
            labelPlatformPath.transform.rotation = Quaternion.LookRotation(labelPlatformPath.transform.position - Camera.main.transform.position);
            if (GetComponent<PlatformPathAnimation>().isActivated)
            {
                labelPlatformPath.GetComponent<TMPro.TMP_Text>().text = "PLATFORM TRIGGERED WITH PLAYER ABOVE IT AND MOVING ON A SPECIFIC PATH";
            }
            else
            {
                labelPlatformPath.GetComponent<TMPro.TMP_Text>().text = "PLATFORM NEEDS TO BE TRIGGERED WITH PLAYER ABOVE TO MOVE ON A SPECIFIC PATH";
            }
        }
    }
}
