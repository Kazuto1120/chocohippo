using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class riflescript : MonoBehaviourPunCallbacks
{
    public float damage = 10;
    public float range = 100f;
    public float firerate = 30f;
    public float bullet = 50f;
    public float maxSpread = .5f; 
    public float spreadIncreasePerShot = 0.05f; 
    public float spreadResetDelay = 1f;
    public float gunnumber;
    public float bulletpershot = 1f;
    public float camerashakeMagnitude = 0;
    public float camerashakeDuration = 0;

    public Camera camera;
    public ParticleSystem muzzleflash;
    public GameObject impact;
    public camerashake camerashake;
    public AudioSource sound;
    public playerMovement playerMovement;
    public Animator animator;
    public Image ammobar;


    public KeyCode shootkey = KeyCode.Mouse0;

    private float timetillfire = 0f;
    private float bulletremain = 0f;
    public bool abletofire = true;
    private float currentSpread = 0f;  
    private float timeSinceLastShot = 0f; 
    private void Start()
    {
        
        bulletremain = bullet;
        
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {

            ammocircle();

            if (Time.time - timeSinceLastShot > spreadResetDelay)
            {
                ResetSpread();
            }
            if (Input.GetKey(shootkey) && Time.time >= timetillfire && bulletremain > 0 && abletofire)
            {
                animator.SetBool("fire", true);
                timetillfire = Time.time + 1 / firerate;
                shoot();
            }
            else
            {
                animator.SetBool("fire", false);
            }
            if (Input.GetKey(KeyCode.R) && bulletremain < bullet)
            {
                reload();
            }

        }
    }
    void shoot()
    {
        
        muzzleflash.Emit(1);
        sound.Play();
        StartCoroutine(camerashake.Shake(camerashakeDuration,camerashakeMagnitude));
        RaycastHit hit;
        for (float i = 0; i < bulletpershot; ++i)
        {
            Vector3 bulletDirection = CalculateBulletDirection();
            if (Physics.Raycast(camera.transform.position, bulletDirection, out hit, range))
            {
                Debug.Log("the name of the target hit:"+hit.transform.name);
                
                enemymovement enemey = hit.transform.GetComponent<enemymovement>();
                if (enemey != null)
                {
                    enemey.takedamage(damage);
                }
                boss2movement boss2 = hit.transform.GetComponent<boss2movement>();
                if (boss2 != null)
                {
                    boss2.takedamage(damage);
                }
                minion temp = hit.transform.GetComponent<minion>();
                if (temp != null)
                {
                    temp.takedamage(damage);
                }
                PhotonNetwork.Instantiate(impact.name, hit.point, Quaternion.LookRotation(hit.normal));
            }
            bulletremain--;
            currentSpread = Mathf.Min(currentSpread + spreadIncreasePerShot, maxSpread);
            timeSinceLastShot = Time.time;
        }
    }
    private void reload()
    {
        abletofire = false;
        bulletremain = bullet;
        animator.SetBool("reload", true);
        StartCoroutine(ResetReloadParameter(1f));
        StartCoroutine(abletofired(2f));
        
    }
    void ResetSpread()
    {
        currentSpread = 0f;
    }
    private void ammocircle()
    {
        ammobar.fillAmount = bulletremain / bullet;
    }
    Vector3 CalculateBulletDirection()
    {
        float randomSpreadX = Random.Range(-currentSpread, currentSpread);
        float randomSpreadY = Random.Range(-currentSpread, currentSpread);

        Vector3 bulletDirection = camera.transform.forward;
        bulletDirection.x += randomSpreadX;
        bulletDirection.y += randomSpreadY;

        return bulletDirection.normalized;
    }
    private IEnumerator ResetReloadParameter(float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetBool("reload", false);
    }
    private IEnumerator abletofired(float delay)
    {
        yield return new WaitForSeconds(delay);

        abletofire = true;
    }

}
