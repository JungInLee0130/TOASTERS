using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TempMobBoss : TempDefaultMob {
    RaycastHit2D[] targets;
    bool strike = false;
    float meleeCooldown;
    float skillCooldown; // 공통 스킬 쿨타임
    float summonCooldown; // 소환 쿨타임
    //float chargeCooldown = Time.time; // 돌진 쿨타임
    //float bombCooldown = Time.time; // 폭탄 쿨타임
    public GameObject croissantBomb;
    int phase = 1;
    //int SkillNo = 0;
    protected override void Init()
    {
        MobIndex = 3;
        base.Init();
        strike = false;
        entity.MaxHp = 500;
        entity.Armor = 3;
        AttackRange = 12f;
        AttackCoolTime = 6f;
        AcquisitionRange = 8f;
        MoveSpeed = 3f;
        Damage = 25;
        meleeCooldown = Time.fixedTime;
        skillCooldown = Time.fixedTime;
        summonCooldown = Time.fixedTime;
        
    }

    void FixedUpdate()
    {
        if(phase == 1 && entity.Hp <= entity.MaxHp * 2 / 3)
        {
            phase = 2;
            entity.Armor += 1;
        }
        if (phase == 2 && entity.Hp <= entity.MaxHp * 1 / 3)
        {
            phase = 3;
            entity.Armor += 1;
        }
        switch (currentOrder)
        {
            case Order.Idle:        // 주변에 적이 있으면 Idle 상태가 됨
                Idle();
                break;
            case Order.Move:        // 주변에 적이 없으면 빵집으로 움직임
                Move();
                // NavMesh로 하여금 target으로 이동하기
                agent.SetDestination(target.transform.position);
                // 이동중엔 X를 뒤집을 수 있음 (공격중에 뒤집지 않는 이유는 이미 공격중에 방향이 바뀌면 어색해보임)
                spr.flipX = (target.transform.position.x > transform.position.x);
                break;
            case Order.Attack:      // 스킬 사용중
                Attack();
                break;
            case Order.Summon:
                Summon();
                break;
            case Order.Spell:
                Spell();
                break;
            case Order.Charge:
                Charge();
                break;
            case Order.Die:         // 사망함 (파괴 애니메이션이 있다면 재생함)
                Die();
                break;
        }

        //MobMove(target);

        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time < meleeCooldown + 1f) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어에게 닿으면 데미지를 줌
            collision.gameObject.GetComponent<TempPlayerController>().GainDamage(Damage);
            meleeCooldown = Time.time;
        }
    }

    protected override void Idle()
    {
        if (!orderInit)
        {
            waitTime = Time.fixedTime + Random.Range(0.25f, 1f); // 랜덤 딜레이
            anim.SetTrigger("Idle");
            orderInit = true;
            Stop();
        }

        if (Time.fixedTime >= waitTime)
        {
            if (HasEnemyAround(AttackRange)) // 주변에 적이 있다면 (획득 범위보다 더 김)
            {
                if(Time.fixedTime >= skillCooldown) // 스킬을 사용할 수 있다면...
                {
                    //
                    bool canUseSummon = Time.fixedTime >= summonCooldown; // 마지막 소환으로부터 30초 지났다면
                    if (canUseSummon)
                    {
                        SetOrder(Order.Summon);
                        return;
                    }
                    int decide =  Random.Range(0, phase-1);
                    if(Physics2D.CircleCastAll(transform.position, 5f, Vector2.zero, 0, LayerMask.GetMask("Player")).Length > 0)
                    {
                        SetOrder(Order.Spell);
                        return;
                    }
                    switch (decide)
                    {
                        case 0:
                            SetOrder(Order.Spell);
                            break;
                        case 1:

                            break;
                        case 2:
                            break;
                    }
                    //SkillNo 
                }
            }
            else
            {
                SetOrder(Order.Move);
            }
        }
    }

    bool HasEnemyAround(float range)
    {
        return Physics2D.CircleCastAll(transform.position, range, Vector2.zero, 0, combinedLayerMask).Length > 0;
    }

    protected override void Move()
    {
        if (!orderInit)
        {
            anim.SetTrigger("Move");
            agent.speed = MoveSpeed;
            orderInit = true;
        }
        if (HasEnemyAround(AcquisitionRange))
        {
            SetOrder(Order.Idle);       // 멈춰섬
        }
        else
        {
            target = defaultTarget;     // 빵집으로 이동
        }
        if (Time.fixedTime >= skillCooldown && Time.fixedTime >= summonCooldown) // 소환 스킬을 사용할 수 있다면...
        {
            SetOrder(Order.Summon);
            //SkillNo = 0;
         }

    }

    protected override void Attack()
    {
        base.Attack();
        if (!strike && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            strike = true;
            StartCoroutine(MoveBullet(2.0f));
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            strike = false;
            SetOrder(Order.Idle);
            
        }
    }

    void Summon()
    {
        if (!orderInit)
        {
            anim.SetTrigger("Summon");
            skillCooldown = Time.fixedTime + 3f;
            summonCooldown = Time.fixedTime + 10f;
            //anim.SetInteger("SkillNo", 0);
            Stop();
            strike = false;
            orderInit = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Summon") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            strike = false;
            SetOrder(Order.Idle);
        }
    }
    void Spell()
    {
        // 장판 공격
        if (!orderInit)
        {
            anim.SetTrigger("Spell");
            skillCooldown = Time.fixedTime + 3f;
            Stop();
            strike = false;
            orderInit = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Spell") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            strike = false;
            SetOrder(Order.Idle);
        }
    }
    void Charge()
    {

    }
    void MakeSlimes()
    {
        //float[,] summonOffsets = new float[4, 2] { { -3f, 0f }, { 3f, 0f }, { 0, -3f }, { 0f, 3f } };
        //Debug.Log("슬라임 소환!");
        int count = 2 + phase;
        for (int i = 0; i < count; i++)
        {
            float radius = 2f;
            float angle = 360 * i / count;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            GameObject slime = PoolManager.Instance.GetMob(1);
            slime.transform.position = transform.position + new Vector3(x, y, 0);
        }
    }
    void Stomp()
    {
        float[,] summonOffsets = new float[4, 2] { { -3f, 0f }, { 3f, 0f }, { 0, -3f }, { 0f, 3f } };

        for (int i = 0; i < 6; i++)
        {
            float radius = 2.5f + phase * 0.5f;
            float angle = 360 * i / 6;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            
            //bomb.transform.rotation = bulletRotation;
            GameObject obj = Instantiate(croissantBomb, transform.position + new Vector3(x, y, 0), bulletRotation);
            MobCroissantBomb bomb = obj.GetComponent<MobCroissantBomb>();

            int bombDamage = 80;
            if (bomb != null)
            {
                bomb.Init(target, 0f, 8f, bombDamage, 1f);
                //bomb.targetLayer = combinedLayerMask;
            }
        }
        Vector2 teleportNearby = Random.insideUnitCircle * 25f;
        Vector3 randomPoint = transform.position + new Vector3(teleportNearby.x, teleportNearby.y, 0f);
        Vector3 teleportTo = transform.position;
        NavMeshHit hit;
        for (int j = 0; j < 6; j++)
        {
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                teleportTo = hit.position;
                break;
            }
        }
        transform.position = teleportTo;
    }
    IEnumerator MoveBullet(float time)
    {
        Vector2 mobPosition = transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - mobPosition).normalized;
        MobBullet bullet = BulletPoolManagerReal.Instance.GetMobBullet();
        bullet.transform.position = mobPosition;
        bullet.transform.rotation = Quaternion.identity;
        bullet.bulletDamage = Damage;
        float timer = 0;
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(rotationAngle - 200, Vector3.forward);

        while (timer < time)
        {
            if (bullet.gameObject == null) break;
            timer += Time.deltaTime;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = direction * 10f;
            
            yield return null; // 한 프레임 기다림
        }
    }

    public void Strike()
    {

    }

}
