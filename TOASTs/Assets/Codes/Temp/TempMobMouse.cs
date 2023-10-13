/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMobMouse : TempDefaultMob {

    protected override void Init()
    {
        base.Init();
        entity.MaxHp = 10;
        AttackRange = 1.5f;
        AttackCoolTime = 0.5f;
        AcquisitionRange = 3f;
        MoveSpeed = 3.5f;
        Damage = 5;
    }

    protected override void Attack()
    {
        base.Attack();
        //lastAttackTime = Time.time;
        // 근거리 공격
        // 공격 애니메이션 실행 setTrigger

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            SetOrder(Order.Idle);
            if (target.transform.CompareTag("Player"))
            {
                if (target.gameObject.GetComponent<TempEntity>().Hp > 0)
                {
                    target.gameObject.GetComponent<TempPlayerController>().GainDamage(Damage);
                }
            }
            //Debug.Log("Target >>> Player");

            else if (target.transform.CompareTag("Bakery"))
            {
                // 빵집 관련 코드가 아직 미완성
                if (target.gameObject.GetComponent<TempEntity>().Hp > 0)
                {
                    target.gameObject.GetComponent<BakeryController>().GainDamage(Damage);
                }
            }
        }
    }



}
*/