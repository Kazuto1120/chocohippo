using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemymovement : MonoBehaviour
{
    public float lookRadius, attackRadius;
    bool playerInsightRange, playerinattackrange;
    Collider playercharacter;


    public NavMeshAgent agent;
    public LayerMask ground, playerlayer;
    public GameObject attack;
    

    public Vector3 walkpoint;
    bool walkPointset;
    public float walkrange;

    public float timebetweenattacks;
    bool alreadyattack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        playerInsightRange = Physics.CheckSphere(transform.position, lookRadius, playerlayer);
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, lookRadius, playerlayer);
        if (playerCollider.Length>0)
        {
            playercharacter = playerCollider[0];
        }
        else
        {
            playercharacter = null;
        }
        playerinattackrange = Physics.CheckSphere(transform.position, attackRadius,playerlayer);
        Collider[] playerColliderA = Physics.OverlapSphere(transform.position, attackRadius, playerlayer);
        if (playerColliderA.Length > 0)
        {
            playercharacter = playerColliderA[0];
        }

        if (!playerInsightRange && !playerinattackrange)
        {
            Patrolling();
        }else if (playerInsightRange && !playerinattackrange)
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
        if (!walkPointset)
        {
            Searchwalkpoint();
        }
        else
        {
            agent.SetDestination(walkpoint);
            transform.LookAt(walkpoint);
        }
        Vector3 distance = transform.position - walkpoint;
        if (distance.magnitude <= 2f)
        {
            Debug.Log("reach");
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
    private void attackstage2()
    {

    }
    private void resetattack()
    {
        alreadyattack = false;
    }
    private void Searchwalkpoint()
    {

        float randomz = Random.Range(-walkrange, walkrange);
        float randomx = Random.Range(-walkrange, walkrange);
        walkpoint = new Vector3(transform.position.x + randomx, transform.position.y,transform.position.z +randomz);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, ground))
        {
            walkPointset = true;
        }
        Debug.Log("newwalkset");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

}
