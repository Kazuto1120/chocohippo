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
    public Animator animator;
    public bool shell;
    float storage;
    public float test;
    public AudioSource audio;

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
    public PhotonView view;
    public Slider slider;
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
        if (shell)
        {
            animator.SetBool("shell", true);
        }
        else
        {
            animator.SetBool("shell", false);
        }
        if (shell && !alreadyattack)
        {
            alreadyattack = true;
            shellin();
        }
        else if (!playerInsightRange && !playerinattackrange)
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
    private void shellin()
    {
        agent.SetDestination(transform.position);
        health += (maxhealth / 50);
        if (health > maxhealth)
        {
            health = maxhealth;
        }
        view.RPC("sethealth", RpcTarget.AllBuffered);
        Invoke(nameof(resetattack), 1f);


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
            animator.SetBool("move", true);
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
        Vector3 a = new Vector3(0, test, 0);
        animator.SetBool("move", true);
        agent.SetDestination(playercharacter.transform.position + a);
        transform.LookAt(playercharacter.transform.position);

    }
    private void attackPlayer()
    {
        animator.SetBool("move", false);
        animator.SetTrigger("attack");
        agent.SetDestination(transform.position);
        transform.LookAt(playercharacter.transform.position);
        if (!alreadyattack)
        {
            Debug.Log("run");
            alreadyattack = true;
            Collider[] players = Physics.OverlapSphere(attack.transform.position, attackRadius, playerlayer);
            for (int i = 0; i < players.Length; ++i)
            {
                
                players[i].GetComponent<playerMovement>().Takedamage(damage);
                players[i] = null;
            }
            Invoke(nameof(resetattack), timebetweenattacks);
        }
    }
    private void idle()
    {
        iding = true;
        animator.SetBool("move", false);

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
        walkpoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, ground))
        {
            walkPointset = true;
            iding = false;

        }

    }
    private void resetshell()
    {
        shell = false;
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
        view.RPC("takedamage2", RpcTarget.AllBuffered, x);
    }
    [PunRPC]
    private void takedamage2(float x)
    {
        storage += x;
        if (storage >= maxhealth / 20 && Random.Range(0, 10) >= 8)
        {
            storage = 0;
            shell = true;
            Invoke(nameof(resetshell), 5f);

        }
        else
        {
            storage = 0;
        }
        lookRadius = lookRadius * 5;
        Debug.Log(x);
        if (!shell)
        {
            health = health - x;
            view.RPC("sethealth", RpcTarget.AllBuffered);
        }
        if (health <= 0)
        {
            animator.SetTrigger("dead");
            audio.Play();
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
