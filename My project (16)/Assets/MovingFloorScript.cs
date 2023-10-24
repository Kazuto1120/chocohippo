using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloorScript : MonoBehaviour
{
    //declaring array to hold more than 2 waypoints
    [SerializeField] GameObject[] waypoints;
    int currentWaypointIndex = 0;

    //variable for the speed of platform
    [SerializeField] float speed = 1f;

    // Fixed Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < .1f)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Length)
            {
                  currentWaypointIndex = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position,waypoints[currentWaypointIndex].transform.position, speed*Time.deltaTime);
    }
}
