using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


// 메인 빵집 : 빵 생성
public class BakeryController : MonoBehaviour
{
    PhotonView photonview;

    public int curHp;
	public int maxHp;
	public bool isFixed;

	Animator ani;
	BreadData[,] breadDatas;
	public TempEntity entity;
	//BakeryHPBarUI healthUI;

	// 임시
	public bool isBreak = false;

	// 빵 대기 ArrayList
	//public ArrayList readyBreads = new ArrayList(); // 5개 최대
	public List<BreadData> readyBreads = new List<BreadData>(); /// 이게 빵 굽 큐

	// 빵 완료 ArrayList
	public ArrayList comBreads = new ArrayList(); // 5개 최대

	// UI
	public BakeryUIController BUIC;
	bool isBaking = false;



	// hp 초기화
	void Awake()
	{
		ani = GetComponent<Animator>();

		breadDatas = GetComponent<Breads>().breadDatas;

		entity = GetComponent<TempEntity>();
		//entity.Restore();
		//healthUI = GetComponent<BakeryHPBarUI>();
		//healthUI.SetMaxHealth(entity.maxHp);
	}

	void Start()
	{
        photonview = GetComponent<PhotonView>();

        breadDatas = GetComponent<Breads>().breadDatas;
	}

    private void Init()
    {
		entity.Restore();
    }


    void Update()
	{

        /*// 수리 받음 : 수리 오브젝트 받을시 체력 회복? or 체력 만땅?
		if (isFixed)
		{
			isFixed = true;
			FixTower();
		}*/
    }

	void GameOver()
	{

	}

	void broken()
	{
		if (entity.Hp <= 0)
		{
			isBreak = true;
			return;
		}
	}

	public void GainDamage(int damage)
	{
		bool isAlive = entity.GainDamage(damage);
		//healthUI.SetHealth(entity.hp);
		if (!isBreak && !isAlive)
		{
			SoundManager.Instance.StartSound(2);
			ani.SetTrigger("Destroy");
			isBreak = true;
			//          // 게임종료 호출 (게임 매니저)
		}

		//      // 체력까임
		//      curHp = (curHp - damage < 0) ? 0 : curHp - damage;

		//      if (curHp == 0 && !isBreak)
		//{
		//          ani.SetTrigger("Destroy");
		//	isBreak = true;

		//          // 게임종료 호출 (게임 매니저)
		//      }
		//return;
	}

	void OnExplosion()
	{
		Transform childTransform = transform.GetChild(1);

		if (childTransform != null)
		{
			childTransform.GetComponent<Explosion>().OnExplosion();
		}
	}

	/*void FixTower()
	{
		if (isFixed)
		{
			// 고치기
			isFixed = false;
			return;
		}
	}*/

	// 인덱스로 빵찾기
	public BreadData SearchBread(int num)
	{
		int tierLen = 3;
		int indexLen = 4;

		for (int tier = 0; tier < tierLen; tier++)
		{
			for (int index = 0; index < indexLen; index++)
			{
				if (num == breadDatas[tier, index].num)
				{
					return breadDatas[tier, index];
				}
			}
		}

		return null;
	}

	// UI를 눌렀을때 빵 준비 배열에 대입
	public void MakeBread(int BreadId)
	{
        photonview.RPC("RPC_MakeBread", RpcTarget.All, BreadId);
    }

    [PunRPC]
    void RPC_MakeBread(int BreadId)
    {
        // UI로 부터 가져온 빵 데이터. 현재는 더미데이터로 한다.
        BreadData selectedBread = SearchBread(BreadId);
        readyBreads.Add(selectedBread);
    }

    // 서버시간이 경과되면, comBreads로 이동함 -> breadSpawn
    public void FinishBread()
	{
		//Debug.Log("Finish Bread");
        BreadData breadData = GetFirstBread();

		readyBreads.Remove(breadData);
		comBreads.Add(breadData);

		// UI 수정 전송
		BUIC.removeLastBread(readyBreads.Count);

    }

	public void readyBread(int BreadId)
	{
		int readyBreadsLen = readyBreads.Count;

		//Debug.Log("Bakery is Clicked : ");
		//Debug.Log(readyBreadsLen);

		// 빵 대기 배열 : 0 이상 5 이하
		if (0 <= readyBreadsLen && readyBreadsLen < 5)
		{
			// 빵굽기
			//ani.SetInteger("Baking", readyBreads.Count);
			MakeBread(BreadId);
			return;
		}
	}

	public BreadData GetFirstBread()
	{
        if (readyBreads.Count > 0)
        {
            return readyBreads[0];
        }

        return null;
    }

    public Boolean IsEmptyFirstBread()
    {
        if (readyBreads.Count > 0)
        {
            return false;
        }

        return true;
    }

	public void BakingAnim()
	{
        if (readyBreads.Count == 0)
        {
            ani.SetInteger("Baking", 0);
        }
        else
        {
            if (GetComponent<BreadSpawn>().getIsSpawnLength() == 5)
            {
                ani.SetInteger("Baking", 0);
            }
            else
            {
                ani.SetInteger("Baking", 1);
            }
        }
    }
}


