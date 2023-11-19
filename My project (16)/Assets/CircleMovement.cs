using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CircleMovement : MonoBehaviour
{
    public float object_speed = 3f; // Speed of the movement
    public float radius = 12f; // Radius of the circle

   

    private Vector3 centerPosition; // Center position of the circle
    private float angle = 2f;

    

    private void Start()
    {
        centerPosition = transform.position; // Set the initial center position
    }
    //Fixed Update function
    private void FixedUpdate()
    {
        // Calculate the new position on the circle based on the current angle
        float xVal = centerPosition.x + Mathf.Sin(angle) * radius;
        float zVal = centerPosition.z + Mathf.Cos(angle) * radius;

        // Update the position of the moving object
        
        transform.position = new Vector3(xVal, transform.position.y, zVal);

        // Increases the angle based on the speed and time *2
        angle += object_speed * Time.deltaTime*2;

        // Wrap around the angle to keep it within 0 to 360 degrees range
        if (angle >= 360f)
            angle -= 360f;
    }
}