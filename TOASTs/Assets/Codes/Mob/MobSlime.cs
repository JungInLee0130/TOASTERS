using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MobSlime : DefaultMob
{

    bool strike = false;
    protected override void Init()
    {
        MobIndex = 1;
        base.Init();
        strike = false;
        entity.MaxHp = 30;
        AttackRange = 3f;
        AttackCoolTime = 3f;
        AcquisitionRange = 8f;
        MoveSpeed = 2f;
        if(GameManager.instance.waveCount >= 5)
        {
            AttackRange += 2f;
            MoveSpeed = 3f;
            //AcquisitionRange = 8f;
        }
        Damage = 30;

    }

    protected override void Attack()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        base.Attack();
        if (!strike && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            strike = true;
            MoveBullet();
            // StartCoroutine(MoveBullet(2.0f));
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            strike = false;
            SetOrder(Order.Idle);

        }
    }
    public override void GainDamage(int _damage)
    {
        if (currentOrder != Order.Die && !entity.GainDamage(_damage))
        {
            SetOrder(Order.Die);
        }
        else
        {
            if (Time.fixedTime - lastAttackTime >= AttackCoolTime && target != null && target != defaultTarget) // 공격 쿨다운이 지났을 때
            {
                TempEntity targetEntity = target.GetComponent<TempEntity>();
                if (targetEntity.Hp > 0)
                {
                    SetOrder(Order.Attack);
                }
            }
        }
    }

    void MoveBullet()
    {
        Vector2 mobPosition = transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - mobPosition).normalized;
        MobBullet bullet = PhotonNetwork.Instantiate("EnemyBullet", mobPosition, Quaternion.identity).GetComponent<MobBullet>();
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        bullet.bulletDamage = Damage;
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(rotationAngle - 180, Vector3.forward);

        rb.velocity = direction * 10f;

        /*        while (timer < time)
                {
                    if (bullet.gameObject == null) break;
                    timer += Time.deltaTime;
                    rb.velocity = direction * 10f;

                    yield return null; // 한 프레임 기다림
                }*/
    }

    [PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.instance.currentWave.Remove(this.gameObject);
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSlime : DefaultMob
{
    //new int damage;
    //new bool attackType;
    *//*protected override void Attack(bool type = true)
    {
        Debug.Log("오버라이딩 공격");
        // 지정된 적을 향해 투사체를 발사하는 기능 구현
    }*//*
    protected override void Attack()
    {
        //Debug.Log("공격 성공!");
        base.lastAttackTime = Time.time;
        {
            // 원거리 공격
            StartCoroutine(MoveBullet(2.0f));
        }
    }

    IEnumerator MoveBullet(float time)
    {
        Vector2 mobPosition = transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - mobPosition).normalized;
        //GameObject obj = Instantiate(rangeAttack, transform.position, Quaternion.identity);
        GameObject obj = BulletPoolManagerReal.Instance.GetMobBullet().gameObject;
        obj.transform.position = mobPosition;
        obj.transform.rotation = Quaternion.identity;
        
        float timer = 0;

        while (timer < time)
        {
            if (obj.gameObject == null)
            {
                break;
            }
            timer += Time.deltaTime;
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity = direction * 10f;

            yield return null; // 한 프레임 기다림
        }
    }
}
*/