using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;
    private Vector3 _startingPos;
    private Vector3 _finalPos;
    [SerializeField] float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        _startingPos = transform.position;
        _finalPos = _finalPos + targetPosition;
    }

    // Update is called once per frame
    void FixedUpdate() //Script Execution Order => 300
    {
        float step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _finalPos, step);

        //If it's close to the final position, invert direction
        if (Vector3.Distance(transform.position, _finalPos) < 0.01f)
        {
            //Swap final and starting point
            Vector3 temp = _startingPos;
            _startingPos = _finalPos;
            _finalPos = temp;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
