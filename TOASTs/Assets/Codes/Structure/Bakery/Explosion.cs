using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnExplosion()
    {
        anim.SetTrigger("Explosion");
    }

    public void GameOver()
    {
        GameManager.instance.GameOver();
    }

}
