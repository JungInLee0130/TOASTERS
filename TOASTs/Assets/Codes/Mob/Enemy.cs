using UnityEngine;

public class Enemy : MonoBehaviour
{
	public EnemyData enemyData;
	public BakeryController bakeryController;

	public RuntimeAnimatorController[] animCon;
	public Rigidbody2D target;

	bool isLive;
	public bool attack = false;

	Rigidbody2D rigid;
	Animator anim;
	SpriteRenderer spriter;


	void AttackTrue()
	{
		attack = true;
	}

	void AttackFalse()
	{
		attack = false;
	}
	// 초기화
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriter = GetComponent<SpriteRenderer>();
	}

	// 몬스터 체력 설정중...
	void OnEnable()
	{
		target = GameManagerSiltr.instance.player.GetComponent<Rigidbody2D>();
		isLive = true;
		enemyData.health = enemyData.maxHealth;
	}

	// 물리적 이동
	void FixedUpdate()
	{
		if (!isLive) return;
		Vector2 dirVec = target.position - rigid.position;
		Vector2 nextVec = dirVec.normalized * enemyData.speed * Time.fixedDeltaTime;

		rigid.MovePosition(rigid.position + nextVec);
		rigid.velocity = Vector2.zero;
	}

	// 스프라이트 방향 전환
	void LateUpdate()
	{
		if (!isLive) return;

		spriter.flipX = target.position.x < rigid.position.x;
	}





	/*public void Init(SpawnData data)
	{
		anim.runtimeAnimatorController = animCon[data.spriteType];
		enemyData.speed = data.speed;
		enemyData.maxHealth = data.health;
		enemyData.health = data.health;

	}*/
}

[System.Serializable]
public class EnemyData
{
	public float speed;
	public int health; // 현재 체력
	public int maxHealth; // 최대 체력
	public int atkDmg = 1; // 공격 데미지
	public float atkSpeed = 1.0f; // 공격 속도
}
