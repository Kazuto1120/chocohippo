using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class riflescript : MonoBehaviour
{
    public float damage = 10;
    public float range = 100f;
    public float firerate = 30f;
    public float bullet = 50f;
    public float maxSpread = .5f; 
    public float spreadIncreasePerShot = 0.05f; 
    public float spreadResetDelay = 1f;
    public float gunnumber;

    public Camera camera;
    public ParticleSystem muzzleflash;
    public GameObject impact;
    public camerashake camerashake;
    public AudioSource sound;
    public playerMovement playerMovement;
    public Animator animator;


    public KeyCode shootkey = KeyCode.Mouse0;

    private float timetillfire = 0f;
    private float bulletremain = 0f;
    private bool abletofire = true;
    private float currentSpread = 0f;  
    private float timeSinceLastShot = 0f; 
    private void Start()
    {
        bulletremain = bullet;
    }

    private void FixedUpdate()
    {
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
        if (Input.GetKey(KeyCode.R)&&bulletremain < bullet)
        {
            reload();
        }
    }
    void shoot()
    {
        muzzleflash.Emit(1);
        sound.Play();
        StartCoroutine(camerashake.Shake(.2f,.1f));
        RaycastHit hit;
        Vector3 bulletDirection = CalculateBulletDirection();
        if (Physics.Raycast(camera.transform.position, bulletDirection, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Debug.DrawLine(camera.transform.position, hit.point, Color.red, 2.0f);
            Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
        }
        bulletremain--;
        currentSpread = Mathf.Min(currentSpread + spreadIncreasePerShot, maxSpread);
        timeSinceLastShot = Time.time;
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
