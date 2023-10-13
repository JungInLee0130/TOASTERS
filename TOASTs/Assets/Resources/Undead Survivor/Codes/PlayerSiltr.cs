using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSiltr : MonoBehaviour
{
	public Vector2 inputVec;
	public float speed; // 속도

	Rigidbody2D rigid;
	SpriteRenderer spriter;
	Animator anim;

	// 초기화
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriter = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
	}

	// 플레이어 이동
	void FixedUpdate()
	{
		Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
		rigid.MovePosition(rigid.position + nextVec);
	}


	void OnMove(InputValue value)
	{
		inputVec = value.Get<Vector2>();
	}


	void LateUpdate()
	{
		anim.SetFloat("Speed", inputVec.magnitude);

		if (inputVec.x != 0)
		{
			spriter.flipX = inputVec.x < 0;
		}
	}
}
