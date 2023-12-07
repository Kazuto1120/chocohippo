using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class final : MonoBehaviour
{
    public string name;
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(name);
        Destroy(other.gameObject);
    }
}
