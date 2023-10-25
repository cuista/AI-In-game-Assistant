using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterSystem : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    public GameObject bulletCreationPoint;

    private bool shootPressed;

    // Start is called before the first frame update
    void Start()
    {
        shootPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        shootPressed = Input.GetButtonUp("Shoot") || shootPressed;
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            if (shootPressed)
            {
                GameObject bullet = Instantiate(bulletPrefab) as GameObject;
                bullet.transform.position = (bulletCreationPoint != null) ? bulletCreationPoint.transform.position : transform.TransformPoint(Vector3.forward * 2.5f);
                bullet.transform.rotation = bulletCreationPoint.transform.rotation;
                shootPressed = false;
            }
        }
    }
}
