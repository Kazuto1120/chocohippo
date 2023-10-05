using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletscript : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask player;

    [Range(0f, 1f)]
    public float bounce;
    public bool gravity;

    public float damage;
    public float range;

    public int maxCollision;
    public float lifetime;
    public bool playerCollide =true;
    // Start is called before the first frame update
    int collisions;
    PhysicMaterial physicM;
    private void Start()
    {
        Setup();   
    }
    private void Update()
    {
        if(collisions > maxCollision)
        {
            Explode();
        }
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Explode();
        }
    }
    private void Explode()
    {
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        Collider[] players = Physics.OverlapSphere(transform.position, range, player);
        for(int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<playerMovement>().Takedamage(damage);
        }
        Invoke("Delay", 0.05f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if(collision.collider.CompareTag("Player") && playerCollide)
        {
            Explode();
        }
    }
    private void Setup()
    {
        physicM = new PhysicMaterial();
        physicM.bounciness = bounce;
        physicM.frictionCombine = PhysicMaterialCombine.Minimum;
        physicM.frictionCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physicM;

        rb.useGravity = gravity;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
