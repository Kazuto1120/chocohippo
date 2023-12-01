using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisappearance : MonoBehaviour
{
    public float disappearanceTime = 10f; //time in seconds

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisappearAfterTime(disappearanceTime));
    }


    IEnumerator DisappearAfterTime(float time)
    {
yield return new WaitForSeconds(time);
        gameObject.SetActive(false); //disables the object at specific time


    }
    
}
