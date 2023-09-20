using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class shootingsystem : MonoBehaviourPunCallbacks
{
    public AudioSource src;
    public int damage;
    public float spread, range, timebetweenshots, tempspread, spreadincrement, spreadResetTime,spreadResetRemain, cameraRotationAngle;
    public int fuel = 100;
    public bool rapid , readytoshoot;
    int fuelleft;

    public Camera camera;
    public Transform attactpoint;
    public RaycastHit rayhit;
    public LayerMask enemy;
    public PhotonView views;


    public GameObject muzzleFlash, buttethole;
    private void Awake()
    {
        fuelleft = fuel;
        readytoshoot = true;
        tempspread = spread;
        spreadResetRemain = 0;
        
    }



    private void FixedUpdate()
    {
        if (views.IsMine)
        {
            if (readytoshoot == true && fuelleft > 0 && Input.GetKey(KeyCode.Mouse0))
            {
                shoot();
                if (tempspread < spread * 50)
                    tempspread += spreadincrement;
            }
            if (spreadResetRemain > 0f)
            {
                spreadResetRemain -= Time.deltaTime;


            }
            else
                tempspread = spread;
        }
    }


    private void shoot()
    {
        readytoshoot = false;

        float x = Random.Range(-tempspread, tempspread);
        float y = Random.Range(-tempspread, tempspread);

        Vector3 direction = Quaternion.Euler(x, y, 0f) * camera.transform.forward;

        if (Physics.Raycast(camera.transform.position, direction, out rayhit, range, enemy))
        {
            Debug.Log(rayhit.collider.name);
            if (rayhit.collider.CompareTag("enemy"))
            {
                rayhit.collider.GetComponent<movement>().Takedamage(damage);
            }
        }
        spreadResetRemain = spreadResetTime;
        src.Play();
        Instantiate(buttethole, rayhit.point, Quaternion.FromToRotation(Vector3.forward, rayhit.normal));
        RotateCameraUp();
        Instantiate(muzzleFlash, attactpoint.position, Quaternion.LookRotation(camera.transform.forward));
        fuelleft--;
        Invoke("resetshot", timebetweenshots);
    }
    private void resetshot()
    {
        readytoshoot = true;
    }
    private void RotateCameraUp()
    {
        camera.GetComponent<playerlook>().Cameramove(-(cameraRotationAngle/2));
        camera.GetComponent<playerlook>().Cameramove(cameraRotationAngle);
    }


}
