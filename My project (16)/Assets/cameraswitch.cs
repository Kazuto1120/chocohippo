using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraswitch : MonoBehaviour
{
    public GameObject enemyHolder;
    public Camera[] cameras;
    private int index = 0;
    private bool switchEnable = false;
    public float time = 2f;
    void Start()
    {
        cameras = enemyHolder.GetComponentsInChildren<Camera>();
        disableAllCamera();
        EnableCamera(index);
        StartCoroutine(ResetSwitch(time));
    }

    void Update()
    {
        if (switchEnable)
        {
            Switch();
            switchEnable = false;
            StartCoroutine(ResetSwitch(Random.Range(time,time*2)));
        }
    }
    IEnumerator ResetSwitch(float time)
    {
        yield return new WaitForSeconds(time);
        switchEnable = true;
    }
    private void Switch()
    {
        
        disableCamera(index);
        index = (index + 1) % cameras.Length;
        EnableCamera(index);
    }
    private void disableAllCamera()
    {
        for(int i = 0; i < cameras.Length; ++i)
        {
            cameras[i].enabled = false;
        }
    }
    private void EnableCamera(int x)
    {
        cameras[x].enabled = true;
    }
    private void disableCamera(int x)
    {
        cameras[x].enabled = false;
    }
}
