using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy : MonoBehaviour
{
    public float destroyDelay = 1.0f; // Time delay before destruction in seconds

    void Start()
    {
        // Schedule the object for destruction after the specified delay
        Destroy(gameObject, destroyDelay);
    }
}
