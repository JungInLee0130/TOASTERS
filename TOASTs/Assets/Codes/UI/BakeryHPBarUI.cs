using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BakeryHPBarUI : MonoBehaviour
{
    #region Fields
    public Transform bakery;
    public Slider slider; // 체력바 슬라이더

    //public TempEntity entity;
    #endregion

    #region
    private void Awake()
    {
        transform.position = bakery.position + new Vector3(0,90,0);
    }
    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
/*
        fill.color = gradient.Evaluate(1f);*/
    }

    public void SetHealth(float health)
    {
        slider.value = health;
/*
        fill.color = gradient.Evaluate(slider.normalizedValue);*/
    }
    #endregion
}