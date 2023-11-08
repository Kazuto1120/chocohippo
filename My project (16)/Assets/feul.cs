using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class feul : MonoBehaviour
{
    private bool activate = true;
    public float time = 10f;
    public GameObject image;
    public PhotonView view;
    public Collider another;
    private void OnTriggerEnter(Collider other)
    {
        if (activate && other.CompareTag("Player"))
        {
            Debug.Log("player");
            another = other;
            view.RPC("Trigger", RpcTarget.AllBuffered);
        }
    }
    private void Reset()
    {
        view.RPC("Reset2", RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void Trigger()
    {
        Debug.Log("player2");
        activate = false;
        image.SetActive(false);
        another.GetComponentInChildren<weaponswitch>().addreload(1);
        Invoke(nameof(Reset), time);
    }
    [PunRPC]
    private void Reset2()
    {
        activate = true;
        image.SetActive(true);
    }
}
