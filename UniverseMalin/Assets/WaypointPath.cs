using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] GameObject[] way_points;
    int currentPosition = 0;
    [SerializeField] float speed = 2f;

    

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, way_points[currentPosition].transform.position) < .1f)
        {
            currentPosition++;
            if(currentPosition >= way_points.Length)
            {
                currentPosition = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, way_points[currentPosition].transform.position, speed * Time.deltaTime);
    }
}
