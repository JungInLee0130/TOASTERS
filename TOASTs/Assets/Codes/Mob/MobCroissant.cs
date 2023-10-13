using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCroissant : DefaultMob
{
    public MobCroissantBomb myBomb;
    protected override void Init()
    {
        base.Init();
        //strike = false;
        entity.MaxHp = 120;
        entity.Armor = 1;
        AttackRange = 6f;
        AttackCoolTime = 2.5f;
        AcquisitionRange = 9f;
        MoveSpeed = 1.5f;
        Damage = 60;
        //if (GameManager.instance.waveCount >= 5)
        //{
        //    //MoveSpeed = 2.25f;
        //    //AttackRange += 3f;
        //    //AcquisitionRange += 4f;
        //}
    }

    protected override void Attack()
    {
        //Debug.Log(">>> "+ PhotonNetwork.NickName + " / "+ target.GetComponent<PhotonView>().Controller.NickName);
        //if (PhotonNetwork.NickName != target.GetComponent<PhotonView>().Controller.NickName) return;
        //Debug.Log($"attack: {target}");
        // 특수한 공격 루틴, 기존의 공격 루틴을 완전히 덮어쓴다.
        if (!orderInit)
        {
            lastAttackTime = Time.fixedTime - AttackCoolTime + 0.5f; // 공격할 때 0.5초의 선딜레이를 준다
            anim.SetTrigger("Attack");
            Stop();
            orderInit = true;
        }
        //Stop();

        TempEntity targetEntity = target.GetComponent<TempEntity>();

        Debug.Log($">>>> target:{target.GetPhotonView().ViewID}");
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
        if (distanceToTarget <= AcquisitionRange)//타겟이 공격 범위 내에 있고,
        {
            if (Time.fixedTime - lastAttackTime >= AttackCoolTime && myBomb == null) // 공격 쿨다운이 지났고, 내가 쏜 폭탄이 없을 때
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

        if (target == defaultTarget) // 빵집을 공격중이면 어그로를 돌릴 수 있음
        {
            FindEnemy();
        }
        if (target == null) return;

        float xOffset = (GetComponent<SpriteRenderer>().flipX ? 1 : -1) * 1f;
        Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // MobCroissantBomb bomb = BulletPoolManagerReal.Instance.GetCroissantBomb();
        if (PhotonNetwork.NickName != target.GetComponent<PhotonView>().Controller.NickName) return;
        MobCroissantBomb bomb = PhotonNetwork.Instantiate("EnemyBomb", transform.position + new Vector3(xOffset, 0, 0.5f), bulletRotation).GetComponent<MobCroissantBomb>();

        if (bomb != null)
        {
            // 타겟, 지속시간, 이동속도, 데미지, 폭발반경 및 사거리
            bomb.Init(target, time, 8f, Damage, 1.5f);
            bomb.targetLayer = combinedLayerMask;
            bomb.parent = this;
        }
        myBomb = bomb;
    }
    protected override void Die()
    {
        if(myBomb != null)
        {
            myBomb.Explode();
        }
        base.Die();
    }

    [PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.instance.currentWave.Remove(this.gameObject);
    }
}
