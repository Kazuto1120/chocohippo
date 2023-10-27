using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class boss2bullet : MonoBehaviour
{
    public Vector3 position;
    public Rigidbody rb;
    public float speed = 5;
    public LayerMask playerlayer;
    public float lookRadius;
    public bool playerInsightRange;
    Collider playercharacter;
    public float rotatespeed;
    private RaycastHit hit;
    public GameObject explosion;
    public float damage , range;

    void Start()
    {
        
    }


    private void FixedUpdate()
    {
        
        
        rb.velocity = transform.up * speed;
        directionM();
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        explode();
    }
    private void directionM()
    {
        Vector3 direction;
        playerInsightRange = Physics.CheckSphere(transform.position, lookRadius, playerlayer);
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, lookRadius, playerlayer);
        if (playerCollider.Length > 0)
        {
            playercharacter = playerCollider[0];
            direction = playercharacter.transform.position - rb.position;
            direction.Normalize();
            float rotatex = Vector3.Cross(direction, transform.up).x;
            float rotatez = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = new Vector3(-rotatex, 0, -rotatez) * rotatespeed;
        }
        else
        {
            playercharacter = null;
        }
    }
    public void explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Collider[] players = Physics.OverlapSphere(transform.position, range, playerlayer);
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<playerMovement>().Takedamage(damage);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
