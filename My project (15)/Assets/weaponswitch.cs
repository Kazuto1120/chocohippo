using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponswitch : MonoBehaviour
{
    public int weaponSelected = 0;
    // Start is called before the first frame update
    void Start()
    {
        selectweapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousweapon = weaponSelected;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (weaponSelected >= transform.childCount - 1)
            {
                weaponSelected = 0;
            }
            else
            {
                weaponSelected++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (weaponSelected <= 0)
            {
                weaponSelected = transform.childCount - 1;
            }
            else
            {
                weaponSelected--;
            }
        }
        if (previousweapon != weaponSelected)
        {
            selectweapon();
        }
    }
    void selectweapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == weaponSelected)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
