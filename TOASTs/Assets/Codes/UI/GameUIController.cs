using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    //PhotonView photonview;

    // UI 요소
    VisualElement _hp;
    Label _goldText;
    Label _wheatText;

    List<Label> _hpText;
    List<Label> _state;

    // Start is called before the first frame update
    void Start()
    {
        //photonview = GetComponent<PhotonView>();

        //gold = GameManager.instance.gold;
        //wheat = GameManager.instance.wheat;
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UI Toolkit 요소
        _hp = root.Q<VisualElement>("Hp");
        _goldText = root.Q<Label>("GoldText");
        _wheatText = root.Q<Label>("WheatText");
        _hpText = root.Query<Label>("HpText").ToList();
        _state = root.Query<Label>("StateText").ToList();

        // 초기화
        SetState(10, 0, 5);
        SetPlayerHp(200, 200);
        SetGold();
        SetWheat();
    }

    // Update is called once per frame
    void Update()
    {
        SetGold();
        SetWheat();
    }

    public void SetState(int attackPower, int defensePower, float moveSpeed)
    {
        for(int i=0; i<3; i++)
        {
            switch (i)
            {
                case 0:
                    _state[i].text = attackPower.ToString();
                    Debug.Log(attackPower);
                    break;

                case 1:
                    _state[i].text = defensePower.ToString();
                    Debug.Log(defensePower);
                    break;

                case 2:
                    _state[i].text = (moveSpeed * 2).ToString();
                    Debug.Log(moveSpeed);
                    break;

            }
        }
    }

    // 플레이어 체력 설정
    public void SetPlayerHp(int Hp, int Maxhp)
    {
        // 보험
        if (Hp < 0) Hp = 0;

        // float로 변환
        int newWidthPercentage = (int)((float)Hp / Maxhp * 100f);
        _hp.style.width = new Length(newWidthPercentage, LengthUnit.Percent);

        // 현재 hp text 표시
        _hpText[0].text = Hp.ToString();
        _hpText[1].text = Hp.ToString();

    }

    // 머니 설정
    public void SetGold()
    {
        _goldText.text = GameManager.instance.gold.ToString();

    }

    // 밀 설정
    public void SetWheat()
    {
        _wheatText.text = GameManager.instance.wheat.ToString();
        //photonview.RPC("RPC_UpdateWheat", RpcTarget.All, GameManager.instance.wheat);
    }

    
}
