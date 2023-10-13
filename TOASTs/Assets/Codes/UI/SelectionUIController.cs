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

    // ���� ������ �迭
    int[] breadIdinCard = new int[3];

    // �ӽ�
    int tier = 1;

    // �ӽ� id ���� �迭
    int[,] selectedBreadId = new int[3,3];

    // BakeryUIController���� ���� ����
    public BakeryUIController bUICon;

    // Ŭ���� �ѹ���~
    bool plzClickOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        photonview = GetComponent<PhotonView>();

        var root = GetComponent<UIDocument>().rootVisualElement;

        _selectionContainer = root.Q<VisualElement>("SelectionContainer");
        //_card = root.Q<VisualElement>("Card");

        _selectionContainer.style.display = DisplayStyle.None;

        // ī��
        cards = root.Query<VisualElement>("Card").ToList();

        // ��ü�� ����
        Images = root.Query<VisualElement>("Image").ToList();
        Names = root.Query<Label>("Name").ToList();
        Explanations = root.Query<Label>("Explanation").ToList();
        GradText = root.Query<Label>("GradText").ToList();
        WheatInfo = root.Query<VisualElement>("WheatInfo").ToList();
        TimerText = root.Query<Label>("TimerText").ToList();

        // �� ī�忡 �̺�Ʈ �����ʸ� ����մϴ�.
        for (int i = 0; i < cards.Count; i++)
        {
            int cardIndex = i;
            cards[i].RegisterCallback<ClickEvent>(evt => SelectCard(evt, cardIndex));
        }
    }

    public void SelectCardWave()
    {
        // ī�� �������� ��ġ
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

        // �ٱ��Ͽ� ���
        bUICon.SetBasket();
        bUICon.RefreshOven();


        // ��������
        _selectionContainer.AddToClassList("close-sheet");

        // ������
        _selectionContainer.style.display = DisplayStyle.None;
        plzClickOnce = true;
    }

    /*[PunRPC]
    void RPC_SelectCard(int cardIndex)
    {
        Debug.Log(">>> RPC_SelectCard called");

        // �ٱ��Ͽ� ���
        bUICon.SetBasket();
        bUICon.RefreshOven();


        // ��������
        _selectionContainer.AddToClassList("close-sheet");

        // ������
        _selectionContainer.style.display = DisplayStyle.None;
        plzClickOnce = true;
    }*/


    // ������ ī�� ����
    public void SetRandomCard()
    {

        BreadData[,] breadDatas = GameManager.instance.breadDatas;

        HashSet<int> uniqueNumbers = new HashSet<int>();

        // �̸� �����Ѱ� ���� �־����
        int selectedBreadIndex = GameManager.instance.SelectedBread[tier - 1, 0];
        uniqueNumbers.Add(selectedBreadIndex);


        int index = 0;

        while (index < 3)
        {
            int randomNumber = Random.Range(1, 5); // 1���� 4������ ������ ������ �����մϴ�.

            // �̹� �ִ� ���ڸ� �ٽ�
            if (uniqueNumbers.Contains((tier - 1) * 4 + randomNumber)) continue;

            uniqueNumbers.Add((tier - 1) * 4 + randomNumber);


            breadIdinCard[index] = (tier - 1) * 4 + randomNumber;

    
            // ����迭�� ��
            selectedBreadId[tier - 1, index] = breadIdinCard[index];

            // �̹��� ����
            Sprite newImage = breadDatas[tier - 1, randomNumber - 1].sp;
            Images[index].style.backgroundImage = new StyleBackground(newImage);

            // �̸� ����
            string newName = breadDatas[tier - 1, randomNumber-1].name;
            Names[index].text = newName;

            // ���� ����
            string newExplanation = breadDatas[tier - 1, randomNumber-1].effect;
            Explanations[index].text = newExplanation;

            // wheat�� �ʿ����� �Ǵ� 
            if(breadDatas[tier - 1, randomNumber - 1].wheat == 0)
            {
                WheatInfo[index].style.display = DisplayStyle.None;
            }
            else
            {
                WheatInfo[index].style.display = DisplayStyle.Flex;

                // wheatText ����
                string wheatText = breadDatas[tier - 1, randomNumber - 1].wheat.ToString();
                GradText[index * 2].text = wheatText;

            }

            // goldText ����
            string goldText = breadDatas[tier - 1, randomNumber - 1].gold.ToString();
            GradText[index * 2].text = goldText;

            // ���� �ð� ǥ��
            string timeText = breadDatas[tier - 1, randomNumber - 1].seconds.ToString();
            TimerText[index].text = timeText+"��";


            index++;
        }

        tier++;

        plzClickOnce = false;
    }
}
