using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerRnG : MonoBehaviour
{

    public GameObject player;
    public Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        player.transform.position = spawnPoint.position;
    }
}
