using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    public Image healthcircle;
    private float speed;
    public float Maxhealth = 100f;
    public float health;
    public CharacterController player;
    public GameObject body;
    public float gravity = -9.81f;
    public float jumpheight = 3f;
    public float walkspeed = 5f;
    public bool air = false;

    public Transform groundedCheck;
    public float groundDistance = 0.4f;
    public LayerMask ground;
    public bool isGrounded;
    public Animator animator;

    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode runkey = KeyCode.LeftShift;

    public float x, y;
    PhotonView view;

    Vector3 velocity;



    private void Start()
    {
        health = Maxhealth;
        view = GetComponent<PhotonView>();
        player = gameObject.GetComponent<CharacterController>();
        healthCircle();
    }
    private void FixedUpdate()
    {
        if (view.IsMine) { 

        isGrounded = Physics.CheckSphere(groundedCheck.position, groundDistance, ground);
        if (isGrounded)
        {

            air = false;
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Input.GetKey(runkey))
        {
            speed = 2 * walkspeed;
        }
        else
        {
            speed = walkspeed;
        }


        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        if ((x > 0 || y > 0) && Input.GetKey(runkey))
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", true);

        }
        else if (x > 0 || y > 0)
        {
            animator.SetBool("run", false);
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
        }

        Vector3 move = transform.right * x + transform.forward * y;

        player.Move(move * speed * Time.deltaTime);

        if (Input.GetKey(jumpkey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
            if (!air)
            {

                air = true;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        player.Move(velocity * Time.deltaTime);
    }
    }
    public void reloadplay()
    {
        Debug.Log(" is played");
        animator.SetBool("reload", true);
        StartCoroutine(ResetReloadParameter(1f));

    }
    private IEnumerator ResetReloadParameter(float delay)
    {
        yield return new WaitForSeconds(delay);

        // After the specified delay, reset the "reload" parameter to false.
        animator.SetBool("reload", false);
    }
    void healthCircle()
    {
        healthcircle.fillAmount = health / Maxhealth; 
    }


    public void Takedamage(int damage)
    {
        health = health - damage;
        healthCircle();
        if (health <= 0)
        {
            Destroy(gameObject);
        }

    }
}
