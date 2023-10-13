using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryAnim : MonoBehaviour
{

    public int BakingBread = 0;    // ���� ���� �ִ� ���� ����

    Animator anim;
    BakeryController bakery;

    void Awake()
    {
        anim = GetComponent<Animator>();
        bakery = GetComponent<BakeryController>();
    }

    /*void Update()
    {
        int curHp = bakery.towerData.curHp;
        int maxHp = bakery.towerData.maxHp;


        // �׽�Ʈ ��
        if (Input.GetKeyDown(KeyCode.O)) 
        {

            curHp = (curHp + 10 > maxHp) ? maxHp : curHp + 10;

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            curHp = (curHp - 10 < 0) ? 0 : curHp - 10;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            BakingBread++;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            BakingBread--;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            OnFinishAnim();
        }

        // ���� ���
        if(curHp <= 0)
        {
            OnDestroyAnim();
        }
    }*/

    // Update is called once per frame
    void LateUpdate()
    {
        // �������� �� ������ �ִϸ��̼� ��ȯ
        anim.SetInteger("Baking", BakingBread);
    }

    void OnFinishAnim()
    {
        anim.SetTrigger("Finish");
    }

    /*void OnDestroyAnim()
    {
        anim.SetTrigger("Destroy");
    }*/
}
