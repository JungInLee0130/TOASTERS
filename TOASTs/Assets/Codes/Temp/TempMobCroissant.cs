/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMobCroissant : TempDefaultMob {

    protected override void Init()
    {
        base.Init();
        //strike = false;
        entity.MaxHp = 150;
        AttackRange = 8f;
        AttackCoolTime = 3.5f;
        AcquisitionRange = 12f;
        MoveSpeed = 1.5f;
        Damage = 80;
    }

    protected override void Attack()
    {
        Debug.Log($"attack: {target}");
        // 특수한 공격 루틴, 기존의 공격 루틴을 완전히 덮어쓴다.
        if (!orderInit)
        {
            lastAttackTime = Time.fixedTime - AttackCoolTime + 0.5f; // 공격할 때 0.5초의 선딜레이를 준다
            anim.SetTrigger("Attack");
            Stop();
            orderInit = true;
        }
        

        TempEntity targetEntity = target.GetComponent<TempEntity>();
        if (targetEntity == null)
        {
            // 체력 정보가 없는 타겟이라면?
            target = defaultTarget;
            SetOrder(Order.Move);
            return;
        }

        // 평소에는 공격 준비 모션을 취하면서 공격 쿨다운이 다 될 때 까지 기다린다.
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (target == defaultTarget)
        {
            distanceToTarget -= 3f; // 빵집이면 사거리 증가
        }
        if (distanceToTarget <= AttackRange)//타겟이 공격 범위 내에 있고,
        {
            if (Time.fixedTime - lastAttackTime >= AttackCoolTime) // 공격 쿨다운이 지났을 때
            {

                if (targetEntity.Hp > 0)
                {
                    lastAttackTime = Time.fixedTime - Random.Range(minRandomAtkDelay, maxRandomAtkDelay); // 랜덤 딜레이
                    //strike = true;
                    CreateMob(1.5f);
                }
                else
                {
                    if (target != defaultTarget)
                    {
                        target = defaultTarget;
                        SetOrder(Order.Move);
                        return;
                    }
                }


            }
        }
        else
        {
            SetOrder(Order.Move); // 적이 사거리 바깥에 있으면 이동
        }
    }
    void CreateMob(float time)
    {
//        Vector2 mobPosition = transform.position;
        //Vector2 targetPosition = target.transform.position;
        //Vector2 direction = (targetPosition - mobPosition).normalized;
        float xOffset = (GetComponent<SpriteRenderer>().flipX ? 1 : -1) * 1f;
        Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //TempMobCroissantBomb bomb = BulletPoolManager.instance.GetCroissantBomb();
        //bomb.transform.position = transform.position + new Vector3(xOffset, 0, 0.5f);
        //bomb.transform.rotation = bulletRotation;
        GameObject obj = Instantiate(rangeAttack, transform.position + new Vector3(xOffset, 0, 0.5f), bulletRotation);
        TempMobCroissantBomb bomb = obj.GetComponent<TempMobCroissantBomb>();
        if (bomb != null)
        {
            bomb.Init(target, time, 8f, Damage, 1f);
            bomb.targetLayer = combinedLayerMask;
        }

    }


}
*/