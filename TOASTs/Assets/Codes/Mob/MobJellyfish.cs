using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobJellyfish : DefaultMob
{

    //bool strike = false;
    public float BarrageCoolTime = 8f;
    public int barrageCount = 0;
    protected override void Init()
    {
        MobIndex = 3;
        base.Init();
        //strike = false;
        entity.MaxHp = 100;
        entity.Armor = 1;
        AttackRange = 1000f;
        AttackCoolTime = 0.2f;
        AcquisitionRange = 1000f;
        MoveSpeed = 0f;
        Damage = 10;
        BarrageCoolTime = 8f;
        if (GameManager.instance.waveCount >= 5)
        {
            entity.Armor = 3;
        }
    }
    protected override void Idle()
    {
        //base.Idle();
        if (!orderInit)
        {
            waitTime = Time.fixedTime + Random.Range(0.05f, 0.5f); // 랜덤 딜레이
            anim.SetTrigger("Idle");
            orderInit = true;
            Stop();
        }
        target = defaultTarget;
        if (target != null && Time.fixedTime >= waitTime)
        {
            if(barrageCount == 0 && Time.fixedTime >= lastAttackTime + BarrageCoolTime)
            {
                //BarrageCoolTime = Time.fixedTime;
                lastAttackTime = Time.fixedTime + Random.Range(minRandomAtkDelay, maxRandomAtkDelay); // 랜덤 딜레이
                barrageCount = 8;
            }
            if (barrageCount > 0 && Time.fixedTime >= lastAttackTime + AttackCoolTime)
            {
                lastAttackTime = Time.fixedTime + Random.Range(minRandomAtkDelay, maxRandomAtkDelay); // 랜덤 딜레이
                CreateBullet();
                barrageCount--;
            }
        }
    }
    protected override void Move()
    {
        if (!orderInit)
        {
            Stop();
            orderInit = true;
        }
    }
    protected override void Attack()
    {
    }

    void CreateBullet()
    {
        Vector2 mobPosition = transform.position;
        Vector2 targetPosition = defaultTarget.transform.position;
        Vector2 direction = (targetPosition - mobPosition).normalized;

        Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        MobJellyfishBullet bullet = PhotonNetwork.Instantiate("JellyfishBullet", transform.position, Quaternion.identity).GetComponent<MobJellyfishBullet>();



        bullet.transform.position = mobPosition;
        bullet.transform.rotation = Quaternion.identity;
        bullet.bulletDamage = Damage;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * 20f;
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