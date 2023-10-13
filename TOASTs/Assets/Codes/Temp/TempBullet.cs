/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBullet : MonoBehaviour
{
    public float lifeTime = 1f;
    public float fadeTime = 0.2f;

    SpriteRenderer spriteRenderer;


    void Start()
    {
        //spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeAndDestroy());
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<TempDefaultMob>().GainDamage(10);

            if (!GameManager.instance.isPenetrate)
            {
                // 다시 집어넣기
                BulletPoolManager.instance.ReturnBullet(this);
                // Destroy(gameObject);
            }
        }
    }

    // 추후 pool으로 관리
    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(lifeTime - fadeTime);

        float timer = 0f;
        Color originalColor = spriteRenderer.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        BulletPoolManager.instance.ReturnBullet(this);
    }
}
*/
