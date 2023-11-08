using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class weaponswitch : MonoBehaviourPunCallbacks
{
    public int weaponSelected = 0;
    public GameObject scopeoverlay;
    public Camera maincamera;
    public Camera camera;
    public PhotonView view;
    public float reloadleft = 3;
    public Text text;

    

    public Animator animator;
    // Start is called before the first frame update
    private void Start()
    {
        reloadupdate();
        view.RPC("selectweapon", RpcTarget.AllBuffered, weaponSelected);
    }

    // Update is called once per frame
         void Update()
    {
        if (!photonView.IsMine) { 
            return;}
        if (!transform.GetChild(weaponSelected).GetComponent<riflescript>().abletofire)
        {
            unscope();
            animator.SetBool("scoping", false);
            return;
        }
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
            view.RPC("selectweapon", RpcTarget.AllBuffered, weaponSelected);
            

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
    public void reloadupdate()
    {
        text.text = reloadleft + "";
    }

    void unscope()
    {
        if (view.IsMine)
        {
            camera.enabled = true;
            scopeoverlay.SetActive(false);
            maincamera.fieldOfView = 60f;
        }
    }
    IEnumerator onscope()
    {
        yield return new WaitForSeconds(.25f);
        camera.enabled = false;
        scopeoverlay.SetActive(true);
        maincamera.fieldOfView = 15f;
    }
    [PunRPC]
    void selectweapon(int x)
    {
        Debug.Log("select function called");
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
