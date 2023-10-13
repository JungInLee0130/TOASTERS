using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerCreate : MonoBehaviour
{

    public static PlayerCreate instance = null;
    // public bool isConnect = false;
    public Transform[] spawnPoints;
    public GameObject curPlayer;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        // StartCoroutine(CreatePlayer());
        CreatePlayer();
    }

    void CreatePlayer()
    {
        // yield return new WaitUntil(() => isConnect);
        //GameObject playerTemp = PhotonNetwork.Instantiate("Player", Vector2.one, Quaternion.identity, 0);

        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        int idx = Random.Range(1, spawnPoints.Length);
        Vector2 pos = spawnPoints[idx].position;
        Quaternion rot = spawnPoints[idx].rotation;

        Debug.Log("PC : " + pos);
		curPlayer = PhotonNetwork.Instantiate("Player", pos, rot, 0);
		//GameObject playerTemp = PhotonNetwork.Instantiate("Player", pos, rot, 0);
		PhotonView photonView = curPlayer.GetComponent<PhotonView>();

        // 카메라
        if (photonView.IsMine && SceneManager.GetActiveScene().name == "GameScene")
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            virtualCamera.Follow = curPlayer.transform;
        }
    }

}
