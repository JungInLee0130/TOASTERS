using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Serializable Fields
    [SerializeField]
    public TMP_InputField idInput;

    [SerializeField]
    public Button loginButton;

    [SerializeField]
    public Button exitButton;

    #endregion

    #region Fields
    private string id;


    [Tooltip("게임 버전")]
    private readonly string gameVersion = "1";


    #endregion

    #region Launcher Methods
    /// <summary>
    /// 씬 불러오기 전에, 입력창에 들어오는 값과 사용할 player name을 동기화
    /// </summary>
    private void Awake()
    {
        id = idInput.GetComponent<TMP_InputField>().text;
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Connect();
        }
    }

    /// <summary>
    /// play 버튼 입력 후 마스터 서버(로비)로 연결 
    /// </summary>
    public void Connect()
    {
        id = idInput.GetComponent<TMP_InputField>().text;
        // 입력받은 값이 있는 경우에만 연결 해야 하니까 입력 값이 없으면 입력하라고 알림
        if (id != string.Empty && !CheckIfPlayerNameExists(id))
        {
            // 갱신
            PhotonNetwork.NickName = id;
            Debug.Log($"PUN Tutorials: 마스터 서버로 이동, 버전 v{gameVersion}");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("PUN Tutorials: 입력받은 값이 없습니다!");
        }
    }


    /// <summary>
    /// 마스터 서버(로비)에 접속하면 실행되는 콜백함수
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Tutorials: 마스터서버 접속");
        Debug.Log($"현재 접속한 사람: {PhotonNetwork.NickName}");

        PhotonNetwork.JoinLobby();

        // 씬 이동
        SceneManager.LoadScene("Lobby");
    }

    /// <summary>
    /// 연결 종료시 실행되는 콜백함수
    /// </summary>
    /// <param name="cause"> 연결 종료 원인 </param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 다시 닉네임 입력창 호출
        Debug.LogWarningFormat("PUN Tutorials: 연결 해제, 사유) {0}", cause);
    }

    /// <summary>
    /// 중복된 닉네임 체크하는 함수
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool CheckIfPlayerNameExists(string name)
    {
        Player[] list = PhotonNetwork.PlayerList;

        // 이미 있는 닉네임인지 플레이어 리스트 탐색
        foreach (Player p in list){
            // 있으면 중복 있음
            if (p.NickName == name)
            {
                return true;
            }
        }

        // 없는 닉네임이면 중복 안됨
        return false;
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
    }
    #endregion
}
