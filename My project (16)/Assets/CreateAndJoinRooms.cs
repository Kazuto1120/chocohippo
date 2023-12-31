using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;



public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public Animator animator;
    public AudioSource audio;
    public string name;

    public void CreateRoom()
    {
        Debug.Log("attempt to create");
        StartCoroutine(Load());
    }
    public void joinRoom()
    {
        Debug.Log("attempt to join");
        StartCoroutine(Load2());
    }
    public void gototutorial()
    {
        audio.Play();
        animator.SetTrigger("start");
        Invoke(nameof(goto2), 1f);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(name);
    }
    IEnumerator Load()
    {
        audio.Play();
        animator.SetTrigger("start");

        yield return new WaitForSeconds(1);

        PhotonNetwork.CreateRoom(createInput.text);
    }
    IEnumerator Load2()
    {
        audio.Play();
        animator.SetTrigger("start");

        yield return new WaitForSeconds(1);

        PhotonNetwork.JoinRoom(joinInput.text);
        PhotonNetwork.CreateRoom(joinInput.text);

    }
    private void goto2()
    {
        SceneManager.LoadScene("Level1");
    }


}
