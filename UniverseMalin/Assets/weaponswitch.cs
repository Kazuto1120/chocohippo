using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class weaponswitch : MonoBehaviourPunCallbacks
{
    public int weaponSelected = 0;
    public GameObject scopeoverlay;
    public Camera maincamera;
    public Camera camera;
    

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        selectweapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { 
            return;}
            int previousweapon = weaponSelected;
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (weaponSelected >= transform.childCount - 1)
                {
                    weaponSelected = 0;
                }
                else
                {
                    weaponSelected++;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (weaponSelected <= 0)
                {
                    weaponSelected = transform.childCount - 1;
                }
                else
                {
                    weaponSelected--;
                }
            }
            if (previousweapon != weaponSelected)
            {
                selectweapon();
            if (photonView.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponSelected", weaponSelected);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }

            }
            if (weaponSelected == 2 && Input.GetButtonDown("Fire2"))
            {
                animator.SetBool("scoping", !animator.GetBool("scoping"));
                if (animator.GetBool("scoping"))
                {
                    StartCoroutine(onscope());
                }
                else
                {
                    unscope();
                }
            }
        
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine)
        {
            selectweapon1((float)changedProps["weaponSelected"]);
        }
    }
    void unscope()
    {
        camera.enabled = true;
        scopeoverlay.SetActive(false);
        maincamera.fieldOfView = 60f;
    }
    IEnumerator onscope()
    {
        yield return new WaitForSeconds(.25f);
        camera.enabled = false;
        scopeoverlay.SetActive(true);
        maincamera.fieldOfView = 15f;
    }
    void selectweapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == weaponSelected)
            {
                animator.SetBool("scoping", false);
                unscope();
                animator.SetFloat("gunnum",weapon.gameObject.GetComponent<riflescript>().gunnumber);
                animator.SetTrigger("switch");
                weapon.gameObject.SetActive(true);
                
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
    void selectweapon1( float x)
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == x)
            {
                animator.SetBool("scoping", false);
                unscope();
                animator.SetFloat("gunnum", weapon.gameObject.GetComponent<riflescript>().gunnumber);
                animator.SetTrigger("switch");
                weapon.gameObject.SetActive(true);

            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
