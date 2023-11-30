using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class nextmap : MonoBehaviour
{
    public string name;
    public LayerMask playerlayer;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other.name);
        
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, 100000, playerlayer);
        for (int i = 0; i < playerCollider.Length; ++i)
        {
            playerCollider[i].GetComponent<playerMovement>().newD();
        }
        
        PhotonNetwork.LoadLevel(name);

    }
}
