using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class camerafreemovement : MonoBehaviour
{
    public float sensitivity = 500f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public float x, y;
    public float speed =1f;
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode downkey = KeyCode.LeftShift;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * y;
        transform.position += move*Time.deltaTime*speed;
        if (Input.GetKey(jumpkey))
        {
            transform.position += Vector3.up * speed*Time.deltaTime;
        }
        if (Input.GetKey(downkey))
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
    }
}
