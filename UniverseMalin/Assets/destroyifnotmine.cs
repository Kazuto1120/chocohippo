using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class destroyifnotmine : MonoBehaviourPunCallbacks
{
    public void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }

}
