using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class multiplayerdisable : MonoBehaviourPunCallbacks
{
    public GameObject movement;
    public GameObject cameramain;
    public GameObject cameraweapon;
    public GameObject weaponscript;
    public GameObject canvas;
    // Start is called before the first frame update
    private void Start()
    {
        if (!photonView.IsMine)
        {
            cameramain.GetComponent<Camera>().enabled = false;
            cameramain.GetComponent<camerascript>().enabled = false;
            cameramain.GetComponent<camerashake>().enabled = false;
            cameraweapon.GetComponent<Camera>().enabled = false;
            canvas.gameObject.SetActive(false);
        }
    }


}
