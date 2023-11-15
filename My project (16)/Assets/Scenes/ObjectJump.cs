
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectJump : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 targetPosition;

    void Start()
    {
        // Set the initial target position
        targetPosition = new Vector3(5f, 0f, 0f);
    }

    void Update()
    {
        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If the object reaches the target position, set a new random target position
        if (transform.position == targetPosition)
        {
            targetPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
        }
    }
}