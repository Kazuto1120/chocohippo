using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class restart : MonoBehaviourPunCallbacks
{
    public string name;
    public AudioSource audio;
    public Animator animator;
    private void Awake()
    {
        PhotonNetwork.LeaveRoom();
        Cursor.lockState = CursorLockMode.None;
    }
    public void restartGame()
    {
        Debug.Log("test");
        audio.Play();
        animator.SetTrigger("start");
        Invoke(nameof(Reset2),1f);
        
    }
    private void Reset2()
    {
        SceneManager.LoadScene(name);
    }
}
