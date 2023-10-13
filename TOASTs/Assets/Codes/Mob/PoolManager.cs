using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;

    public GameObject[] mobs; // 0: 근거리, 1: 원거리

    List<Queue<int>> mobPool = new List<Queue<int>>();

    //GameManager gameManager = GameManager.instance;
    PhotonView photonview;

    public static PoolManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PoolManager>();
            return instance;
        }
    }


    private void Start()
    {
        // instance = this;
        photonview = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < mobs.Length - 1; i++)
            {
                Instance.mobPool.Add(new Queue<int>());
                Pooling(i, 100);
            }
            Instance.mobPool.Add(new Queue<int>());
            Pooling(3, 1);
        }
        else
        {
            mobPool = Instance.mobPool;
        }
    }
        
    private void Pooling(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewMob(index);
        }
    }

    private GameObject CreateNewMob(int index)
    {
        //GameObject newDefaultMob = Instantiate(mobs[index], transform);
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        //Debug.Log(">>> mobs[index].gameObject.name : " + mobs[index].gameObject.name);
        //GameObject newDefaultMob = Instantiate(mobs[index], transform);
        
        GameObject newDefaultMob = PhotonNetwork.Instantiate(instance.mobs[index].gameObject.name, pos, rot);

        int pvID = newDefaultMob.GetComponent<PhotonView>().ViewID;
        //Debug.Log(">>> mob pvID : " + pvID);
        photonview.RPC("RPC_HideMob", RpcTarget.All, pvID); //newDefaultMob.gameObject.SetActive(false);
        
        Instance.mobPool[index].Enqueue(pvID);
        return newDefaultMob;
    } 

    public GameObject GetMob(int index)
    {
        if (!PhotonNetwork.IsMasterClient) return null;
        int pvID = 0;
        Debug.Log(index);
        //Debug.Log(">>> instance.mobPool[index].Count : " + instance.mobPool[index].Count);
        if (instance.mobPool[index].Count > 0)
        {
            // 이미 있는 거 가져오기
            pvID = instance.mobPool[index].Dequeue();
        }
        else
        {
            // 만들기
            Vector2 pos = Vector2.zero;
            Quaternion rot = Quaternion.identity;
            GameObject newDefaultMob = PhotonNetwork.Instantiate(instance.mobs[index].gameObject.name, pos, rot);
            pvID = newDefaultMob.GetComponent<PhotonView>().ViewID;
            photonview.RPC("RPC_HideMob", RpcTarget.All, pvID);
        }
        Instance.mobPool[index].Enqueue(pvID);
        photonview.RPC("RPC_ShowMob", RpcTarget.All, pvID);
        return PhotonView.Find(pvID).gameObject;
    }

    public void ReturnMob(GameObject mob)
    {
        Debug.Log("다시 들어감");
        int pvID = mob.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideMob", RpcTarget.All, pvID);
    }


    [PunRPC]
    void RPC_ShowMob(int pvID)
    {
        PhotonView.Find(pvID).gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_HideMob(int pvID)
    {
        PhotonView.Find(pvID).gameObject.SetActive(false);
    }
}
/*using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject[] mobs; // 0: 근거리, 1: 원거리, 2: 크로와상

    List<Queue<GameObject>> mobPool = new List<Queue<GameObject>>();

    GameManager gameManager = GameManager.instance;

    public enum MobType
    {
        Mouse = 0,
        Slime = 1,
        Croissant = 2,
        Boss = 3
    }

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < mobs.Length; i++)
        {
            mobPool.Add(new Queue<GameObject>());
            Pooling(i, 100);
        }
    }

    private void Pooling(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewMob(index);
        }
    }

    private GameObject CreateNewMob(int index)
    {
        GameObject newDefaultMob = Instantiate(mobs[index], transform);
        newDefaultMob.gameObject.SetActive(false);
        mobPool[index].Enqueue(newDefaultMob);
        return newDefaultMob;
    }

    public GameObject GetMob(int index)
    {
        GameObject newMob = null;
        if (instance.mobPool[index].Count > 0)
        {
            // 이미 있는 거 가져오기
            newMob = instance.mobPool[index].Dequeue();
            newMob.transform.SetParent(null);
            newMob.SetActive(true);
            //gameManager.currentPooledMobs.Add(newMob);
            //Debug.Log(gameManager.currentPooledMobs[index].ToString());
        }
        else
        {
            // 만들기
            newMob = Instantiate(mobs[index], transform);
            newMob.transform.SetParent(null);
            newMob.SetActive(false);
            //gameManager.currentPooledMobs.Add(newMob);
        }
        return newMob;
    }

    public void ReturnMob(GameObject mob)
    {

        mobPool[mob.GetComponent<TempDefaultMob>().MobIndex].Enqueue(mob);
        GameManager.instance.currentWave.Remove(mob);
        mob.SetActive(false);
    }
}
*/