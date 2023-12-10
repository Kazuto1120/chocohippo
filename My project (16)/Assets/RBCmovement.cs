using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCmovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 5.0f;
    public float offset = 0.0f;
    public GameObject temp;


    private float originalX;
    void Start()
    {
        originalX = transform.position.x;
    }

    void Update()
    {
        float xPosition = originalX + Mathf.Sin(Time.time * speed + offset) * distance;

        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            temp = collision.collider.transform.parent.gameObject;
            collision.collider.transform.parent.SetParent(transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            temp.transform.SetParent(null);
            temp = null;
        }
    }
}
