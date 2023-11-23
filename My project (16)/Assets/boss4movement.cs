using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class boss4movement : MonoBehaviour
{
    public float lookRadius, look, attackRadius;
    bool playerInsightRange, playerinattackrange;
    Collider playercharacter;
    public float maxhealth = 1000;
    public float health = 100;
    public NavMeshAgent agent;

    public LayerMask ground, playerlayer;

    public Vector3 walkpoint;
    public GameObject attack;
    public float timebetweenattacks;
    bool alreadyattack;
    public int damage = 10;
    public float bounceForce = 2;
    private float idle2Chance = 0.25f;
    private float timeBetweenIdle2Checks = 1.0f;
    private float lastIdle2CheckTime;
    bool iding = false;
    bool walkPointset;
    public float walkrange;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        look = lookRadius;
        health = maxhealth;
    }
    private void Update()
    {
        playerInsightRange = Physics.CheckSphere(transform.position, lookRadius, playerlayer);
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, lookRadius, playerlayer);
        if (playerCollider.Length > 0)
        {
            playercharacter = playerCollider[0];
        }
        else
        {
            playercharacter = null;
        }
        playerinattackrange = Physics.CheckSphere(transform.position, attackRadius, playerlayer);
        Collider[] playerColliderA = Physics.OverlapSphere(transform.position, attackRadius, playerlayer);
        if (playerColliderA.Length > 0)
        {
            playercharacter = playerColliderA[0];
        }

        if (iding && Time.time - lastIdle2CheckTime >= timeBetweenIdle2Checks)
        {
            lastIdle2CheckTime = Time.time;
            float randomValue = Random.value;
            if (randomValue <= idle2Chance)
            {
                idle2();
            }
        }

        if (!playerInsightRange && !playerinattackrange)
        {

            Patrolling();

        }
        else if (playerInsightRange && !playerinattackrange)
        {
            chasePlayer();
        }
        else
        {
            attackPlayer();
        }
    }
    private void Patrolling()
    {
        if (!walkPointset && !iding)
        {
            int x = Random.Range(-2, 2);
            if (x < 0)
            {
                idle();
            }
            else
            {
                Searchwalkpoint();
            }
        }
        else if (!iding)
        {
            agent.SetDestination(walkpoint);
            transform.LookAt(walkpoint);
        }
        else
        {
            idle();
        }
        Vector3 distance = transform.position - walkpoint;
        if (distance.magnitude <= 2f)
        {

            walkPointset = false;
        }
    }
    private void chasePlayer()
    {

        agent.SetDestination(playercharacter.transform.position);
        transform.LookAt(playercharacter.transform.position);

    }
    private void attackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(playercharacter.transform.position);
        if (!alreadyattack)
        {
            alreadyattack = true;
            attack.GetComponent<enemyattack>().shoot(playercharacter);
            Invoke(nameof(resetattack), timebetweenattacks);
        }
    }
    private void idle()
    {
        iding = true;

    }
    private void idle2()
    {

        Invoke(nameof(Searchwalkpoint), 2f);
    }
    private void resetattack()
    {
        alreadyattack = false;
    }
    private void Searchwalkpoint()
    {

        float randomz = Random.Range(-walkrange, walkrange);
        float randomx = Random.Range(-walkrange, walkrange);
        walkpoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, ground))
        {
            walkPointset = true;
            iding = false;

        }

    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<takedamage>().Takedamage(damage);
        }

        Vector3 bounceDirection = (transform.position - collision.contacts[0].point).normalized;
        GetComponent<Rigidbody>().AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        StartCoroutine(StopBounceForce());
    }
    private IEnumerator StopBounceForce()
    {
        yield return new WaitForSeconds(0.5f);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawLine(transform.position, walkpoint);
    }

}
