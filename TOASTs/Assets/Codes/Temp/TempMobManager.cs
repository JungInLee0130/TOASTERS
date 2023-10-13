using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TempMobManager : MonoBehaviour
{
    public int meleeMobNum;
    public int rangedMobNum;

    private Transform[] spawnPoints;

    public GameObject meleeMobs;
    public GameObject rangedMobs;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.Find("MobSpawnPointGroup").GetComponentsInChildren<Transform>();

        CreateMeleeMob(meleeMobNum);
        CreateRangedMob(rangedMobNum);
    }

    void CreateMeleeMob(int monNum)
    {
        Debug.Log(">>> meleeMob " + monNum + "마리 생성");
        for (int i = 0; i < monNum; i++)
        {
            int idx = Random.Range(1, spawnPoints.Length);
            Vector2 pos = spawnPoints[idx].position;
            Quaternion rot = spawnPoints[idx].rotation;

            Instantiate(meleeMobs, pos, rot);
            //GameObject playerTemp = PhotonNetwork.Instantiate("MeleeMob", pos, rot, 0); //
            //PhotonView photonView = playerTemp.GetComponent<PhotonView>();
        }
    }
    
    void CreateRangedMob(int monNum)
    {
        Debug.Log(">>> RangedMob " + monNum + "마리 생성");
        for (int i = 0; i < monNum; i++)
        {
            int idx = Random.Range(1, spawnPoints.Length);
            Vector2 pos = spawnPoints[idx].position;
            Quaternion rot = spawnPoints[idx].rotation;

            Instantiate(rangedMobs, pos, rot);
            //GameObject playerTemp = PhotonNetwork.Instantiate("RangedMob", pos, rot, 0); // 
            //PhotonView photonView = playerTemp.GetComponent<PhotonView>();
        }
    }

}
