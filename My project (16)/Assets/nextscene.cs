//Lata Jorge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class nextscene : MonoBehaviour
{
    public string scenename;
    
    //this method is invoked when the prefab player collides with door
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
        {
            SceneManager.LoadScene(scenename);
        }
    }
}