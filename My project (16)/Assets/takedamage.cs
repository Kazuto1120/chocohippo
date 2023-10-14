using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takedamage : MonoBehaviour
{
    public GameObject game;
    public void Takedamage(int x)
    {
        game.GetComponent<playerMovement>().Takedamage(x);
    }
}
