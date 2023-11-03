using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoadManager.PermanentObject(this.gameObject);
    }
}
