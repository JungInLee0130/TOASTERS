using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInLayer : MonoBehaviour
{
    SpriteRenderer spriter;
    public int privateOrder;

    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spriter.sortingOrder = (int)(transform.position.y * -100) + privateOrder;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        spriter.sortingOrder = (int)(transform.position.y * -100) + privateOrder;
    }
}
