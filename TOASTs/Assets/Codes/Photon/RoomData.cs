using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviourPunCallbacks
{
    #region Fields
    private Text roomInfoText;
    private RoomInfo roomInfo;

    public RoomInfo RoomInfo
    {
        get
        {
            return roomInfo;
        }
        set
        {
            roomInfo = value;
            // 방은 "{방 이름} {방 인원 수} / {풀방 인원 수}}"로 표시
            roomInfoText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        }
    }

    #endregion
    #region Methods

    /// <summary>
    /// roomInfoText는 room button 하위에 text에 연결
    /// </summary>
    private void Awake()
    {
        roomInfoText = GetComponentInChildren<Text>();
    }

    #endregion
}





