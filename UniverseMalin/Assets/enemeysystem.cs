using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemeysystem : MonoBehaviour
{
    public float health = 100;
    public Slider slider;

    void Start()
    {
        slider.maxValue = health;
        sethealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takedamage(float x)
    {
        health = health - x;
        sethealth();
        if(health <=0)
        {
            Destroy(gameObject);
        }
    }
    private void sethealth()
    {
        slider.value = health;
    }
}
