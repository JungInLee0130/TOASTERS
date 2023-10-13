using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundUIController : MonoBehaviour
{
    Label _roundText;
    int round = 0;

    void Start()
    {
       // round = GameManager.instance.

        var root = GetComponent<UIDocument>().rootVisualElement;
        _roundText = root.Q<Label>("Round");

        SetRound(0);
    }

    void Update()
    {
        //round = GameManager.instance.waveCount;
    }

    // 라운드 업데이트
    public void SetRound(int round)
    {
        _roundText.text = round + " 라운드";
    }
}
