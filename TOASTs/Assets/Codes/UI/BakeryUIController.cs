using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;


public class BakeryUIController : MonoBehaviour
{

    VisualElement _bakeryContainer;
    private bool isOpen = false;

    // Bread Data
    int[,] SelectedBread = new int[3, 2];
    BreadData[,] breadDatas;

    // open/close flag btn
    Button _flagButton;

    // UI import
    List<VisualElement> _image_LOR;
    List<VisualElement> _bread;         // bread
    List<Label> _goldText;              // meeded gold
    List<Label> _wheatText;             // meeded wheat
    List<VisualElement> _wheatContainer;        
    List<VisualElement> _breadBaking;   // baking bread
    VisualElement _bar;                 // baking bar
    VisualElement _card;
    VisualElement _breadInCard;
    Label _nameInCard;
    Label _infoInCard;
    Label _tooFar;
    Label _moreMoney;
    Label _notYet;
    Label _flagText;




    // ? img
    public Sprite questImg;

    // bakery connect
    public GameObject bakery;

    void Start()
    {
        SelectedBread = GameManager.instance.SelectedBread;
        breadDatas = GameManager.instance.breadDatas;

        var root = GetComponent<UIDocument>().rootVisualElement;

        // root.Q import
        _bakeryContainer = root.Q<VisualElement>("BakeryContainer");
        _flagButton = root.Q<Button>("Flag");
        _image_LOR = root.Query<VisualElement>("Image_LOR").ToList();
        _bread = root.Query<VisualElement>("Bread").ToList();
        _goldText = root.Query<Label>("GoldText").ToList();
        _breadBaking = root.Query<VisualElement>("BreadBaking").ToList();
        _bar = root.Q<VisualElement>("Bar");
        _tooFar = root.Q<Label>("TooFar");
        _wheatText = root.Query<Label>("WheatText").ToList();
        _wheatContainer = root.Query<VisualElement>("WheatContainer").ToList();
        _moreMoney = root.Q<Label>("MoreMoney");
        _notYet = root.Q<Label>("NotYet");
        _flagText = root.Q<Label>("FlagText");

        //card
        _card = root.Q<VisualElement>("Card");
        _breadInCard = root.Q<VisualElement>("ImageInCard");
        _nameInCard = root.Q<Label>("NameInCard");
        _infoInCard = root.Q<Label>("InfoInCard");



        _flagButton.RegisterCallback<ClickEvent>(OnUIActive);

        _bakeryContainer.AddToClassList("close-sheet");

        // breads
        for (int i = 0; i < _bread.Count; i++)
        {
            int cardIndex = i;
            _bread[i].RegisterCallback<ClickEvent>(evt => SelectBread(evt, cardIndex));
        }

        // UI setting reset
        // yeah~!
        SetBasket();    

        // baking bread
        for (int i = 0; i < _breadBaking.Count; i++)
        {
            _breadBaking[i].style.backgroundImage = null;
        }

        // hover
        for (int i = 0; i < _bread.Count; i++)
        {
            int cardIndex = i;
            _bread[i].RegisterCallback<MouseEnterEvent>(evt => ShowInfoWindow(_card, cardIndex));
            _bread[i].RegisterCallback<MouseLeaveEvent>(evt => HideInfoWindow(_card, cardIndex));
        }


    }

    void Update()
    {
        // reset
        RefreshBaking();
        RefreshBar();

        // 멀리 떨어지면 강제 종료
        if(GameManager.instance.canOpenBakeryUIShutDown)
        {
            _bakeryContainer.AddToClassList("close-sheet");
            isOpen = false;
        }

    }

    // show
    void ShowInfoWindow(VisualElement _card, int cardIndex)
    {
        // ???
        if (SetCardInfo(cardIndex)) return;

        _card.style.display = DisplayStyle.Flex;
    }

    // hide
    void HideInfoWindow(VisualElement _card, int cardIndex)
    {
        _card.style.display = DisplayStyle.None;
    }

    bool SetCardInfo(int cardIndex)
    {
        int indexCol = cardIndex / 2;
        int indexRow = cardIndex % 2;

        if (SelectedBread[indexCol, indexRow] == 0)
        {
            return true;
        }
        else
        {


            BreadData bread = changeIndexToArray(SelectedBread[indexCol, indexRow]);

            // bread img
            Sprite newImage = bread.sp;
    
            _breadInCard.style.backgroundImage = new StyleBackground(newImage);

            // bread name
            String name = bread.name;
            _nameInCard.text = name;

            // bread effect
            String info = bread.effect;
            _infoInCard.text = info;

            return false;
        }


    }

    // 바 갱신
    void RefreshBar()
    {
        
        float curTime = bakery.GetComponent<BreadTimeManager>().curTime;
        float totalTime;
        float newWidthPercentage = 0;

        if (bakery.GetComponent<BakeryController>().IsEmptyFirstBread() == true)
        {
            newWidthPercentage = 0;
        }
        else
        {
            totalTime = bakery.GetComponent<BakeryController>().GetFirstBread().seconds;
            newWidthPercentage = (curTime / totalTime) * 97f;
        }
 
        _bar.style.width = new Length(newWidthPercentage, LengthUnit.Percent);  // ?덈퉬 媛믪쓣 ?쇱꽱?몃줈 蹂寃쏀빀?덈떎.
    }

    // 바 밑에 있는 생성 칸
    void RefreshBaking()
    {
        List<BreadData> readyBreads = bakery.GetComponent<BakeryController>().readyBreads;

        int len = readyBreads.Count;

        // 보험
        if (len > 5)
        {
            len = 5;
        }

        for (int i = 0; i < len; i++)
        {

            if (readyBreads[i].name == null)
            {
                _breadBaking[i].style.backgroundImage = null; 
            }
            else
            {
                // 이미지 가져오기
                Sprite newImage = readyBreads[i].sp;
                _breadBaking[i].style.backgroundImage = new StyleBackground(newImage);
            }
        }

    }

    // 앞으로 땡기기
    public void removeLastBread(int index)
    {
        // 없애기
        _breadBaking[index].style.backgroundImage = null;

        _bar.style.width = new Length(0, LengthUnit.Percent);
    }

    // 빵 선택 시
    public void SelectBread(ClickEvent evt, int cardIndex)
    {
        int indexCol = cardIndex / 2;
        int indexRow = cardIndex % 2;

        int breadIndex = SelectedBread[indexCol, indexRow];

        // 아직 선택 권한이 없는 빵이면?
        if (SelectedBread[indexCol, 1] == 0)
        {
            _notYet.style.display = DisplayStyle.Flex;
            Invoke("OffNotYetUI", 1f);

            return;
        }

        // 재화 소진
        if (GameManager.instance.useGoods(changeIndexToArray(breadIndex).gold, changeIndexToArray(breadIndex).wheat)) 
        {
            _moreMoney.style.display = DisplayStyle.Flex;
            Invoke("OffMoreMoneyUI", 1f);

            return;
        }

        // 빵 생성 ㄱㄱㄱ 
        bakery.GetComponent<BakeryController>().readyBread(breadIndex);
    }

    BreadData changeIndexToArray(int breadIndex)
    {
        return breadDatas[(breadIndex - 1) / 4, (breadIndex + 3) % 4];
    }

    // 바구니 담기
    public void SetBasket()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int index = SelectedBread[i, j];
                int index_UI = i * 2 + j;

                // 없을때
                if (index == 0) 
                {
                    // ? 넣기
                    _bread[index_UI].style.backgroundImage = new StyleBackground(questImg);
                    _goldText[index_UI].text = "?";
                    _wheatContainer[index_UI].style.display = DisplayStyle.None;
                }
                else
                {
                    _wheatContainer[index_UI].style.display = DisplayStyle.Flex;

                    // 있는거 이미지
                    Sprite newImage = breadDatas[(index-1)/4, (index + 3) % 4].sp;
                    _bread[index_UI].style.backgroundImage = new StyleBackground(newImage);

                    // 얜 재화 텍스트
                    int cost = breadDatas[(index - 1) / 4, (index + 3) % 4].gold;
                    _goldText[index_UI].text = cost.ToString();

                    // 얜 밀 텍스트
                    int wheat = breadDatas[(index - 1) / 4, (index + 3) % 4].wheat;
                    if(wheat == 0)
                    {
                        _wheatContainer[index_UI].style.display = DisplayStyle.None;
                    } 
                    else
                    {
                        _wheatText[index_UI].text = wheat.ToString();
                    }
                }

            }
        }
    }

    public void OnUIActive(ClickEvent evt)
    {
        if (!GameManager.instance.canOpenBakeryUI)
        {
            _tooFar.style.display = DisplayStyle.Flex;
            Invoke("OffTooFarText", 1f);
            return;
        }
         
        SoundManager.Instance.StartSound(0);
        RefreshOven();
        SetBasket(); 

        if (isOpen)
        {
            
            _bakeryContainer.AddToClassList("close-sheet");
        }
        else
        {
            _bakeryContainer.RemoveFromClassList("close-sheet");
            _flagText.style.display = DisplayStyle.None;
        }
        isOpen = !isOpen;
    }

    // ?좉린?먮깘 耳쒕뒓??
    public void RefreshOven()
    {
        // 
        for (int i = 0; i < 3; i++)
        {
            if (SelectedBread[i, 1] == 0) return;

            switch (i)
            {
                case 0:
                    _image_LOR[i].AddToClassList("img-unlock01");
                    break;

                case 1:
                    _image_LOR[i].AddToClassList("img-unlock02");
                    break;

                case 2:
                    _image_LOR[i].AddToClassList("img-unlock03");
                    break;
            }
        }
    }

    // 넘 멀어요 끄기
    void OffTooFarText()
    {
        _tooFar.style.display = DisplayStyle.None;
    }

    // 아직임 끄기
    void OffNotYetUI()
    {
        _notYet.style.display = DisplayStyle.None;
    }

    // 돈 없냐? ㅋㅋ 끄기
    void OffMoreMoneyUI()
    {
        _moreMoney.style.display = DisplayStyle.None;
    }
}
