using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class enemeysystem : MonoBehaviour
{
    public Animator animator;
    public PhotonView view;
    public float health = 100;
    public Slider slider;

    void Start()
    {
        view = GetComponent<PhotonView>();
        slider.maxValue = health;
        view.RPC("sethealth", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void takedamage(float x)
    {
        view.RPC("takedamage2", RpcTarget.AllBuffered, x);
    }
    [PunRPC]
    private void takedamage2(float x)
    {
        Debug.Log(x);
        health = health - x;
        view.RPC("sethealth",RpcTarget.AllBuffered);
        if (health <= 0)
        {
            animator.SetTrigger("dead");
            StartCoroutine(DestroyAfterDelay(2f));
        }
    }
    [PunRPC]
    private void sethealth()
    {
        slider.value = health;
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }
}
