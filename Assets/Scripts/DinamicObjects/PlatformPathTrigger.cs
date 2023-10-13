using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPathTrigger : MonoBehaviour
{
    [SerializeField] private GameObject platformPath;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformPath.SendMessage("Activate");
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformPath.SendMessage("Deactivate");
            other.transform.SetParent(null);
        }
    }
}
