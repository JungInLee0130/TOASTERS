using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] 
    Texture2D[] cursorTexture;

    Vector2 cursorHotspot;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        int index = GameManager.instance.cursorIndex;
        
        //cursorHotspot = new Vector2(cursorTexture[index].width * 0.5f, cursorTexture[index].height * 0.5f);
        Cursor.SetCursor(cursorTexture[index], Vector2.zero, CursorMode.Auto);
    }
}
