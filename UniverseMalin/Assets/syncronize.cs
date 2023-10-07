using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class syncronize : MonoBehaviourPunCallbacks
{
    public GameObject camera;
        
    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        transform.position = camera.GetComponent<Transform>().position;
        transform.rotation = camera.GetComponent<Transform>().rotation;
    }
}
