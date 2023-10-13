using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUI : MonoBehaviour
{
    VisualElement GameOver;
 
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        GameOver = root.Q<VisualElement>("GameOver");

        GameOver.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    public void SetGameOver()
    {
        Debug.Log("gg");
        GameOver.style.display = DisplayStyle.Flex;
    }
}
