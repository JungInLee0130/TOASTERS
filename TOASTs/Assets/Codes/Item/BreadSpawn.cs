using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreadSpawn : MonoBehaviour
{
	public Transform[] spawnPoint; // 소환 지점
	public GameObject breadPrefab;
	bool[] isSpawn; // spawnPoint 소환 여부
	int isSpawnLen; // isSpawn의 길이
	public bool onFinishAni = false;

	BakeryController bakeryController;


	void Awake()
	{
		bakeryController = GetComponentInParent<BakeryController>();
		//bakeryController = GameObject.Find("Bakery").GetComponent<BakeryController>();

		isSpawnLen = 5;
		isSpawn = new bool[isSpawnLen];
	}

	void Update()
	{

		int len = bakeryController.comBreads.Count;

		// 완성된 빵이 있으면 소환
		if (len != 0) SpawnBread();
	}

	// 완료배열 comBreads 빵들을 소환함
	void SpawnBread()
	{
		checkSpawnPoint();

		BreadData breadData = null;

		// 완료 큐
		foreach (BreadData item in bakeryController.comBreads)
		{
			breadData = item;
            bakeryController.comBreads.Remove(item);
			break;
		}

		int index = -1;

		// 소환지점
		for (int i = 0; i < isSpawnLen; i++)
		{
			if (!isSpawn[i])
			{
				index = i;
				isSpawn[i] = true;
				break;
			}
		}


		// 빈곳이 없거나 소환할 빵이 없음.
		if (index == -1)
		{
            onFinishAni = true;

            return;
		} 
		else
		{
            onFinishAni = false;
        }

		// 빵을 소환함 : 포지션, 스프라이트
		GameObject breadSpawn = Instantiate(breadPrefab);

		breadSpawn.transform.parent = spawnPoint[index].transform;
		breadSpawn.transform.position = spawnPoint[index].position;
		breadSpawn.GetComponent<SpriteRenderer>().sprite = breadData.sp;
		InitBreadData(breadSpawn, breadData);

		GetComponent<BakeryController>().BakingAnim();

    }

	// BreadPrefab에 BreadData를 대입
	void InitBreadData(GameObject breadSpawn, BreadData breadData)
	{
		breadSpawn.GetComponent<BreadData>().num = breadData.num;
		breadSpawn.GetComponent<BreadData>().tier = breadData.tier;
		breadSpawn.GetComponent<BreadData>().name = breadData.name;
		breadSpawn.GetComponent<BreadData>().sp = breadData.sp;
		breadSpawn.GetComponent<BreadData>().seconds = breadData.seconds;
		breadSpawn.GetComponent<BreadData>().effect = breadData.effect;
		breadSpawn.GetComponent<BreadData>().gold = breadData.gold;
	}

	// 소환된 빵 개수 리턴
	public int getIsSpawnLength()
	{
		int count = 0;

		for (int i = 0; i < isSpawnLen; i++)
		{
			if (isSpawn[i])
			{
				count++;
			}
		}

		return count;
	}

	// 소환 가능한 장소인지 체크 
	public void checkSpawnPoint()
	{
		for (int i = 0; i < isSpawnLen; i++)
		{
			Transform point = spawnPoint[i];
			Transform tempBread = null;

			// point 있으면
			try
			{
				tempBread = point.GetChild(0);
				if (tempBread != null)	isSpawn[i] = true;
			}
			// point에 없으면
			catch (Exception e)
			{
				if (tempBread == null)
				{
					isSpawn[i] = false;
					break;
				}
			}
		}
		
	}
}
