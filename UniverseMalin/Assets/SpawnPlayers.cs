using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerPrefeb;

    public float minx = -17f;
    public float maxX = 0f;
    public float minZ = -8f;
    public float maxZ = 8f;

    private void Start()
    {
        Vector3 randomRosition = new Vector3(Random.Range(minx, maxX), 1f, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(PlayerPrefeb.name, randomRosition, Quaternion.identity);
    }
}
