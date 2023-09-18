using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public CharacterController player;

    private void Start()
    {
        player = gameObject.GetComponent<CharacterController>();
    }
    private void Update()
    {
        
    }
}
