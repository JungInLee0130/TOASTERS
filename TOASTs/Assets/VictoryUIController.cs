using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VictoryUIController : MonoBehaviour
{
    VisualElement _victory;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _victory = root.Q<VisualElement>("Victory");

        _victory.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    public void SetVictory()
    {
        _victory.style.display = DisplayStyle.Flex;
    }
}
