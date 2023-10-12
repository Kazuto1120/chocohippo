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
    public Animator animator;

    private float idle2Chance = 0.25f;
    private float timeBetweenIdle2Checks = 1.0f;
    private float lastIdle2CheckTime;

    public Vector3 walkpoint;
    bool walkPointset;
    public float walkrange;
    bool iding =false;

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
        else if(!iding)
        {
            agent.SetDestination(walkpoint);
            animator.SetBool("moving",true);
            transform.LookAt(walkpoint);
        }
        else
        {
            idle();
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
        animator.SetBool("moving", true);
        agent.SetDestination(playercharacter.transform.position);
        transform.LookAt(playercharacter.transform.position);
        
    }
    private void attackPlayer()
    {
        animator.SetBool("moving", false);
        animator.SetTrigger("attack");
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
    private void idle()
    {
        iding = true;
        animator.SetBool("moving", false);
    }
    private void idle2()
    {
        animator.SetTrigger("switch");
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
        walkpoint = new Vector3(transform.position.x + randomx, transform.position.y,transform.position.z +randomz);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, ground))
        {
            walkPointset = true;
            iding = false;
        }
        Debug.Log("newwalkset");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawLine(transform.position, walkpoint);
    }

}
