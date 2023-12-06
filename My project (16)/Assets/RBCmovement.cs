using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCmovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 5.0f;
    public float offset = 0.0f;
    public GameObject temp;
    // Start is called before the first frame update

    private float originalX;
    void Start()
    {
        originalX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float xPosition = originalX + Mathf.Sin(Time.time * speed + offset) * distance;

        // Set the GameObject's position
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("test1");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("test");
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
