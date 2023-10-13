using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MobBoss : DefaultMob
{
    PhotonView photonview;
    RaycastHit2D[] targets;
    bool strike = false;
    bool chargeInit = false;
    float meleeCooldown;
    float skillCooldown; // 공통 스킬 쿨타임
    float summonCooldown; // 소환 쿨타임
    float chargeTimer; // 돌진 타이머
    //float chargeCooldown = Time.time; // 돌진 쿨타임
    //float bombCooldown = Time.time; // 폭탄 쿨타임
    public GameObject croissantBomb;
    public GameObject teleportEffect;
    Vector2 attackTarget;
    int phase = 1;
    //int SkillNo = 0;

    void Start()
    {
        photonview = GetComponent<PhotonView>();    
    }
    protected override void Init()
    {
        MobIndex = 4;
        base.Init();
        strike = false;
        entity.MaxHp = 500 + PhotonNetwork.PlayerList.Length * 300;
        entity.Armor = 3;
        AttackRange = 12f;
        AttackCoolTime = 6f;
        AcquisitionRange = 8f;
        MoveSpeed = 3f;
        Damage = 100;
        meleeCooldown = Time.fixedTime;
        skillCooldown = Time.fixedTime;
        summonCooldown = Time.fixedTime;

    }

    void FixedUpdate()
    {
        if (phase == 1 && entity.Hp <= entity.MaxHp * 2 / 3)
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
            collision.gameObject.GetComponent<PlayerController>().GainDamage(Damage);
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
                if (Time.fixedTime >= skillCooldown) // 스킬을 사용할 수 있다면...
                {
                    //
                    bool canUseSummon = Time.fixedTime >= summonCooldown; // 마지막 소환으로부터 쿨타임이 지났다면
                    if (canUseSummon)
                    {
                        SetOrder(Order.Summon);
                        return;
                    }
                    //int decide = Random.Range(0, phase - 1);
                    //if (Physics2D.CircleCastAll(transform.position, 5f, Vector2.zero, 0, LayerMask.GetMask("Player")).Length > 0)
                    //{
                    //    SetOrder(Order.Spell);
                    //    return;
                    //}
                    //if (Physics2D.CircleCastAll(transform.position, 10f, Vector2.zero, 0, LayerMask.GetMask("Player")).Length > 0)
                    //{
                    //    //SetOrder(Order.Spell);
                    //    //SetOrder(Order.Attack);
                    //    SetOrder(Order.Charge);
                    //    return;
                    //}
                    int decide = Random.Range(0, phase == 1 ? 4 : phase == 2 ? 6 : 10);
                    int[] decideArray = { 0, 0, 1, 1, 1, 2, 2, 2, 2 };
                    // 0 0 1 
                    // 0 0 1 1 1
                    // 0 0 1 1 1 2 2 2 2
                    decide = decideArray[decide];
                    switch (decide)
                    {
                        case 0:
                            SetOrder(Order.Spell);
                            break;
                        case 1:
                            SetOrder(Order.Attack);
                            break;
                        case 2:
                            SetOrder(Order.Charge);
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
        //base.Attack();
        if (!orderInit)
        {
            anim.SetTrigger("Attack");
            skillCooldown = Time.fixedTime + 3f;
            Stop();
            //strike = false;
            orderInit = true;

            targets = Physics2D.CircleCastAll(transform.position, AcquisitionRange * 2, Vector2.zero, 0, LayerMask.GetMask("Player"));
            if (targets.Length > 0)
            {
                int randomPlayer = Random.Range(0, targets.Length);
                if (targets[randomPlayer].transform.CompareTag("Player"))
                {
                    // 무작위 플레이어를 공격하려 함
                    attackTarget = targets[randomPlayer].transform.position;
                    
                }
            }
            else
            {
                attackTarget = defaultTarget.transform.position;
            }
            spr.flipX = (attackTarget.x > transform.position.x);
        }
    }

    void Summon()
    {
        if (!orderInit)
        {
            anim.SetTrigger("Summon");
            skillCooldown = Time.fixedTime + 3f;
            summonCooldown = Time.fixedTime + 15f;
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
            skillCooldown = Time.fixedTime + 4f;
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
        // 돌진공격
        if (!orderInit)
        {

            //agent.enabled = false;

            // 각도: 빵집을 향하지만 거기서 랜덤으로 적당히 퍼짐
            Vector2 mobPosition = transform.position;
            Vector2 targetPosition = defaultTarget.transform.position;
            Vector2 direction = (targetPosition - mobPosition).normalized;
            float aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            aimAngle += Random.Range(-22.5f, 22.5f);

            // 거리: 현재 위치에서 빵집의 반대편으로 날아가기 충분한 거리
            float aimDistance = Vector2.Distance(transform.position, defaultTarget.transform.position);
            aimDistance = Mathf.Max(20f, aimDistance * 2);

            // 해당위치에 보스가 있을 수 있는지 체크한다. (없으면 거리를 늘이거나 줄여본다)
            bool mayCharge = false;
            NavMeshHit hit;
            for (int i = 0; i <= 20; i++)
            {
                var newDistance = aimDistance + 2f * (((i + 1) / 2) * (i % 2 == 0 ? 1 : -1));
                var x = newDistance * Mathf.Cos(aimAngle * Mathf.Deg2Rad);
                var y = newDistance * Mathf.Sin(aimAngle * Mathf.Deg2Rad);
                Vector3 chargeDestination = transform.position + new Vector3(x, y, 0f);
                if (NavMesh.SamplePosition(chargeDestination, out hit, 1.0f, NavMesh.AllAreas))
                {
                    Debug.Log("Can Hit");
                    mayCharge = true;
                    attackTarget = hit.position;//new Vector2(chargeDestination.x, chargeDestination.y);
                    break;
                }
            }
            if (mayCharge)
            {
                spr.flipX = (attackTarget.x > transform.position.x);
                Vector2 direction2 = (attackTarget - mobPosition).normalized;
                //Debug.Log("돌진 방향: "+ direction2);
                anim.SetTrigger("Charge");
                skillCooldown = Time.fixedTime + 6f;
                Stop();
                orderInit = true;
                chargeTimer = 0f;
                strike = false;
                chargeInit = false;
            }
            else
            {
                skillCooldown = Time.fixedTime + 1f;
                SetOrder(Order.Idle);
                return;
            }

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Charging"))
        {


            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            Vector2 mobPosition = transform.position;
            Vector2 direction = (attackTarget - mobPosition).normalized;

            float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (!chargeInit)
            {
                collider.enabled = true;
                collider.isTrigger = true;
                spr.flipX = false;
                transform.rotation = Quaternion.AngleAxis(rotationAngle - 180, Vector3.forward);
                chargeInit = true;
                agent.enabled = false;
            }

            chargeTimer += Time.fixedDeltaTime;
            rb.velocity = direction * 40f;

            float dist = Vector2.Distance(transform.position, attackTarget);

            if (!strike && (dist <= 1f || chargeTimer >= 1.5f))
            {
                strike = true;
                collider.isTrigger = false;
                anim.SetTrigger("ChargeEnd");
                transform.rotation = Quaternion.identity;
                spr.flipX = rb.velocity.x > 0f;
                agent.enabled = true;
                rb.drag = 5.0f;
                rb.velocity = Vector2.zero;
                Stop();
            }
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.ChargeEnd"))
        {
            CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
            collider.enabled = true;
            collider.isTrigger = false;
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            transform.rotation = Quaternion.identity;
            agent.enabled = true;
            rb.velocity = Vector2.zero;
            Stop();
        }
    }
    void MakeSlimes()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (phase == 3 && Random.Range(0,2) == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                float radius = 2f;
                float angle = 360 * i / 2;
                float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                GameObject croissant = PhotonNetwork.Instantiate("MobCroissant", transform.position + new Vector3(x, y, 0), Quaternion.identity);
            }
        }
        else
        {
            int count = 2 + phase;
            for (int i = 0; i < count; i++)
            {
                float radius = 2f;
                float angle = 360 * i / count;
                float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                GameObject slime = PhotonNetwork.Instantiate("MobSlime", transform.position + new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
    void Stomp()
    {
        for (int i = 0; i < 6; i++)
        {
            float radius = 2.5f + phase * 0.5f;
            float angle = 360 * i / 6;
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);


            MobCroissantBomb bomb = PhotonNetwork.Instantiate("EnemyBomb", transform.position + new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<MobCroissantBomb>();

            int bombDamage = 80;
            if (bomb != null)
            {
                bomb.Init(target, 0f, 8f, bombDamage, 1f);
                bomb.targetLayer = combinedLayerMask;
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

        Instantiate(teleportEffect, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        Instantiate(teleportEffect, teleportTo + new Vector3(0, 2f, 0), Quaternion.identity);

        transform.position = teleportTo;
    }
    //IEnumerator MoveBullet(float time)
    //{
    //    Vector2 mobPosition = transform.position;
    //    Vector2 targetPosition = target.transform.position;
    //    Vector2 direction = (targetPosition - mobPosition).normalized;
    //    MobBullet bullet = BulletPoolManagerReal.Instance.GetMobBullet();
    //    bullet.transform.position = mobPosition;
    //    bullet.transform.rotation = Quaternion.identity;
    //    bullet.bulletDamage = Damage;
    //    float timer = 0;
    //    float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    bullet.transform.rotation = Quaternion.AngleAxis(rotationAngle - 200, Vector3.forward);

    //    while (timer < time)
    //    {
    //        if (bullet.gameObject == null) break;
    //        timer += Time.deltaTime;
    //        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //        rb.velocity = direction * 10f;

    //        yield return null; // 한 프레임 기다림
    //    }
    //}

    public void Strike()
    {
        Vector2 mobPosition = transform.position + new Vector3(spr.flipX ? 5f : -5f, 0 ,0 );
        Vector2 targetPosition = attackTarget;
        Vector2 direction = (targetPosition - mobPosition).normalized;
        BossStrike bullet = PhotonNetwork.Instantiate("BossStrike", transform.position, Quaternion.identity).GetComponent<BossStrike>();
        bullet.transform.position = transform.position + new Vector3(spr.flipX ? 5f : -5f, 0, 0);
        bullet.transform.rotation = Quaternion.identity;
        bullet.bulletDamage = 150;
        //float timer = 0;
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(rotationAngle - 180, Vector3.forward);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * 60f;
    }

    //void SkillEnd()
    //{
    //    SetOrder(Order.Idle);
    //}

    protected override void Die()
    {
        photonview.RPC("RPC_DestroyMob", RpcTarget.MasterClient);
        GameManager.instance.Victory();
    }

    [PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.instance.currentWave.Remove(this.gameObject);
    }
}
