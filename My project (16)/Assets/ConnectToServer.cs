using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public Animator animator;
    public AudioSource audio;
    private bool finish = false;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connecting to server");
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("loading lobby ");

        finish = true;
    }
    public void test()
    {
        if(finish)
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        audio.Play();
        animator.SetTrigger("start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Lobby");
    }

}
