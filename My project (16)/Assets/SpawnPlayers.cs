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
    public float y;
    public int amount = 5;

    private void Start()
    {
        Vector3 randomRosition = new Vector3(Random.Range(minx, maxX), y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(PlayerPrefeb.name, randomRosition, Quaternion.identity);
        for(int i = 0; i < amount; ++i)
        {
            health();
            ammo();
        } 
    }
    private void health()
    {
        Vector3 randomRosition = new Vector3(Random.Range(minx, maxX), y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate("defaultpill", randomRosition, Quaternion.identity);
    }
    private void ammo()
    {
        Vector3 randomRosition = new Vector3(Random.Range(minx, maxX), y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate("energy cell", randomRosition, Quaternion.identity);
    }
}
