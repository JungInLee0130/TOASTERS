using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class BulletPoolManagerReal : MonoBehaviour
{
    #region Fields
    private static BulletPoolManagerReal instance;

    Queue<int> bulletPool = new Queue<int>(); //Bullet
    Queue<int> slimeBulletPool = new Queue<int>(); //MobBullet
    Queue<int> croissantBombPool = new Queue<int>(); //MobCroissantBomb
    Queue<int> jellyfishBulletPool = new Queue<int>(); //MobJellyfishBullet

    PhotonView photonview;

    #endregion

    #region Methods

    private void Start()
    {
        photonview = GetComponent<PhotonView>(); 

        // 플레이어 탄환
        Pooling(100);

        // 몹 탄환
        //if (PhotonNetwork.IsMasterClient)
        {
            Scene scene = SceneManager.GetActiveScene();
            if(scene.name == "GameScene")
            {
                MobPooling(30);
                CroissantBombPooling(30);
                //MobJellyfishBulletPooling(10);
            }
        }
    }

    public static BulletPoolManagerReal Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<BulletPoolManagerReal>();
            return instance;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void Pooling(int count)
    {
        for (int i = 0; i < count; i++) 
        {
            bulletPool.Enqueue(CreateNewBullet());
        }
    }

    private void MobPooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            slimeBulletPool.Enqueue(CreateNewMobBullet());
        }
    }

    private void CroissantBombPooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            croissantBombPool.Enqueue(CreateNewCroissantBomb());
        }
    }

    private void MobJellyfishBulletPooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            jellyfishBulletPool.Enqueue(CreateNewJellyfishBullet());
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 새로운 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private int CreateNewBullet()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        Bullet newBullet = PhotonNetwork.Instantiate("Bullet", pos, rot).GetComponent<Bullet>();
        int pvID = newBullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(false);
        return pvID;

    }

    /// <summary>
    /// 새로운 몬스터 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private int CreateNewMobBullet()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        MobBullet newBullet = PhotonNetwork.Instantiate("EnemyBullet", pos, rot).GetComponent<MobBullet>();
        int pvID = newBullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID);//newBullet.gameObject.SetActive(false);
        return pvID;
    }

    /// <summary>
    /// 새로운 폭탄 생성하는 메소드
    /// Photon 연결하면 반환값 Temp 떼면 된다
    /// </summary>
    /// <returns>풀링할 새로운 폭탄</returns>
    private int CreateNewCroissantBomb()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        MobCroissantBomb newBomb = PhotonNetwork.Instantiate("EnemyBomb", pos, rot).GetComponent<MobCroissantBomb>();
        int pvID = newBomb.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //newBomb.gameObject.SetActive(false);
        return pvID;
    }

    private int CreateNewJellyfishBullet()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        MobJellyfishBullet newBullet = PhotonNetwork.Instantiate("JellyfishBullet", pos, rot).GetComponent<MobJellyfishBullet>();
        int pvID = newBullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //newBomb.gameObject.SetActive(false);
        return pvID;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public Bullet GetBullet()
    {
        int pvID = 0;
        if (Instance.bulletPool.Count > 0)
        {
            pvID = Instance.bulletPool.Dequeue();
        }
        else
        {
            pvID = CreateNewBullet();
        }
        EnqueueBullet(pvID);
        photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(true);
        return PhotonView.Find(pvID).gameObject.GetComponent<Bullet>();
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public MobBullet GetMobBullet()
    {
        //if (!PhotonNetwork.IsMasterClient) return null;
        int pvID = 0;
        if (instance.slimeBulletPool.Count > 0)
        {
            pvID = Instance.slimeBulletPool.Dequeue();
        }
        else
        {
            pvID = CreateNewMobBullet();
        }
        EnqueueMobBullet(pvID);
        photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(true);
        return PhotonView.Find(pvID).gameObject.GetComponent<MobBullet>();
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public MobCroissantBomb GetCroissantBomb()
    {
        int pvID = 0;
        if (instance.croissantBombPool.Count > 0)
        {
            pvID = Instance.croissantBombPool.Dequeue();
        }
        else
        {
            pvID = CreateNewCroissantBomb();
        }
        EnqueueCroissantBomb(pvID);
        photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBomb.gameObject.SetActive(true);
        return PhotonView.Find(pvID).gameObject.GetComponent<MobCroissantBomb>();
    }

    public MobJellyfishBullet GetJellyfishBullet()
    {
        int pvID = 0;
        if (instance.jellyfishBulletPool.Count > 0)
        {
            pvID = Instance.jellyfishBulletPool.Dequeue();
        }
        else
        {
            pvID = CreateNewJellyfishBullet();
        }
        EnqueueJellyfishBullet(pvID);
        photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBomb.gameObject.SetActive(true);
        return PhotonView.Find(pvID).gameObject.GetComponent<MobJellyfishBullet>();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(Bullet bullet)
    {
        //Debug.Log(">>> ReturnBullet 불림!!!");
        int pvID = bullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //bullet.gameObject.SetActive(false);
    }

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnMobBullet(MobBullet bullet) 
    {
        int pvID = bullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID);
    }

    public void ReturnCroissantBomb(MobCroissantBomb bomb)
    {
        int pvID = bomb.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID);
    }

    public void ReturnJellyfishBullet(MobJellyfishBullet bullet)
    {
        int pvID = bullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void EnqueueBullet(int pvID)
    {
        Instance.bulletPool.Enqueue(pvID);
    }

    public void EnqueueMobBullet(int pvID)
    {
        Instance.slimeBulletPool.Enqueue(pvID);
    }

    public void EnqueueCroissantBomb(int pvID)
    {
        Instance.croissantBombPool.Enqueue(pvID);
    }

    public void EnqueueJellyfishBullet(int pvID)
    {
        Instance.jellyfishBulletPool.Enqueue(pvID);
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [PunRPC]
    void RPC_ShowBullet(int pvID)
    {
        //Debug.Log(">>> RPC_ShowBullet 불림 : "+pvID);
        Debug.Log(">>> PhotonView.Find(pvID).gameObject.name : "+ PhotonView.Find(pvID).gameObject.name);
        /*if (PhotonView.Find(pvID).gameObject.name == "EnemyBullet(Clone)" && PhotonNetwork.IsMasterClient)
        {
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            return;
        }*/
        PhotonView.Find(pvID).gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_HideBullet(int pvID)
    {
        //Debug.Log(">>> RPC_HideBullet 불림 : "+pvID);
        PhotonView.Find(pvID).gameObject.SetActive(false);
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletPoolManagerReal : MonoBehaviour
{
    #region Fields
    private static BulletPoolManagerReal instance;

    Queue<int>[] bulletPool = new Queue<int>[5]; // 1001 2001 2002 2003
    //Queue<int> bulletPool = new Queue<int>(); //Bullet
    //Queue<MobBullet> slimeBulletPool = new Queue<MobBullet>();
    //Queue<MobCroissantBomb> croissantBombPool = new Queue<MobCroissantBomb>();


    PhotonView photonview;


    #endregion

    #region Methods
    private void Start()
    {
        photonview = GetComponent<PhotonView>();

        //if (!PhotonNetwork.IsMasterClient) { return; }
        //Debug.Log(">>> BulletPoolManager 처음 만듦!!!");
        for (int i=1; i<5; i++)
        {
            bulletPool[i] = new Queue<int>();
            Pooling(5, i);
        }

    }

    public static BulletPoolManagerReal Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<BulletPoolManagerReal>();
            return instance;
        }
    }

    private void Pooling(int count, int idx)
    {
        for (int i = 0; i < count; i++) 
        {
            //bulletPool.Enqueue(CreateNewBullet());
            //photonview.RPC("RPC_Enqueue", RpcTarget.All, CreateNewBullet());
            bulletPool[idx].Enqueue(CreateNewBullet());
        }
    }

    /// <summary>
    /// 새로운 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private int CreateNewBullet()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        Bullet newBullet = PhotonNetwork.Instantiate("Bullet", pos, rot).GetComponent<Bullet>();
        //      newBullet.transform.SetParent(Instance.transform);

        ///GameObject o = newBullet.gameObject;
        ///Debug.Log(">>> o 포톤 뷰 아이디 : "+o.GetComponent<PhotonView>().ViewID);
        int pvID = newBullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(false);
        return pvID;

    }


    /// <summary>
    /// bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public Bullet GetBullet(int playerID)
    {
        Debug.Log(">>> Get Bullet called!");
        playerID = GetIdx(playerID);
        Debug.Log(">>> playerID 잘 바뀜??!! : " + playerID);
        if (Instance.bulletPool[playerID].Count > 0)
        {
            int pvID = Instance.bulletPool[playerID].Dequeue();
            EnqueueBullet(pvID, playerID);
            //PhotonView bulletPvID = PhotonView.Find(pvID);
            //bulletPvID.TransferOwnership(playerID);
            //newBullet.transform.SetParent(null);
            photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(true); 
            return PhotonView.Find(pvID).gameObject.GetComponent<Bullet>();
        }
        else
        {
            int pvID = CreateNewBullet();
            EnqueueBullet(pvID, playerID);
            //newBullet.transform.SetParent(null);
            //int pvID = newBullet.gameObject.GetComponent<PhotonView>().ViewID;
            photonview.RPC("RPC_ShowBullet", RpcTarget.All, pvID); //newBullet.gameObject.SetActive(true);
            return PhotonView.Find(pvID).gameObject.GetComponent<Bullet>();
        }
    }


    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(Bullet bullet)
    {
        int pvID = bullet.gameObject.GetComponent<PhotonView>().ViewID;
        photonview.RPC("RPC_HideBullet", RpcTarget.All, pvID); //bullet.gameObject.SetActive(false);
        //bullet.transform.SetParent(Instance.transform);
        //Instance.bulletPool[playerID-1000].Enqueue(pvID);
        //photonview.RPC("RPC_Enqueue", RpcTarget.All, pvID);
    }


    public void EnqueueBullet(int pvID, int playerID)
    {
        Instance.bulletPool[playerID].Enqueue(pvID);
    }
    
    private int GetIdx(int playerID)
    {
        if (playerID == 1001) return 1;
        else if (playerID == 2001) return 2;
        else if (playerID == 2002) return 3;
        else if (playerID == 2003) return 4;
        else return 0;
    }

    #endregion


    [PunRPC]
    void RPC_ShowBullet(int pvID)
    {
        Debug.Log(">>> RPC_ShowBullet 불림 : "+pvID);
        //b.gameObject.SetActive(true);
        PhotonView.Find(pvID).gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_HideBullet(int pvID)
    {
        Debug.Log(">>> RPC_HideBullet 불림 : "+pvID);
        //b.gameObject.SetActive(false);
        PhotonView.Find(pvID).gameObject.SetActive(false);
    }

    *//*[PunRPC]
    void RPC_Dequeue()
    {
        Instance.bulletPool.Dequeue();
    }

    [PunRPC]
    void RPC_Enqueue(int pvID)
    {
        Instance.bulletPool.Enqueue(pvID);
    }*//*
}*/




/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletPoolManagerReal : MonoBehaviour
{
    #region Fields
    private static BulletPoolManagerReal instance;

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject slimeBulletPrefab;

    [SerializeField]
    GameObject croissantBombPrefab;

    Queue<Bullet> bulletPool = new Queue<Bullet>();
    //Queue<MobBullet> slimeBulletPool = new Queue<MobBullet>();
    //Queue<MobCroissantBomb> croissantBombPool = new Queue<MobCroissantBomb>();


    #endregion

    #region Methods
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log(">>> BulletPoolManager 처음 만듦!!!");
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Pooling(100);
            //MobPooling(50);
            //CroissantBombPooling(50);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log(">>> BulletPoolManager 이미 있음, 죽임!!!");
        }
        else
        {
            Debug.Log(">>> BulletPoolManager 이미 있음!!!");
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        Pooling(100);
        //MobPooling(50);
        //CroissantBombPooling(50);
    }

    public static BulletPoolManagerReal Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Pooling(int count)
    {
        for (int i = 0; i < count; i++) 
        {
            bulletPool.Enqueue(CreateNewBullet());
        }
    }

    private void MobPooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            slimeBulletPool.Enqueue(CreateNewMobBullet());
        }
    }

    private void CroissantBombPooling(int count)
    {
        for (int i = 0;i < count; i++)
        {
            croissantBombPool.Enqueue(CreateNewCroissantBomb());
        }
    }

    /// <summary>
    /// 새로운 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private Bullet CreateNewBullet()
    {
        //TempBullet newBullet = Instantiate(bulletPrefab).GetComponent<TempBullet>();
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        //Bullet newBullet = PhotonNetwork.InstantiateRoomObject("Bullet", pos, rot).GetComponent<Bullet>();
        Bullet newBullet = PhotonNetwork.Instantiate("Bullet", pos, rot).GetComponent<Bullet>();
        newBullet.gameObject.SetActive(false);
        return newBullet;
    }


    /// <summary>
    /// 새로운 몬스터 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private MobBullet CreateNewMobBullet()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        MobBullet newBullet = PhotonNetwork.Instantiate("EnemyBullet", pos, rot).GetComponent<MobBullet>();
        newBullet.gameObject.SetActive(false);
        return newBullet;
    }

    /// <summary>
    /// 새로운 폭탄 생성하는 메소드
    /// Photon 연결하면 반환값 Temp 떼면 된다
    /// </summary>
    /// <returns>풀링할 새로운 폭탄</returns>
    private MobCroissantBomb CreateNewCroissantBomb()
    {
        Vector2 pos = Vector2.zero;
        Quaternion rot = Quaternion.identity;
        MobCroissantBomb newBomb = PhotonNetwork.Instantiate("EnemyBomb", pos, rot).GetComponent<MobCroissantBomb>();
        newBomb.gameObject.SetActive(false);
        return newBomb;
    }

    /// <summary>
    /// bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public Bullet GetBullet()
    {
        Debug.Log(">>> Get Bullet called!");
        if (instance.bulletPool.Count > 0)
        {
            Bullet newBullet = instance.bulletPool.Dequeue();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
        else
        {
            Bullet newBullet = CreateNewBullet();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public MobBullet GetMobBullet()
    {
        if (instance.bulletPool.Count > 0)
        {
            MobBullet newBullet = instance.slimeBulletPool.Dequeue();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
        else
        {
            MobBullet newBullet = CreateNewMobBullet();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public MobCroissantBomb GetCroissantBomb()
    {
        if (instance.bulletPool.Count > 0)
        {
            MobCroissantBomb newBomb = instance.croissantBombPool.Dequeue();
            newBomb.transform.SetParent(null);
            newBomb.gameObject.SetActive(true);
            return newBomb;
        }
        else
        {
            MobCroissantBomb newBomb = CreateNewCroissantBomb();
            newBomb.transform.SetParent(null);
            newBomb.gameObject.SetActive(true);
            return newBomb;
        }
    }

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(instance.transform);
        instance.bulletPool.Enqueue(bullet);
    }

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(MobBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(instance.transform);
        instance.slimeBulletPool.Enqueue(bullet);
    }

    public void ReturnBullet(MobCroissantBomb bomb)
    {
        bomb.gameObject.SetActive(false);
        bomb.transform.SetParent(instance.transform);
        instance.croissantBombPool.Enqueue(bomb);
    }
    #endregion
}
*/