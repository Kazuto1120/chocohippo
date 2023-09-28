using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chasingPlayer : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public float chase_speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    //Fixed update fucntion

    void Update()
    {
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, chase_speed);
    }
}
