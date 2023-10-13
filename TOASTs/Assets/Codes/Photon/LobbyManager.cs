using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Fields
    public Scrollbar scrollbar; // 스크롤바
    public RectTransform content; // 스크롤 바에 있는 콘텐츠
    public GameObject roomDataPrefab; // 방 프리팹
    public InputField roomNameInput; // 방 만들기에 사용하는 입력 필드
    public Button createButton; // 방 만들기 버튼
    public Button joinButton; // 방 이름으로 들어가기 버튼
    public GameObject createRoomFailedPanel; // 이미 있는 방을 만들려고 할 때 띄워지는 패널
    public GameObject joinRoomFailedPanel; // 방에 입장 실패했을 때 띄워지는 패널
    public GameObject gameDoesNotExistPanel; // 해당 방이 존재하지 않을 때 띄워지는 패널
    public GameObject gameAlreadyStartedPanel; // 해당 방이 이미 플레이중일 때 띄워지는 패널

    public Dictionary<string, GameObject> currentRoomList = new(); // 띄우려고 하는 방 목록


    #endregion

    #region Methods

    private void Awake()
    {
        createRoomFailedPanel.SetActive(false);
        joinRoomFailedPanel.SetActive(false);
        gameDoesNotExistPanel.SetActive(false);
        gameAlreadyStartedPanel.SetActive(false);
    }


    /// <summary>
    /// 로비 접속 후 실행되는 콜백 함수
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("PUN Tutorials: 로비 접속");
    }

    /// <summary>
    /// joinLobby가 실행될 때마다 실행되는 콜백 함수
    /// 방 목록을 새로고침한다
    /// </summary>
    /// <param name="roomList">현재 PUN이 관리하는 방 목록</param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방 새로고침
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// 방 목록을 새로고침 하는 함수
    /// </summary>
    /// <param name="roomList">현재 PUN이 관리하는 방 목록</param>
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        GameObject curRoom = null;
        foreach (RoomInfo room in roomList)
        {
            // 현재 방 리스트가 관리하고 있지 않는 방이라면
            if (room.RemovedFromList)
            {
                // 방 리스트에서 삭제
                currentRoomList.TryGetValue(room.Name, out curRoom);
                Destroy(curRoom);
                currentRoomList.Remove(room.Name);
            }
            // 아니면 리스트에 저장(이미 있으면 상관 없음)
            else
            {
                // 새로 만들어진 방이면 생성
                if (!currentRoomList.ContainsKey(room.Name))
                {
                    curRoom = Instantiate(roomDataPrefab, content);
                    curRoom.GetComponent<RoomData>().RoomInfo = room;
                    // 버튼 누르면 해당 방으로 들어가게
                    curRoom.GetComponent<Button>().onClick.AddListener(
                        () => {
                            JoinRoom(room.Name);
                        });
                    currentRoomList.Add(room.Name, curRoom);
                }
                // 아니면 정보 수정
                else
                {
                    currentRoomList.TryGetValue(room.Name, out curRoom);
                    curRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }

        // 현재 방 수
        Debug.Log($"현재 서버에 열린 방 수: {PhotonNetwork.CountOfRooms}");
    }
/*
    private void UpdateRoomList()
    {
        foreach (RoomData room in currentRoomList)
        {
            // 현재 방 리스트가 관리하고 있지 않는 방이라면
            if (room.RemovedFromList)
            {
                // 방 리스트에서 삭제
                currentRoomList.TryGetValue(room.Name, out curRoom);
                Destroy(curRoom);
                currentRoomList.Remove(room.Name);
            }
            // 아니면 리스트에 저장(이미 있으면 상관 없음)
            else
            {
                // 새로 만들어진 방이면 생성
                if (!currentRoomList.ContainsKey(room.Name))
                {
                    curRoom = Instantiate(roomDataPrefab, content);
                    curRoom.GetComponent<RoomData>().RoomInfo = room;
                    // 버튼 누르면 해당 방으로 들어가게
                    curRoom.GetComponent<Button>().onClick.AddListener(
                        () => {
                            JoinRoom(room.Name);
                        });
                    currentRoomList.Add(room.Name, curRoom);
                }
                // 아니면 정보 수정
                else
                {
                    currentRoomList.TryGetValue(room.Name, out curRoom);
                    curRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }*/

    public void CreateRoom()
    {
        // 입력한 값 동기화 
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("Pun Tutorials: 방 이름을 입력해주세요!");
            return;
        }

        // 옵션 설정은 일단 간단하게 인원 설정만
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 4
        };

        // 방 생성 후 입장
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    /// <summary>
    /// 새로운 방을 만들고 나서 실행되는 콜백함수
    /// </summary>
    public override void OnCreatedRoom()
    {

        // 방장을 기준으로 scene 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        // scene 이동
        SceneManager.LoadScene("Room");

        // 방 목록에 띄울 프리팹 생성
        Instantiate(roomDataPrefab, content);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        joinRoomFailedPanel.SetActive(true);
        Invoke("SetActiveFalse", 1);
    }

    /// <summary>
    /// 방 입장
    /// </summary>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 방 이름으로 들어가기 위해 사용할 메소드(override)
    /// </summary>
    public void JoinRoom()
    {
        string roomName = roomNameInput.text;
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 방 입장 후 실행되는 콜백함수 
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log($"Pun Tutorials: 플레이어 방 입장: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Pun Tutorials: 입장한 플레이어 이름: {PhotonNetwork.NickName}");
        Debug.Log($"Pun Tutorials: 입장한 인원 수: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}");

        // 새로운 플레이어 객체 생성
        // PlayerCreate.instance.isConnect = true;

        // 방장을 기준으로 scene 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        // scene 이동
        SceneManager.LoadScene("Room");
    }

    /// <summary>
    /// 방 입장에 실패하면 로그 출력
    /// </summary>
    /// <param name="returnCode">실패사유</param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        switch (returnCode)
        {
            case ErrorCode.GameFull: // 풀방일 때
                joinRoomFailedPanel.SetActive(true);
                Invoke(nameof(SetActiveFalse), 1);
                break;
            case ErrorCode.GameIdAlreadyExists: // 이미 있는 방을 만들려고 할 때
                createRoomFailedPanel.SetActive(true);
                Invoke(nameof(SetActiveFalse), 1);
                break;
            case ErrorCode.GameDoesNotExist: // 존재하지 않는 방에 접근할 때
                gameDoesNotExistPanel.SetActive(true);
                Invoke(nameof(SetActiveFalse), 1);
                break;
            case ErrorCode.GameClosed: // 이미 플레이 중인 방일 때
                gameAlreadyStartedPanel.SetActive(true);
                Invoke(nameof(SetActiveFalse), 1); 
                break;
        }
    }

    /// <summary>
    /// 방 만들기 / 입장하기에 실패했을 때 표시해주는 패널
    /// </summary>
    public void SetActiveFalse()
    {
        createRoomFailedPanel.SetActive(false);
        joinRoomFailedPanel.SetActive(false);
        gameDoesNotExistPanel.SetActive(false);
        gameAlreadyStartedPanel.SetActive(false);
    }

    /// <summary>
    /// 로비 나가기 
    /// </summary>
    public void ExitLobby()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("Launcher");
        PhotonNetwork.Disconnect();
        Debug.Log(">>>> 로비에서 런쳐로 이동");
    }

    /// <summary>
    /// 로비를 나가면 실행되는 콜백 함수
    /// </summary>
    /// <param name="cause">접속 종료 원인</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        currentRoomList.Clear();
    }

    #endregion
}


