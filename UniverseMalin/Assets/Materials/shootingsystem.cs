using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingsystem : MonoBehaviour
{
    public int damage;
    public float spread, range, timebetweenshots;
    public int fuel = 100;
    public bool rapid , readytoshoot;
    int fuelleft;

    public Camera camera;
    public Transform attactpoint;
    public RaycastHit rayhit;
    public LayerMask enemy;

    public GameObject muzzleFlash, buttethole;
    private void Awake()
    {
        fuelleft = fuel;
        readytoshoot = true;
    }
    private void Update()
    {
        if (readytoshoot == true &&fuelleft>0&& Input.GetKey(KeyCode.Mouse0))
        {
            shoot();
        }
    }

    private void shoot()
    {
        readytoshoot = false;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = camera.transform.forward + new Vector3(x, y, 0);
        
        if (Physics.Raycast(camera.transform.position, direction, out rayhit, range, enemy))
        {
            Debug.Log(rayhit.collider.name);
            if (rayhit.collider.CompareTag("enemy"))
            {
                rayhit.collider.GetComponent<movement>().Takedamage(damage);
            }
        }
        Instantiate(buttethole, rayhit.point, Quaternion.FromToRotation(Vector3.forward, rayhit.normal));
        Instantiate(muzzleFlash, attactpoint.position, Quaternion.identity);
        fuelleft--;
        Invoke("resetshot", timebetweenshots);
    }
    private void resetshot()
    {
        readytoshoot = true;
    }

}
