using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy : MonoBehaviour
{
    public float destroyDelay = 2f;

    void Start()
    {
        // Call the DestroySelf method after the specified delay
        Invoke("DestroySelf", destroyDelay);
    }

    void DestroySelf()
    {
        // Destroy the GameObject this script is attached to
        Destroy(gameObject);
    }
}

