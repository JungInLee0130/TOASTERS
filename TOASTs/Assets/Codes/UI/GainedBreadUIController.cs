using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaindBreadUIController : MonoBehaviour
{
    public Image filter;
    public Image breadImg;
  

    private void Start()
    {
        
    }

    public void changeImg(Sprite img)
    {
        breadImg.sprite = img;
    }

    public void UpdateBuff(float duration)
    {
        StartCoroutine(UpdateFillAmount(duration));
    }

    private IEnumerator UpdateFillAmount(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float fillAmount = Mathf.Lerp(0, 1, elapsedTime / duration);
            filter.fillAmount = fillAmount;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        filter.fillAmount = 1;

        // ³¡³ª¸é ¤²¤²
        Destroy(gameObject);
    }
}
