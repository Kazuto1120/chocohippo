using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class playerlook : MonoBehaviourPunCallbacks
{
    public float sensitivity = 50f;
    public Transform MainBody;

    private float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
       
        MainBody.Rotate(Vector3.up * mouseX);

    }
    public void Cameramove(float temp)
    {
        xRotation -= temp;
        SmoothCameraRotation();
    }
    private void SmoothCameraRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 5f * Time.deltaTime);
    }

}
