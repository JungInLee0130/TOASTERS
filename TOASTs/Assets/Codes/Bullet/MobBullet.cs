/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float fadeTime = 0.2f;
    public int bulletDamage = 30;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //StartCoroutine(FadeAndDestroy());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GainDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Tower")
        {

        }
        else if (collision.gameObject.tag == "Bakery")
        {
            collision.gameObject.GetComponent<BakeryController>().GainDamage(bulletDamage);
            Destroy(gameObject);
        }
    }

    // ���� pool���� ����

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

        Destroy(gameObject);
    }

}*/


using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float fadeTime = 0.2f;
    public int bulletDamage = 30;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //StartCoroutine(FadeAndDestroy());
        Invoke("DisabledBullet", lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("����");
            collision.gameObject.GetComponent<PlayerController>().GainDamage(bulletDamage);

            // �ٽ� ����ֱ�
            PhotonNetwork.Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Tower")
        {

        }
        else if (collision.gameObject.tag == "Bakery")
        {
            collision.gameObject.GetComponent<BakeryController>().GainDamage(bulletDamage);


            // �ٽ� ����ֱ�
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void DisabledBullet()
    {
        CancelInvoke("DisabledBullet");
        PhotonNetwork.Destroy(gameObject);
    }

    // ���� pool���� ����

    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(lifeTime - fadeTime);

        float timer = 0f;
        Color originalColor = spriteRenderer.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            //float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            //spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            PhotonNetwork.Destroy(gameObject);
            yield return null;
        }

        // �ٽ� ����ֱ�
        //BulletPoolManagerReal.Instance.ReturnMobBullet(this);
    }
}
