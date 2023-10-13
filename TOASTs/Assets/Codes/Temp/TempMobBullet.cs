/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMobBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float fadeTime = 0.2f;
    public int bulletDamage = 30;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeAndDestroy());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("명중");
            collision.gameObject.GetComponent<TempPlayerController>().GainDamage(bulletDamage);
 
            // 다시 집어넣기
            BulletPoolManager.instance.ReturnBullet(this);
        }
        else if (collision.gameObject.tag == "Tower")
        {

        }
        else if (collision.gameObject.tag == "Bakery")
        {
            collision.gameObject.GetComponent<BakeryController>().GainDamage(bulletDamage);


            // 다시 집어넣기
            BulletPoolManager.instance.ReturnBullet(this);
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

        // 다시 집어넣기
        BulletPoolManager.instance.ReturnBullet(this);
    }
}
*/