using UnityEngine;

public class GameManagerSiltr : MonoBehaviour
{
	public static GameManagerSiltr instance;

	public float gameTime; // 시작 시간
	float maxGameTime = 2 * 10f; // 최대 시간 (몬스터 스판 인덱스 관련)

	public PoolManager pool;
	public PlayerController player;
	public Enemy enemy;
	public BakeryController breadHouse;

	void Awake()
	{
		instance = this;
	}

	// 게임타임 최대시간 초과시 초기화
	void Update()
	{
		gameTime += Time.deltaTime;

		if (gameTime > maxGameTime)
		{
			gameTime = maxGameTime;

		}
	}
}
