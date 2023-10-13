using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MobMouse : DefaultMob {

    protected override void Init()
    {
        base.Init();
        entity.MaxHp = 10;
        AttackRange = 1.5f;
        AttackCoolTime = 1f;
        AcquisitionRange = 3f;
        MoveSpeed = 3.5f;
        Damage = 10;
        if(GameManager.instance.waveCount >= 5)
        {
            AttackCoolTime = 0.5f;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        //lastAttackTime = Time.time;
        // �ٰŸ� ����
        // ���� �ִϸ��̼� ���� setTrigger

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            SetOrder(Order.Idle);
            if (target.transform.CompareTag("Player"))
            {
                if (target.gameObject.GetComponent<TempEntity>().Hp > 0)
                {
                    target.gameObject.GetComponent<PlayerController>().GainDamage(Damage);
                }
            }
            //Debug.Log("Target >>> Player");

            else if (target.transform.CompareTag("Bakery"))
            {
                //Debug.Log("Target >>> Bakery HP: " + target.gameObject.GetComponent<TempEntity>().Hp);
                // ���� ���� �ڵ尡 ���� �̿ϼ�
                if (target.gameObject.GetComponent<TempEntity>().Hp > 0)
                {
                    target.gameObject.GetComponent<BakeryController>().GainDamage(Damage);
                }
            }
        }
    }
    [PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
        GameManager.instance.currentWave.Remove(this.gameObject);
    }

    /*private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        
    }*/

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    //protected override void Idle()
    //{
    //    base.Idle();
    //    if (!orderInit)
    //    {
    //        orderInit = true;

    //    }
    //    else
    //    {

    //    }
    //}

    //protected override void Attack()
    //{
    //    base.Attack();
    //    lastAttackTime = Time.time;
    //    // �ٰŸ� ����
    //    // ���� �ִϸ��̼� ���� setTrigger
    //    if (target.transform.CompareTag("Player"))
    //    {
    //        Debug.Log("Target >>> Player");
    //        target.gameObject.GetComponent<PlayerController>().GainDamage(damage);
    //    }
    //    else if (target.transform.CompareTag("Bakery"))
    //    {
    //        Debug.Log("Target >>> Bakery");
    //        // ���� ���� �ڵ尡 ���� �̿ϼ�
    //        target.gameObject.GetComponent<BakeryController>().GainDamage(damage);
    //    }
    //}

    //void LateUpdate()
    //{
    //    anim.SetBool("HasTarget", base.target != null);
    //}




}
