using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private bool reverse = false;

    private Vector3 buttonPressed;
    private Vector3 buttonReleased;

    private int numColliders;

    // Start is called before the first frame update
    void Start()
    {
        buttonPressed = new Vector3(transform.parent.position.x, transform.parent.position.y - 0.15f, transform.parent.position.z);
        buttonReleased = transform.parent.position;
        numColliders = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Clone"))
        {
            transform.parent.position = buttonPressed;
            door.SendMessage(!reverse?"Activate":"Deactivate");
            numColliders++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Clone"))
        {
            numColliders--;
            if(numColliders == 0)
            {
                transform.parent.position = buttonReleased;
                door.SendMessage(!reverse?"Deactivate":"Activate");
            }
        }
    }
}
