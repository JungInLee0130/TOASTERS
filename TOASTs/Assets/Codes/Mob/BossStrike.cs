using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStrike : MonoBehaviour
{
    public float lifeTime = 3f;
    public float birthTime;
    public int bulletDamage = 30;
    List<GameObject> hits = new List<GameObject>();

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
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool hit = false;
        if (!hits.Contains(collision.gameObject))
        {
            if (collision.gameObject.tag == "Player")
            {
                hits.Add(collision.gameObject);
                collision.gameObject.GetComponent<PlayerController>().GainDamage(bulletDamage);
            }
            //else if (collision.gameObject.tag == "Bakery")
            //{
            //    hits.Add(collision.gameObject);
            //    collision.gameObject.GetComponent<BakeryController>().GainDamage(bulletDamage);
            //}
        }

    }
}
