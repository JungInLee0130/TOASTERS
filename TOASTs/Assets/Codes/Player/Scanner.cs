using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scanner : MonoBehaviour
{
	public float playerRange;
	public LayerMask targetLayer; // 원하는 layer만 타겟으로 할수있음
	public RaycastHit2D[] targets; // 레이캐스트 타겟들 저장
	public Transform nearestTarget;

	GameObject nearestObject = null;

    PlayerController player;

    PhotonView photonview;

    void Awake()
    {
        //player = GetComponentInParent<TempPlayerController>();
		//spawnPoints = GetComponentInChildren<BreadSpawn>().spawnPoint;
    }

	void Start()
	{
        Scene scene = SceneManager.GetActiveScene();
		if (scene.name == "GameScene")
		{
			photonview = GameObject.Find("WheatManager").GetComponent<PhotonView>();
		}
		player = GetComponentInParent<PlayerController>();
        //spawnPoints = GetComponentInChildren<BreadSpawn>().spawnPoint;
    }

    void Update()
	{
        Animator animator = player.GetComponent<Animator>();

        // 플레이어의 범위안에 아이템이 있으면 q를 사용해 먹기
        if (Input.GetKeyDown(KeyCode.Q) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Loot_Player") && !animator.GetCurrentAnimatorStateInfo(0).IsName("PickUp_Player"))
		{
            Debug.Log(">> Q 누름");
            targets = Physics2D.CircleCastAll(transform.position, playerRange, Vector2.zero, 0, targetLayer);

			nearestTarget = GetNearest();

			//Debug.Log(">> nearestObject.layer : "+ nearestTarget.gameObject.layer); 
			if (nearestTarget == null) return;

			nearestObject = nearestTarget.gameObject;

            switch (nearestObject.layer)
            {
                case 3: // 빵
                    SoundManager.Instance.StartSound(4);
                    Attain(nearestObject);	// 빵 냠냠
                    break;
                case 10: // 나무
					player.LootToScanner(nearestObject.transform);
					Invoke("CutTree", 3.5f); 
                    break;
                case 11: // 광물
                    player.LootToScanner(nearestObject.transform);
                    //Invoke("CutTree", 3.5f);
                    break;
                default:
                    break;
            }
		}
	}


	// 가장 가까이있는 아이템 리턴
	Transform GetNearest()
	{
		Transform result = null;
		float minDiff = 100;

		foreach (RaycastHit2D target in targets)
		{
			Vector2 myPos = transform.position;
			Vector2 targetPos = target.transform.position;


            float curDiff = Vector2.Distance(myPos, targetPos);

			if (curDiff < minDiff)
			{
				minDiff = curDiff;
				result = target.transform;
			}
		}
        return result;
	}

	// 아이템 습득
	void Attain(GameObject nearestObject)
	{
		Debug.Log(">> 빵 냠냠");
		//nearestObject.SetActive(false);

		int breadNum = nearestObject.GetComponent<BreadData>().num;
		player.ActivateEffect(breadNum);
		string str = nearestObject.transform.parent.gameObject.name;
        photonview.RPC("RPC_DestroyBread", RpcTarget.All, str); 
		//PhotonNetwork.Destroy(nearestObject);
	}

	// 벌목
	public void CutTree()
	{
		//nearestObject.SetActive(false);
		//Debug.Log(">>> nearestObject.name : " + nearestObject.name);
		int pvID = nearestObject.GetComponent<PhotonView>().ViewID;
		//Debug.Log(">>> nearestObject pvID : " + pvID);
		//Debug.Log(">>> photonview.ViewID?: " + photonview.ViewID);
        photonview.RPC("RPC_HideWheat", RpcTarget.All, pvID);
        GameManager.instance.wheat += 10;
        photonview.RPC("RPC_UpdateWheat", RpcTarget.All, GameManager.instance.wheat);
        //GameManager1.instance.AddWood();

    }

}
