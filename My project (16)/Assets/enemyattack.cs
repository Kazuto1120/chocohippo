using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class enemyattack : MonoBehaviour
{
    public GameObject bullet;
    public GameObject minion;

    public float shootforce, upwardforce;

    public int bulletpershot = 1;

    public void shoot(Collider x)
    {
        for (int i = 0; i<bulletpershot;i++) {
            Vector3 direction = x.transform.position - transform.position;

            GameObject currentBullet = Instantiate(bullet, transform.position, Quaternion.identity);

            currentBullet.transform.forward = direction.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootforce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(transform.up * upwardforce, ForceMode.Impulse); }
    }
    public void shoot2(Collider x)
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 direction = x.transform.position - transform.position;

            GameObject currentBullet = PhotonNetwork.Instantiate(minion.name, transform.position, Quaternion.identity); ;

            currentBullet.transform.forward = direction.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootforce, ForceMode.Impulse);
            currentBullet.GetComponent<Rigidbody>().AddForce(transform.up * upwardforce, ForceMode.Impulse);
        }
    }
}
