using System;
using System.Collections;
using UnityEngine;

// 메인 빵집 : 빵 생성
public class TempBakeryController : MonoBehaviour
{
	public Transform[] spawnPoint;
	public GameObject breadPrefab;
	GameObject[] bread; // 프리팹 사용
	//public int curHp;
	//public int maxHp;
	public bool isFixed;

    Animator ani;
	Rigidbody2D rigid;
	TempEntity entity;

	// 임시
	public bool isBreak = false;

	// 빵 대기 ArrayList
	public ArrayList readyBreads = new ArrayList(); // 5개 최대

	// 빵 완료 ArrayList
	public ArrayList comBreads = new ArrayList(); // 5개 최대



	// hp 초기화
	void Awake()
	{
		ani = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();

        bread = new GameObject[5];
		entity = GetComponent<TempEntity>();
		//maxHp = 500;
		//curHp = 500;
		entity.Restore();
		MakeBread();
    }

    void Start()
    {
    }

    void Update()
	{
        // 발표용 코드
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("키 입력 체크");
            ani.SetInteger("Baking", 1);
        }

        // 발표용 코드
        if (Input.GetKeyDown(KeyCode.N))
        {
            ani.SetTrigger("Finish");
        }

        // 수리 받음 : 수리 오브젝트 받을시 체력 회복? or 체력 만땅?
        if (isFixed)
		{
			isFixed = true;
			FixTower();
		}

		// 빵 대기 배열 : 0 이상 5 이하
		if (0 <= readyBreads.Count && readyBreads.Count <= 5)
        {
            //ani.SetInteger("Baking", readyBreads.Count);

			// 빵 완료 배열에 추가 : 0이상 5이하
			if (comBreads.Count >= 0 && comBreads.Count < 5)
			{
				FinishBread();
				return;
			}

			return;
		}
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
		if (!isBreak && !entity.GainDamage(damage))
		{
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

    void FixTower()
	{
		if (isFixed)
		{
			// 고치기
			isFixed = false;
			return;
		}
	}

	// 빵생성
	void MakeBread() // 일단 5개 초기화
	{

		for (int i = 0; i < 5; i++)
		{
			bread[i] = Instantiate(breadPrefab);
			bread[i].transform.position = spawnPoint[i].position;
			bread[i].GetComponent<SpriteRenderer>().sprite = breadPrefab.GetComponent<Breads>().sprites[i];
			readyBreads.Add(bread[i]);
		}
	}

	// 빵 생성 완료
	void FinishBread()
	{
		GameObject go = null;

		foreach (GameObject bp in readyBreads)
		{
			go = bp;
			readyBreads.Remove(bp);			
			break;
		}

		comBreads.Add(go);
	}
}


