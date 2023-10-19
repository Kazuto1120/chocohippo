using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class minion : MonoBehaviour
{
    public float lookRadius, attackRadius;
    bool playerInsightRange, playerinattackrange;
    Collider playercharacter;
    public int damage = 100;
    public float bounceForce = 2;

    public PhotonView view;
    public float health = 100;
    public Slider slider;

    public NavMeshAgent agent;
    public LayerMask ground, playerlayer;
    public GameObject attack;
    public Animator animator;

    public Vector3 walkpoint;
    bool walkPointset;
    public float walkrange;
    // Start is called before the first frame update
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        view = GetComponent<PhotonView>();
        slider.maxValue = health;
        view.RPC("sethealth", RpcTarget.AllBuffered);
    }

    
    void Update()
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

        if (!walkPointset) Searchwalkpoint();
        if (walkPointset) agent.SetDestination(walkpoint);
        
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

    }
    private void Searchwalkpoint()
    {

        float randomz = Random.Range(-walkrange, walkrange);
        float randomx = Random.Range(-walkrange, walkrange);
        walkpoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, ground))
        {
            walkPointset = true;
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
        Debug.Log(x);
        health = health - x;
        view.RPC("sethealth", RpcTarget.AllBuffered);
        if (health <= 0)
        {
            animator.SetTrigger("dead");
            StartCoroutine(DestroyAfterDelay(0f));
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

}

