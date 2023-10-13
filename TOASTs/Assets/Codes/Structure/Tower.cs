using UnityEngine;

public class Tower : MonoBehaviour
{
	// 일단 더미 건물 하나
	public TowerData towerData;
	[SerializeField]
	public int remains;// 남은 건설 횟수
	[SerializeField]
	public int maxRemains; // 타워 최대 건설 횟수

	Rigidbody2D rigid;
	Animator ani;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		ani = GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		// 

	}
}
