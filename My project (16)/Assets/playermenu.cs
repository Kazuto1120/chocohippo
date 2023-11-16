using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class playermenu : MonoBehaviour
{
    public camerascript player;
    public float sensitivityMax = 2000f;
    public Slider Slider;
    public GameObject canvas;
    public bool incanvas;
    public PhotonView view;
    public KeyCode escape = KeyCode.Escape;
    void Start()
    {
        if (view.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Slider.maxValue = sensitivityMax;
            Slider.value = player.sensitivity;
            canvas.SetActive(false);
            incanvas = false;
        }
    }
    private void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(escape))
                switchmenu();
        }
    }
    public void switchmenu()
    {

        if (incanvas == false)
        {

            incanvas = true;
            Cursor.lockState = CursorLockMode.None;
            canvas.SetActive(true);

        }
        else if (incanvas)
        {

            incanvas = false;
            Cursor.lockState = CursorLockMode.Locked;
            canvas.SetActive(false);

        }

    }
    public void back()
    {
        Invoke(nameof(back2), 1f);
    }
    private void back2()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            Cursor.lockState = CursorLockMode.None;
        }
        SceneManager.LoadScene("lobby");
    }
    public void add(float x)
    {
        player.sensitivity += x;
        if (player.sensitivity > sensitivityMax)
            player.sensitivity = sensitivityMax;
        sliderupdate();
    }
    private void sliderupdate()
    {
        Slider.value = player.sensitivity;
    }
}
