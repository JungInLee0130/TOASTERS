using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryAnim : MonoBehaviour
{

    public int BakingBread = 0;    // 현재 굽고 있는 빵의 개수

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


        // 테스트 용
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

        // 빵집 사망
        if(curHp <= 0)
        {
            OnDestroyAnim();
        }
    }*/

    // Update is called once per frame
    void LateUpdate()
    {
        // 제작중인 빵 개수로 애니메이션 변환
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
