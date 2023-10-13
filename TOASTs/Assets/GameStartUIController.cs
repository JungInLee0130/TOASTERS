using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameStartUIController : MonoBehaviour
{
    VisualElement TextContainer;
    Label _RoundStartText;
    Label _Countdown;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        TextContainer = root.Q<VisualElement>("TextContainer");
        _RoundStartText = root.Q<Label>("RoundStartText");
        _Countdown = root.Q<Label>("Countdown");
    }

    public void SetRoundStart(int count)
    {
        // UI�� Ȱ��ȭ�ϰ� ���� ���� �ؽ�Ʈ�� ǥ��
        TextContainer.style.display = DisplayStyle.Flex;
        SetCountdown(count);
    }

    void SetCountdown(int count)
    {
        StartCoroutine(CountdownCoroutine(count));
    }

    IEnumerator CountdownCoroutine(int count)
    {
        while (count > 0)
        {
            _Countdown.text = count.ToString();
            yield return new WaitForSeconds(1f); // 1�� ���
            count--;
            GameManager.instance.currentRestTime++;
        }

        // ī��Ʈ�ٿ��� ������ UI �����
        OffText();
    }

    void OffText()
    {
        TextContainer.style.display = DisplayStyle.None;
    }
}
