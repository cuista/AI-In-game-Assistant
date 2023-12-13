using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private static ExplosionController _explosionController;

    void Awake()
    {
        _explosionController = this;
    }

    //Instantiate explosion and destroy after its duration
    public static void MakeItBoom(GameObject explosionEffect, Transform origin)
    {
        GameObject explosion = Instantiate(explosionEffect, origin.position, origin.rotation);
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
    }
}
