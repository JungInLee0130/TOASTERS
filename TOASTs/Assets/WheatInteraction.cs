using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WheatInteraction : MonoBehaviour
{
    Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Enter");

        if (collision.gameObject.CompareTag("Player"))
        {
            canvas.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger Exit");

        if (collision.gameObject.CompareTag("Player"))
        {
            canvas.enabled = false;
        }
    }
}
