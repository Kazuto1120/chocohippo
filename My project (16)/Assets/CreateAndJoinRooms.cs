using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;

    public void CreateRoom()
    {
        Debug.Log("attempt to create");
        PhotonNetwork.CreateRoom(createInput.text);
    }
    public void joinRoom()
    {
        Debug.Log("attempt to join");
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("game");
    }
}
