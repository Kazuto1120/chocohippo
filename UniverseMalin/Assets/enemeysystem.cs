using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemeysystem : MonoBehaviour
{
    public float health = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takedamage(float x)
    {
        health = health - x;
        if(health <=0)
        {
            Destroy(gameObject);
        }
    }
}
