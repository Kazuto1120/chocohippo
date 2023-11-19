using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;



public class enemymovement : MonoBehaviour
{
    public float lookRadius,look, attackRadius;
    bool playerInsightRange, playerinattackrange;
    Collider playercharacter;

    public PhotonView view;
    public float maxhealth = 1000;
    public float health = 100;
    public Slider slider;
    private float tempD = 0;

    public GameObject minion;
    public int totalminion =2;
    public int currentminion = 0;

    public NavMeshAgent agent;
    public LayerMask ground, playerlayer;
    public GameObject attack;
    public Animator animator;
    public Animator animatortwo;
    public AudioSource audio;

    private float idle2Chance = 0.25f;
    private float timeBetweenIdle2Checks = 1.0f;
    private float lastIdle2CheckTime;

    public Vector3 walkpoint;
    bool walkPointset;
    public float walkrange;
    bool iding =false;

    public float timebetweenattacks;
    bool alreadyattack;
    public int damage = 10;
    public float bounceForce = 2;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        look = lookRadius;
        health = maxhealth;
        view = GetComponent<PhotonView>();
        slider.maxValue = health;
        view.RPC("sethealth", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        if (health <= (maxhealth / 2) && currentminion < totalminion)
        {
            attackstage2();
        }
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
        animator.SetTrigger("attack");
        
        Collider[] playerColliderh = Physics.OverlapSphere(transform.position, 1000f, playerlayer);
        if (playerColliderh.Length > 0)
        {
             playercharacter = playerColliderh[0];
        }
        transform.LookAt(playercharacter.transform.position);
        attack.GetComponent<enemyattack>().shoot2(playercharacter);
        currentminion++;


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
    public void takedamage(float x)
    {
        view.RPC("takedamage2", RpcTarget.AllBuffered, x);
    }
    [PunRPC]
    private void takedamage2(float x)
    {
        lookRadius = lookRadius * 5;
        Debug.Log(x);
        health = health - x;
        tempD += x;
        if (tempD >= 75)
        {
            if (0 > Random.RandomRange(-10, 10) && health <= maxhealth / 2)
            {
                GetComponent<enemymovement>().totalminion += 1;
            }
            tempD = 0;
        }
        view.RPC("sethealth", RpcTarget.AllBuffered);
        if (health <= 0)
        {
            animator.SetTrigger("dead");
            audio.Play();
            animatortwo.SetTrigger("bossroom");
            StartCoroutine(DestroyAfterDelay(2f));
        }
    }
    [PunRPC]
    private void sethealth()
    {
        slider.value = health;
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawLine(transform.position, walkpoint);
    }

}
