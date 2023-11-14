using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class camerafreemovement : MonoBehaviour
{
    public float sensitivity = 500f;
    public float sensitivityMax = 2000f;
    public Slider Slider;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float x, y;
    private float mouseX, mouseY;
    public float speed =1f;
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode downkey = KeyCode.LeftShift;
    public KeyCode escape = KeyCode.Escape;
    public GameObject canvas;
    public bool incanvas;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Slider.maxValue = sensitivityMax;
        Slider.value = sensitivity;
        canvas.SetActive(false);
        incanvas = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (!incanvas) 
        { move();}
        if (Input.GetKey(jumpkey))
        {
            transform.position += Vector3.up * speed*Time.deltaTime;
        }
        if (Input.GetKey(downkey))
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(escape))
        switchmenu();
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
    public void move()
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Vector3 move = transform.right * x + transform.forward * y;
        transform.position += move * Time.deltaTime * speed;
    }
    public void back()
    {
        SceneManager.LoadScene("lobby");
    }
}
