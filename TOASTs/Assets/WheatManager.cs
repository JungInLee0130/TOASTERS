using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

public class WheatManager : MonoBehaviour
{
    public GameObject[] Points;

    public GameObject[] wheats;
    int wheatsLen;

    PhotonView photonview;

    void Start()
    {
        wheatsLen = wheats.Length;
        ActivateWheats();
        photonview = GetComponent<PhotonView>();
    }

    void ActivateWheats()
    {
        for (int i = 0; i < wheatsLen; i++)
        {
            wheats[i].SetActive(true);
        }
    }

    Queue<int> WheatQueue = new Queue<int>();

    [PunRPC]
    void RPC_HideWheat(int pvID)
    {
        Debug.Log(">>> RPC_HideWheat called!!!");
        PhotonView.Find(pvID).gameObject.SetActive(false);
        WheatQueue.Enqueue(pvID);
        // Invoke("ShowWheat", 3.0f);
    }
    /*
        void ShowWheat()
        {
            int pvID = WheatQueue.Dequeue();
            photonview.RPC("RPC_ShowWheat", RpcTarget.All, pvID);
        }

        [PunRPC]
        void RPC_ShowWheat(int pvID)
        {
            PhotonView.Find(pvID).gameObject.SetActive(true);
        }
    */

    [PunRPC]
    void RPC_UpdateWheat(int wheat)
    {
        // Debug.Log(">>> Wheat update to " + wheat);
        GameManager.instance.wheat = wheat;
    }

/*    [PunRPC]
    void RPC_GainGold(int gold)
    {
        GameManager.instance.gold += gold;
    }*/

    [PunRPC]
    void RPC_UpdateGold(int gold)
    {
        //GameManager.instance.gold = gold;
        Debug.Log("goooooooooooold   " + gold);
        GameManager.instance.UpdateGold(gold);
    }

    [PunRPC]
    void RPC_DestroyBread(string point)
    {
        //Debug.Log(">>> point : " + point);
        if (point == "Point")
        {
            Destroy(Points[0].transform.GetChild(0).gameObject);
        }
        else if(point == "Point (1)")
        {
            Destroy(Points[1].transform.GetChild(0).gameObject);
        }
        else if(point == "Point (2)")
        {
            Destroy(Points[2].transform.GetChild(0).gameObject);
        }
        else if(point == "Point (3)")
        {
            Destroy(Points[3].transform.GetChild(0).gameObject);
        }
        else if(point == "Point (4)")
        {
            Destroy(Points[4].transform.GetChild(0).gameObject);
        }
        //Destroy();
    }


    /*[PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    void RPC_CurrentWaveRemove()
    {
        GameManager.instance.currentWave.Remove(gameObject);
    }*/
}