using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterFlipSync : MonoBehaviourPun, IPunObservable
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 클라이언트에서 flipX 값을 전송합니다.
            stream.SendNext(spriteRenderer.flipX);
        }
        else
        {
            // 원격 클라이언트에서 flipX 값을 수신하고 적용합니다.
            spriteRenderer.flipX = (bool)stream.ReceiveNext();
        }
    }
}
