using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class testmovement : MonoBehaviour
{
    private float moveSpeed;
    public float speed;
    public PhotonView view;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;



    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public float playerHeight;
    public LayerMask whatIsGround,wall;
    bool grounded,dead;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        if (view.IsMine)
        {
            moveSpeed = speed;
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            readyToJump = true;
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
            dead = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, wall);
            if (dead)
            {
                Destroyobject();
            }
            MyInput();
            SpeedControl();


        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (Input.GetKey(sprintKey) && grounded)
        {
            moveSpeed = speed * 2;
        }
        else
        {
            moveSpeed = speed;
        }
    }

    private void MovePlayer()
    {

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);


        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);


        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void Destroyobject()
    {
        SceneManager.LoadScene("gameover");
        Destroy(gameObject);
    }
}
