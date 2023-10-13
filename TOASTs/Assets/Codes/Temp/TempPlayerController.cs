using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TempPlayerController : MonoBehaviour
{
        
        public Vector2 inputVec;
        public GameObject sampleBullet;
        BulletPoolManagerReal bulletPoolManager;
        GameObject scanner;

        // 캐릭터 스텟
        //public int health;          // 체력
        public int attackPower;     // 공격력
        public int defensePower;    // 방어력
        public float moveSpeed;     // 이동속도

        // 캐릭터 피격
        float curDamagedDelay;          // 현재 피격 중. 무적 아이템을 먹었다면 curDD를 엄청 낮추면 된다.
        public float maxDamagedDelay;   // 피격 쿨타임
        bool isInvincibility = false;   // 무적

        // 총알
        public float bulletSpeed = 10f;
        float curShotDelay;
        public float attackSpeed;

        // 센서 범위
        public float sensorRange = 0.2f;

        // 빵집 UI > 이거 왜 필요하더라
        public BakeryUIController bakeryUIController;

        // 플레이어의 레이어
        int originalOrderInLayer = 1;

        float AtkMoveSlowCoeffecient = 0.5f;

        NavMeshAgent agent;

        public enum PlayerState
        {
            CanMove,
            Looting,
            Dead
        }

        public PlayerState currentState;

        SpriteRenderer spriter;
        Collider2D col;
        Rigidbody2D rigid;
        Animator anim;
        TempEntity entity;

        void Awake()
        {
            spriter = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            rigid = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            entity = GetComponent<TempEntity>();
            entity.MaxHp = 200;
            entity.Restore();
            bakeryUIController = GetComponent<BakeryUIController>();
            scanner = transform.Find("Scanner").gameObject;

            bulletPoolManager = BulletPoolManagerReal.Instance;
        
        }

        void Start()
        {
            // 게임 시작 시 체력 풀피
            GameManager.instance.gameUIController.SetPlayerHp(entity.Hp, entity.MaxHp);

            // 스텟 UI 수정
            SetStateUI();
        }

        // input 시스템으로 캐릭터 이동 받아오기
        void OnMove(InputValue value)
        {
            inputVec = value.Get<Vector2>();
        }

        void Update()
        {
            // 마우스가 클릭하고 있는 위치

            if (Input.GetMouseButton(0))
            {
                // 죽으면 발사 못함
                if (currentState != PlayerState.CanMove) return;

                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Fire(mouseWorldPosition);
            }


            // 재장전
            DelayTime();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                entity.Hp -= 10;
            }

            // 체력 실시간 반영
            GameManager.instance.gameUIController.SetPlayerHp(entity.Hp, entity.MaxHp);
        }

        public void LootToScanner(Transform trans)
        {
            Debug.Log(">> LootToScanner 실행됨");

            if (trans.position.y + 0.7 > transform.position.y)
            {
                originalOrderInLayer = spriter.sortingOrder;
                spriter.sortingOrder = 3;
            }

            OnLoot();
            Invoke("OnPickUp", 3f);
        }

        void FixedUpdate()
        {
            // 움직일 수 있는 상태가 아니면 리턴
            if (currentState != PlayerState.CanMove) return;
            float tempMoveSpeed = moveSpeed;
            if (curShotDelay < attackSpeed)
                tempMoveSpeed *= AtkMoveSlowCoeffecient;
            Vector2 nextVec = inputVec.normalized * tempMoveSpeed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
        }

        void LateUpdate()
        {
            // 움직일 수 있는 상태가 아니면 리턴
            if (currentState != PlayerState.CanMove) return;

            // 플레이어 방향 바꾸기
            if (inputVec.x != 0)
            {
                spriter.flipX = inputVec.x < 0;
            }


            // 애니메이션
            anim.SetFloat("Speed", inputVec.magnitude); // 벡터의 크기

            // 테스트
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    OnDeath();
            //}

            // 왼쪽 마우스 > 공격
            if (Input.GetMouseButton(0))
            {
                OnAttack();
            }
            else
            {
                anim.SetBool("IsAttack", false);
            }
        }

        void Fire(Vector2 targetPosition)
        {
            if (curShotDelay < attackSpeed) return;

            // 클릭한 위치와 플레이어와의 방향
            Vector2 shootingDirection = (targetPosition - (Vector2)transform.position).normalized;

            // 총알의 회전을 계산
            float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // 총알이 발사되는 위치
            Vector3 firePos = transform.position + new Vector3(1, 0, 0) * 0.5f * (shootingDirection.x < 0 ? -1 : 1)
                    + new Vector3(0, 1, 0) * 0.2f * -1;

            // 총알 생성 및 회전 적용
            Bullet bullet = bulletPoolManager.GetBullet();
            Debug.Log(bullet);
            bullet.transform.position = firePos;
            bullet.transform.rotation = bulletRotation;
    //        GameObject bullet = Instantiate(sampleBullet, firePos, bulletRotation);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = shootingDirection * bulletSpeed;
            SoundManager.Instance.StartSound(3);

            curShotDelay = 0;
        }

        public void GainDamage(int damage)
        {

            if (!gameObject.GetComponent<TempEntity>().GainDamage(damage))
            {
                OnDeath();
                return;
            }

            // 피격 애니
            OnHurt();

            // 무적 반짝반짝
            StartCoroutine(OnInvincibility(maxDamagedDelay));

            curDamagedDelay = 0;

            //int hitDamage = (damage < defensePower) ? 0 : damage - defensePower;
            //health -= hitDamage;

            //if (health <= 0)
            //{
            //    OnDeath();
            //    return;
            //}

            //// 피격 애니
            //OnHurt();

            //// 무적 반짝반짝
            //StartCoroutine(OnInvincibility(maxDamagedDelay));

            //curDamagedDelay = 0;
        }

        /*
        void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Enemy")) return;

            if (curDamagedDelay < maxDamagedDelay) return;

            DefaultMob Mob = collision.gameObject.GetComponent<DefaultMob>();
            if (Mob != null)
            {
                int hitDamage = (Mob.damage < defensePower) ? 0 : Mob.damage - defensePower;
                health -= hitDamage;

                // 사망
                if(health < 0)
                {
                    OnDeath();
                    return;
                }
            }

            // 피격 애니
            OnHurt();

            // 무적 반짝반짝
            StartCoroutine(OnInvincibility(maxDamagedDelay));

            curDamagedDelay = 0;
        } 
        */

        IEnumerator OnInvincibility(float invincibilityTime)
        {
            ChangeStateToCanMove();
            isInvincibility = true;
            invincibilityTime *= 10f;

            // maxDamagedDelay(3초)동안 무적시간
            for (int i = 0; i < invincibilityTime; i++)
            {
                Color color = spriter.color;
                color.a = (color.a == 1f) ? 0.3f : 1f;
                spriter.color = color;

                yield return new WaitForSeconds(0.1f);
            }

            // 초기화
            Color originalColor = spriter.color;
            originalColor.a = 1f;
            spriter.color = originalColor;

            isInvincibility = false;
        }

        IEnumerator OnPenetrate(float penetrateTime)
        {
            GameManager.instance.isPenetrate = true;
            float currentTime = 0f;

            while (currentTime <= penetrateTime)
            {
                currentTime += Time.deltaTime;

                yield return null;
            }

            GameManager.instance.isPenetrate = false;
        }

        // 귀환하기
        void Recall()
        {
            transform.position = Vector3.zero;
            Revive();
        }

        // 부활하기
        void Revive()
        {
            currentState = PlayerState.CanMove;
            col.enabled = true;
            anim.SetTrigger("Revive");
        }

        void DelayTime()
        {
            curShotDelay += Time.deltaTime;
            curDamagedDelay += Time.deltaTime;
        }

        // 다른 애니메이션 끝날 때 플레이어 상태를 CanMove로 전환
        public void ChangeStateToCanMove()
        {
            currentState = PlayerState.CanMove;
            spriter.sortingOrder = originalOrderInLayer;    // 플레이어의 레이어 정상화
        }

        public void CutWheat()
        {
            scanner.GetComponent<Scanner>().CutTree();
        }

        // 플레이어 일하는 동작
        void OnLoot()
        {
            anim.SetTrigger("Loot");
            currentState = PlayerState.Looting;
        }

        // 플레이어 일하는 동작2
        void OnPickUp()
        {
            anim.SetTrigger("PickUp");
        }

        // 플레이어 공격하는 동작
        void OnAttack()
        {
            anim.SetBool("IsAttack", true);
        }

        // 플레이어 사망하는 동작
        void OnDeath()
        {
            col.enabled = false;
            anim.SetTrigger("Death");
            SoundManager.Instance.StartSound(6);
            currentState = PlayerState.Dead;
        }

        // 플레이어 사망하는 동작
        void OnHurt()
        {
            anim.SetTrigger("Hurt");
            SoundManager.Instance.StartSound(5);
        }

        void SetStateUI()
        {
            // UI 스텟 세팅
            GameManager.instance.gameUIController.SetState(attackPower, defensePower, moveSpeed);

        }

        /************* 빵 능력*******************/
            
        // 1. 공격력 영구 증가
        void IncreasePlayerAttack(int increaseAttack)
        {
            attackPower += increaseAttack;
            SetStateUI();

        }

        // 2. 방어력 영구 증가
        void IncreasePlayerDefense(int increaseDefense)
        {
            defensePower += increaseDefense;
            SetStateUI();
        }

        // 3. 이동속도 영구 증가
        void IncreasePlayerSpeed(int increaseSpeed)
        {
            moveSpeed += increaseSpeed;
            SetStateUI();
        }

        // 4. 플레이어 체력 회복
        void RecoverPlayerHp(int recoverHp)
        {
            entity.Hp += recoverHp;

            if (entity.Hp > entity.MaxHp)
            {
                entity.Hp = entity.MaxHp;
            }
        }

        // 5. 빵 제작 시간 감소
        void DecreaseBreadTime(int timePercent)
        {
            float time = timePercent / 100.0f;

            // 현재 SetBreadTime이 없는 상황
            GameManager.instance.Bakery.GetComponent<Breads>().UpdateBreadTime(time);
        }

        // 6. 공격속도 증가
        void DecreasePlayerAttackDelay(int decreaseAttackPercent)
        {
            float interval = decreaseAttackPercent * 0.01f;

            float attackDelay = attackSpeed * interval;

            attackSpeed -= attackDelay;
        }

        // 7. 최대 체력 증가
        void IncreaseMaxHealth(int increaseMaxHealth)
        {
            entity.MaxHp += increaseMaxHealth;
            entity.Hp += increaseMaxHealth;
        }


        //  8. 모든 플레이어 체력 회복
        void RecoverAllPlayersHp(int recoverHp)
        {
            // 포톤 설정 필요
            RecoverPlayerHp(recoverHp);
        }

        // 9. 총알 관통
        void MakeBulletPenetrate(float penetrateTime)
        {
            StartCoroutine(OnPenetrate(penetrateTime));
        }

        // 10. 승리 빵 추가
        void MakeWinBread()
        {
            // 게임 승리 UI 출력
            // 게임종료
        }

        // 11. 무적 빵
        void MakeInvincible(float invincibleTime)
        {
            // 나중에 public 바꿔줘야함
            StartCoroutine(OnInvincibility(maxDamagedDelay));

            //buffUIController.GetComponent<BuffUIController>().AddBuff(2,3,20);
        }

        // 12. 능력치 랜덤 빵
        void MakeRandomBread(int points)
        {
            int attack = Random.Range(0, points + 1);

            points -= attack;

            int defense = Random.Range(0, points + 1);

            points -= defense;

            int speed = points;

            attackPower += attack;
            defensePower += defense;
            moveSpeed += speed;

            SetStateUI();
        }



        public void ActivateEffect(int breadNum)
        {
            switch (breadNum)
            {
                case 1:
                    IncreasePlayerAttack(5);
                    break;
                case 2:
                    IncreasePlayerDefense(5);
                    break;
                case 3:
                    IncreasePlayerSpeed(4);
                    break;
                case 4:
                    RecoverPlayerHp(500);
                    break;
                case 5:
                    DecreaseBreadTime(10);
                    break;
                case 6:
                    DecreasePlayerAttackDelay(10);
                    break;
                case 7:
                    IncreaseMaxHealth(500);
                    break;
                case 8:
                    RecoverAllPlayersHp(500);
                    break;
                case 9:
                    MakeBulletPenetrate(10);
                    break;
                case 10:
                    MakeWinBread();
                    break;
                case 11:
                    MakeInvincible(10);
                    break;
                case 12:
                    MakeRandomBread(10);
                    break;
            }
        }
        
        
}
