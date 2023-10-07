//This C# script implements circular motion for an object.
//Basically position changes as time changes.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    //Use to keep track of time
    float timeCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        //no code
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {

        //using trig functions.
        timeCount += Time.deltaTime;
        float x = Mathf.Cos (timeCount);
        float y = Mathf.Sin(timeCount);
        float z = 0;
        
        transform.position = new Vector3(x,y,z);
    }
}
