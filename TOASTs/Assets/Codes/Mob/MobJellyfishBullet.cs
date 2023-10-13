using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobJellyfishBullet : MonoBehaviour
{
    public float lifeTime = 10f;
    public float birthTime;
    public int bulletDamage = 10;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        birthTime = Time.fixedTime;
    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= birthTime + lifeTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bakery")
        {
            collision.gameObject.GetComponent<BakeryController>().GainDamage(bulletDamage);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
