using UnityEngine;

// 몬스터 소환 클래스
public class Spawner : MonoBehaviour
{
	public Transform[] spawnPoint; // 소환 지점
	public SpawnData[] spawnData;

	int level; // 몬스터 소환 레벨
	float timer; // 몬스터 소환 간격

	// 소환 지점 초기화
	void Awake()
	{
	}

	// 10초 경과시 몬스터 유형 바뀜
	void Update()
	{
		// 게임 시간
		timer += Time.deltaTime;

		// 타이머가 일정 시간 값이 도달하면 소환하도록 설정
		if (timer > 3f)
		{
			timer = 0;
			Spawn();
		}
	}

	// 몬스터 소환
	public void Spawn()
	{
		
		for (int i = 0; i < spawnPoint.Length; i++)
		{
			//GameObject mob = GameManager1.instance.pool.Get(Random.Range(0, 2));
			//mob.transform.position = spawnPoint[i].position;
			//mob.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
		}
		//enemy.GetComponent<DefaultMob>().Init(spawnData[level]);
	}
}

[System.Serializable]
public class SpawnData
{
	public float spawnTime; // 스판 간격
	public int spriteType; // 스판 유형
	public int hp; // 체력
	public float speed; // 속도
}