using Photon.Pun;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    #region Fields
    public Transform character;
    public TextMesh playerName;
    private PhotonView pv;

    #endregion

    #region Methods
    private void Start()
    {
        pv = character.GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;

        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (character != null)
        {
            transform.position = character.position + new Vector3(0f, 1.5f);
        }
    }
    #endregion

}
