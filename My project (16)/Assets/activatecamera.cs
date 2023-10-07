using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class activatecamera : MonoBehaviourPunCallbacks
{
    public Camera camera;
    private void Start()
    {
        if (photonView.IsMine)
        {
            camera.enabled = true;
        }
    }
}
