/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMobSlime : TempDefaultMob {

    bool strike = false;
    protected override void Init()
    {
        MobIndex = 1;
        base.Init();
        strike = false;
        entity.MaxHp = 30;
        AttackRange = 3f;
        AttackCoolTime = 3f;
        AcquisitionRange = 6f;
        MoveSpeed = 2f;
        Damage = 30;

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
    IEnumerator MoveBullet(float time)
    {
        Vector2 mobPosition = transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - mobPosition).normalized;
        //TempMobBullet bullet = BulletPoolManager.instance.GetMobBullet();
        TempMobBullet bullet = Instantiate(rangeAttack).GetComponent<TempMobBullet>();
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


}
*/