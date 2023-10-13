using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TempEffect : MonoBehaviour
{
    Animator anim;
    //PhotonView photonview;
    // Start is called before the first frame update
    void OnEnable()
    {
        //photonview = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }
    [PunRPC]
    void DeleteEffect()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            //PhotonNetwork.Destroy(gameObject);
            Destroy(gameObject);
            //photonview.RPC(nameof(DeleteEffect), RpcTarget.MasterClient);
        }
    }

}
