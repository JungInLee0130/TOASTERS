using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public int meleeMobNum;
    public int rangedMobNum;

    private Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.Find("MobSpawnPointGroup").GetComponentsInChildren<Transform>();

        CreateMeleeMob(meleeMobNum);
        CreateRangedMob(rangedMobNum);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateMeleeMob(int monNum)
    {
        Debug.Log(">>> meleeMob " + monNum + "原軒 持失");
        for (int i = 0; i < monNum; i++)
        {
            int idx = Random.Range(1, spawnPoints.Length);
            Vector2 pos = spawnPoints[idx].position;
            Quaternion rot = spawnPoints[idx].rotation;

            GameObject playerTemp = PhotonNetwork.Instantiate("MeleeMob", pos, rot, 0); //
            PhotonView photonView = playerTemp.GetComponent<PhotonView>();
        }
    }
    
    void CreateRangedMob(int monNum)
    {
        Debug.Log(">>> RangedMob " + monNum + "原軒 持失");
        for (int i = 0; i < monNum; i++)
        {
            int idx = Random.Range(1, spawnPoints.Length);
            Vector2 pos = spawnPoints[idx].position;
            Quaternion rot = spawnPoints[idx].rotation;

            GameObject playerTemp = PhotonNetwork.Instantiate("RangedMob", pos, rot, 0); // 
            PhotonView photonView = playerTemp.GetComponent<PhotonView>();
        }
    }

}
