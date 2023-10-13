using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class posionswamp : MonoBehaviour
{
    public int damage = 100;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide");
        if(collision.collider.CompareTag("Player"))
        {
            Debug.Log("player on top");
            collision.gameObject.GetComponent<takedamage>().Takedamage(damage);
        }
        Debug.Log(collision.gameObject.tag);
        Debug.Log(collision.gameObject.name);
    }
}
