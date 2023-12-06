using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class boss3movement : MonoBehaviour
{
    public float lookRadius, look, attackRadius;
    bool playerInsightRange, playerinattackrange;
    Collider playercharacter;
    public AudioSource audio;

    public PhotonView view;
    public float maxhealth = 1000;
    public float health = 100;
    public Slider slider;
    private float tempD = 0;

    public GameObject minion;
    public int totalminion = 2;
    public int currentminion = 0;

    public NavMeshAgent agent;
    public LayerMask ground, playerlayer;
    public GameObject attack;
    public Animator animator;
    public Animator animatortwo;

    private float idle2Chance = 0.25f;
    private float timeBetweenIdle2Checks = 1.0f;
    private float lastIdle2CheckTime;

    public Vector3 walkpoint;
    bool walkPointset;
    public float walkrange;
    bool iding = false;

    public float timebetweenattacks;
    bool alreadyattack = false;
    public int damage = 10;
    public float bounceForce = 2;
    public bool bury = false;
    public float speed = 20;
    public ParticleSystem particle;
    private RaycastHit hit;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        particle.Stop();
        agent.speed = speed;
        look = lookRadius;
        health = maxhealth;
        view = GetComponent<PhotonView>();
        slider.maxValue = health;
        view.RPC("sethealth", RpcTarget.AllBuffered);
    }

    private void FixedUpdate()
    {
        if (bury)
        {
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle.gameObject.SetActive(false);
        }
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
        else if(!alreadyattack)
        {
            attackPlayer();
        }
    }
    private void Patrolling()
    {
        if (!walkPointset && !iding)
        {
            
            animator.SetBool("moving", false);
            bury = false;
            
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
            
            if(Physics.Raycast(transform.position, transform.forward, 1f))
            {
                Debug.Log("block");
                Searchwalkpoint();
            }
            
           
            agent.speed = speed;
            bury = true;
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
        
        agent.speed = 2 * speed;
        animator.SetBool("moving", true);
        bury = true;
        agent.SetDestination(playercharacter.transform.position);
        transform.LookAt(playercharacter.transform.position);

    }
    private void attackPlayer()
    {
        
        animator.SetBool("moving", false);
        bury = false;
        animator.SetTrigger("attack");
        Collider[]players = Physics.OverlapSphere(transform.position, attackRadius, playerlayer);
        if(players.Length > 0&&speed > 10)
        {
            speed = speed / 10;
            Invoke(nameof(speedreset), timebetweenattacks * 2);
        }
        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<playerMovement>().Takedamage(damage);
            players[i] = null;
        }
        
        agent.SetDestination(transform.position);
        transform.LookAt(playercharacter.transform.position);
        if (!alreadyattack)
        {
            alreadyattack = true;
            Invoke(nameof(resetattack), timebetweenattacks);
        }
    }
    public void speedreset()
    {
        speed = speed * 10;
    }
    
    private void idle()
    {
        iding = true;
        animator.SetBool("moving", false);
        bury = false;

    }
    private void idle2()
    {
        animator.SetTrigger("switch");
        Invoke(nameof(Searchwalkpoint), 3f);
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
            animator.SetBool("moving", true);
            bury = true;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<takedamage>().Takedamage(damage);
        }
        if (collision.gameObject.layer == 11)
        {
            Searchwalkpoint();
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
        if (!bury)
        {
            view.RPC("takedamage2", RpcTarget.AllBuffered, x);
        }
    }
    
    [PunRPC]
    private void takedamage2(float x)
    {
        if(lookRadius < look * 6)
        lookRadius = lookRadius * 5;
        health = health - x;
        
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