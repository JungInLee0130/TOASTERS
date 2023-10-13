using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public override void OnConnectedToMaster()
	{
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
	}

	public override void OnJoinedRoom()
	{
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
	}
}
