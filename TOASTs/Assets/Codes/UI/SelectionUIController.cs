using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SelectionUIController : MonoBehaviour
{
    PhotonView photonview;

    VisualElement _selectionContainer;
    //VisualElement _card;

    List<VisualElement> Images;
    List<Label> Names;
    List<Label> Explanations;
    List<Label> GradText;
    List<VisualElement> WheatInfo;
    List<Label> TimerText;

    List<VisualElement> cards;

    // 선택 감지용 배열
    int[] breadIdinCard = new int[3];

    // 임시
    int tier = 1;

    // 임시 id 저장 배열
    int[,] selectedBreadId = new int[3,3];

    // BakeryUIController에서 쓸거 있음
    public BakeryUIController bUICon;

    // 클릭은 한번만~
    bool plzClickOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        photonview = GetComponent<PhotonView>();

        var root = GetComponent<UIDocument>().rootVisualElement;

        _selectionContainer = root.Q<VisualElement>("SelectionContainer");
        //_card = root.Q<VisualElement>("Card");

        _selectionContainer.style.display = DisplayStyle.None;

        // 카드
        cards = root.Query<VisualElement>("Card").ToList();

        // 교체될 구역
        Images = root.Query<VisualElement>("Image").ToList();
        Names = root.Query<Label>("Name").ToList();
        Explanations = root.Query<Label>("Explanation").ToList();
        GradText = root.Query<Label>("GradText").ToList();
        WheatInfo = root.Query<VisualElement>("WheatInfo").ToList();
        TimerText = root.Query<Label>("TimerText").ToList();

        // 각 카드에 이벤트 리스너를 등록합니다.
        for (int i = 0; i < cards.Count; i++)
        {
            int cardIndex = i;
            cards[i].RegisterCallback<ClickEvent>(evt => SelectCard(evt, cardIndex));
        }
    }

    public void SelectCardWave()
    {
        // 카드 랜덤으로 배치
        SetRandomCard();

        _selectionContainer.style.display = DisplayStyle.Flex;

        _selectionContainer.RemoveFromClassList("close-sheet");
    }

    public void SelectCard(ClickEvent evt, int cardIndex)
    {
        if (plzClickOnce) return;
        GameManager.instance.InputSelectionBread(selectedBreadId[tier - 2, cardIndex]);

        //Debug.Log(">>> cardIndex : " + cardIndex);
        //photonview.RPC("RPC_SelectCard", RpcTarget.All, cardIndex);

        // 바구니에 담기
        bUICon.SetBasket();
        bUICon.RefreshOven();


        // 닫으세요
        _selectionContainer.AddToClassList("close-sheet");

        // 끄세요
        _selectionContainer.style.display = DisplayStyle.None;
        plzClickOnce = true;
    }

    /*[PunRPC]
    void RPC_SelectCard(int cardIndex)
    {
        Debug.Log(">>> RPC_SelectCard called");

        // 바구니에 담기
        bUICon.SetBasket();
        bUICon.RefreshOven();


        // 닫으세요
        _selectionContainer.AddToClassList("close-sheet");

        // 끄세요
        _selectionContainer.style.display = DisplayStyle.None;
        plzClickOnce = true;
    }*/


    // 보여줄 카드 세팅
    public void SetRandomCard()
    {

        BreadData[,] breadDatas = GameManager.instance.breadDatas;

        HashSet<int> uniqueNumbers = new HashSet<int>();

        // 미리 선택한거 먼저 넣어놓기
        int selectedBreadIndex = GameManager.instance.SelectedBread[tier - 1, 0];
        uniqueNumbers.Add(selectedBreadIndex);


        int index = 0;

        while (index < 3)
        {
            int randomNumber = Random.Range(1, 5); // 1부터 4까지의 랜덤한 정수를 생성합니다.

            // 이미 있는 숫자면 다시
            if (uniqueNumbers.Contains((tier - 1) * 4 + randomNumber)) continue;

            uniqueNumbers.Add((tier - 1) * 4 + randomNumber);


            breadIdinCard[index] = (tier - 1) * 4 + randomNumber;

    
            // 저장배열에 슥
            selectedBreadId[tier - 1, index] = breadIdinCard[index];

            // 이미지 변경
            Sprite newImage = breadDatas[tier - 1, randomNumber - 1].sp;
            Images[index].style.backgroundImage = new StyleBackground(newImage);

            // 이름 변경
            string newName = breadDatas[tier - 1, randomNumber-1].name;
            Names[index].text = newName;

            // 설명 변경
            string newExplanation = breadDatas[tier - 1, randomNumber-1].effect;
            Explanations[index].text = newExplanation;

            // wheat가 필요한지 판단 
            if(breadDatas[tier - 1, randomNumber - 1].wheat == 0)
            {
                WheatInfo[index].style.display = DisplayStyle.None;
            }
            else
            {
                WheatInfo[index].style.display = DisplayStyle.Flex;

                // wheatText 관리
                string wheatText = breadDatas[tier - 1, randomNumber - 1].wheat.ToString();
                GradText[index * 2].text = wheatText;

            }

            // goldText 관리
            string goldText = breadDatas[tier - 1, randomNumber - 1].gold.ToString();
            GradText[index * 2].text = goldText;

            // 생산 시간 표시
            string timeText = breadDatas[tier - 1, randomNumber - 1].seconds.ToString();
            TimerText[index].text = timeText+"초";


            index++;
        }

        tier++;

        plzClickOnce = false;
    }
}
