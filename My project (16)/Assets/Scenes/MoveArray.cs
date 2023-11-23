using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveArray : MonoBehaviour
{

    //creating gameobject list
    public List<GameObject> waypoints;
    public float speed = 2;
    public bool goBack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int index = 0;
        Vector3 destination = waypoints[index].transform.position;
       Vector3 newPos = Vector3.MoveTowards(transform.position, waypoints[index].transform.position, speed * Time.deltaTime);
        transform.position = newPos;

        //creating logic for distance
        float distance = Vector3.Distance(transform.position, destination); 
        if(distance <- 0.07)
        {
            if (index < waypoints.Count - 1)
            {
                index++;
            }
            else
            {
                if (goBack)
                {
                    index = 0;
                }
            }
        }
    }
}
