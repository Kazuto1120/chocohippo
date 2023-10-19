using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class minionspawn : MonoBehaviour
{
    public GameObject minion;
   public void Spawn()
    {
        PhotonNetwork.Instantiate(minion.name, transform.position, Quaternion.identity);
    }
}
