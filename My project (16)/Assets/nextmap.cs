using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class nextmap : MonoBehaviourPunCallbacks
{
    public string name;
    public LayerMask playerlayer;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(name);
        }
    }
}
