using UnityEngine;

// HAN 코드 가져와서 테스트
public class BreadEnemy : MonoBehaviour
{
	Rigidbody2D rigid;
	Animator ani;

	Vector2 breadHousePos;

	public EnemyData breadEnemyData;

	bool stop;

	public GameObject target;               // 움직일 대상 오브젝트
	public Transform player;                // 추후 동적으로 넣는 방식으로 변경 예정

	[SerializeField]
	protected int hp;                       // 체력
	[SerializeField]
	protected int damage;                   // 공격력
	[SerializeField]
	protected float attackCoolTime = 2f;    // 공격 쿨타임
	[SerializeField]
	protected float attackRange = 5f;       // 공격 범위
	[SerializeField]
	protected float moveSpeed = 2.5f;       // 이동 속도

	float lastAttackTime;


	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		ani = GetComponent<Animator>();

		target = GameObject.Find("BreadHouse");

		breadHousePos = target.GetComponent<BakeryController>().transform.position;

		breadEnemyData = new EnemyData();
		breadEnemyData.atkDmg = 10;
	}


	void FixedUpdate()
	{
		if (stop) return;

		MobMove(target);
		AttackCheck(target);
	}

	// 공격 로직을 이곳에 구현 예정
	protected virtual void Attack()
	{
		Debug.Log("공격 성공!");
		lastAttackTime = Time.time;

		// 근거리 혹은 원거리 공격이 성공했을때의 로직
		// 근거리 혹은 원거리 공격이 실패했을때의 로직
	}


	// 공격 대상이 플레이어, 타워, 빵집 3종류이기 때문에 해당 객체를 타겟으로 넘겨주기 위해 파라메터를 넘겨주는 방식으로 구현
	// 최초 타겟은 빵집으로 선택할 예정
	void MobMove(GameObject target)
	{
		Vector2 dirVec2 = (target.GetComponent<Rigidbody2D>().position - rigid.position).normalized;
		Vector2 nextVec2 = dirVec2 * moveSpeed * Time.fixedDeltaTime;
		rigid.MovePosition(rigid.position + nextVec2);
		rigid.velocity = Vector2.zero;
	}


	// 사거리 안에 들어오면 건물에 부딫히면 공격 이벤트 실행
	void AttackCheck(GameObject target)
	{
		// 플레이어와 적 캐릭터 간의 거리 계산
		float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);

		// 플레이어가 공격 범위 내에 있고, 공격 쿨다운이 지났을 때
		if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCoolTime) Attack();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("BreadHouse"))
		{
			stop = true;
		}
	}
}
