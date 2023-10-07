using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatorThree : MonoBehaviour
{
    //Use to keep track of time
    float timeCount = 0;

    //Adding speed,width and height
    float speed, width, height;


    // Start is called before the first frame update
    void Start()
    {
        speed = 1.0f;
        width = 2.0f;
        height = 5.0f;

    }

    // FixedUpdate is called once per frame
    //the function is going to change position based on the time.
    //if time changes,motion changes as well.

    void FixedUpdate()
    {

        //using trig functions.
        timeCount += Time.deltaTime * speed;
        float x = Mathf.Sin(timeCount) * height;
        float y = Mathf.Cos(timeCount) * width;
        float z = 15;

        transform.position = new Vector3(x, y, z);
    }
}

