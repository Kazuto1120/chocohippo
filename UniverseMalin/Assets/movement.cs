using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    private float speed;
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

    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode runkey = KeyCode.LeftShift;
    
    Vector3 velocity;



    private void Start()
    {
        player = gameObject.GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundedCheck.position, groundDistance, ground);
        if (isGrounded)
        {
            body.GetComponent<Animator>().SetBool("jump", false);
            air = false;
        }
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if(x != 0 || y != 0)
        {
            body.GetComponent<Animator>().SetBool("ismoving", true);
        }
        else
        {
            body.GetComponent<Animator>().SetBool("ismoving", false);
        }
        Vector3 move = transform.right * x + transform.forward * y;

        if (Input.GetKey(runkey))
        {
            speed = 2 * walkspeed;
            body.GetComponent<Animator>().SetBool("isrunning", true);
        }
        else
        {
            speed = walkspeed;
            body.GetComponent<Animator>().SetBool("isrunning", false);
        }
        player.Move(move * speed * Time.deltaTime);

        if (Input.GetKey(jumpkey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
            if (!air){
                body.GetComponent<Animator>().SetBool("jump", true);
                air = true;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        player.Move(velocity * Time.deltaTime);
    }
    public void Takedamage(int damage)
    {

    }
}
