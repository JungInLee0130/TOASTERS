using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sounds;
    public GameObject[] audios;
    // buttonSound, winSound, loseSound, shootSound
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get 
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartSound(int idx)
    {
        Debug.Log("사운드 호출");
        audios[idx].GetComponent<AudioSource>().PlayOneShot(sounds[idx]);
    }
}
