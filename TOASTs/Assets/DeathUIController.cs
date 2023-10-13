using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
public class DeathUIController : MonoBehaviour
{
    VisualElement _innerContainer;
    VisualElement _deathImg;
    Label _count;

    // Start is called before the first frame update
    void Start()
    {

        var root = GetComponent<UIDocument>().rootVisualElement;
        _innerContainer = root.Q<VisualElement>("InnerContainer");
        _deathImg = root.Q<VisualElement>("Img");
        _count = root.Q<Label>("Count");

        _innerContainer.style.display = DisplayStyle.None;
    }

    // 플레이어 사망
    public void PlayerDeath(int count)
    {
        _innerContainer.style.display = DisplayStyle.Flex;

        StartCoroutine(PlayerDeathCount(count));
    }

    IEnumerator PlayerDeathCount(int count)
    {
        while (count > 0)
        {
            _count.text = count.ToString();
            yield return new WaitForSeconds(1f); // 1초 대기
            count--;
        }

        // 카운트다운이 끝나면 UI 숨기기
        OffText();
    }

    void OffText()
    {
        _innerContainer.style.display = DisplayStyle.None;
    }
}
