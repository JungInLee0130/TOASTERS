using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
    public float lifeTime = 0.75f;
    public float fadeTime = 0.2f;
    PhotonView pv;
    public int damage = 0;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        // pv.ismine 없어도 됨

        Invoke("DisabledBullet", lifeTime);


        //StartCoroutine(FadeAndDestroy());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (true /*pv.IsMine*/)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<DefaultMob>().GainDamage(damage);

                if (!GameManager.instance.isPenetrate)
                {
                    // 다시 집어넣기
                    // BulletPoolManagerReal.Instance.ReturnBullet(this);
                    PhotonNetwork.Destroy(gameObject);
                }
            }

        }
    }

    void DisabledBullet()
    {
        if (!pv.IsMine) return;
        CancelInvoke("DisabledBullet");
        // BulletPoolManagerReal.Instance.ReturnBullet(this); 
        PhotonNetwork.Destroy(this.gameObject);
    }



    
    /*// 추후 pool으로 관리
    IEnumerator FadeAndDestroy()
    {
        if (true *//*pv.IsMine*//*)
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

            BulletPoolManagerReal.Instance.ReturnBullet(this);

            *//*if (PhotonNetwork.IsConnected && pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }*//*

        }
    }*/
}
