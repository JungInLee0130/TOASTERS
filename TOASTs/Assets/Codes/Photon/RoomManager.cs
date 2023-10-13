using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region Fields
    public Button startGameButton;
    public Button leaveRoomButton;

    /*// chat
    public GameObject m_Content;
    public TMP_InputField m_inputField;
    PhotonView photonview;
    GameObject m_ContentText;
    ScrollRect scroll_rect;
    static bool chk = false;*/

    // 싱글톤
    public static RoomManager instance;

    // 방 이름
    public TextMeshProUGUI roomNameText;

    #endregion

    #region Methods

    private void Awake()
    {
        instance = this;
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // 방 이름 설정
        if (PhotonNetwork.InRoom)
        {
            string roomName = PhotonNetwork.CurrentRoom.Name;
            roomNameText.text = "방 이름  \n" + roomName;
        }
        else
        {
            roomNameText.text = "Not in a room";
        }
    }


    private void Update()
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    /// <summary>
    /// 방 나가기
    /// </summary>
    public void ExitRoom()
    {
        // 방 나가기
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 방을 나가면 실행되는 콜백 함수.
    /// 마스터 서버로 이동된다
    /// </summary>
    public override void OnLeftRoom()
    {
        // 일단 마스터 서버로 온 상태
        Debug.Log("PUN Tutorials: 방 나가기, 마스터 서버로 이동");

        // 씬 이동
        SceneManager.LoadScene("Lobby");
    }

    public void StartGame()
    {
        Debug.Log("PUN Tutorials: 게임 시작");
        PhotonNetwork.LoadLevel("GameScene");

        // 시작하고 나서는 이 방 못 들어옴
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"새로운 플레이어 입장:{newPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}");
        //photonview.RPC("RPC_Chat", RpcTarget.All, ">"+ PhotonNetwork.LocalPlayer.NickName+"님이 방에 입장했습니다. ");

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}");
    }

    /*public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(">>> 메시지 보내기");
            chk = true;
            string strMessage = PhotonNetwork.NickName + " : " + m_inputField.text;

            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            m_inputField.text = "";
        }
    }
    public void ChangeChkToFalse()
    {
        chk = false;
    }

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);

        goText.GetComponent<TextMeshProUGUI>().text = message; // 
        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    public void ScrollDown()
    {
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        AddChatMessage(message);
    }
    //*/

    #endregion
}
