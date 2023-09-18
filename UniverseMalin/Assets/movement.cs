using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    public CharacterController player;
    public GameObject body;
    private bool ismoving = false;


    private void Start()
    {
        player = gameObject.GetComponent<CharacterController>();
    }
    private void Update()
    {
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

        player.Move(move * speed * Time.deltaTime);
    }
}
