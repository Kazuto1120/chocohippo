using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlodMove : MonoBehaviour
{
    public Transform[] wayspot;
    public float speed = 5.0f;
    private int currentWayspot = 0;

    void FixedUpdate()
    {
        if (wayspot.Length == 0) return;
        transform.position = Vector3.MoveTowards(transform.position, wayspot[currentWayspot].position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, wayspot[currentWayspot].position) < 0.1f)
        {
            currentWayspot = (currentWayspot + 1) % wayspot.Length;
        }
    }
}
