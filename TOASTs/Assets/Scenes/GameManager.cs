using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PhotonView photonview;

    #region Scripts
    MobSpawner spawner; // 몹 만들기
    BakeryController bakeryController; // 빵집 상황 전달
    #endregion

    #region Fields
    public static GameManager instance;

    public List<GameObject> currentWave = new List<GameObject>();
    public int currentWaveBudgetSum = 0;

    public float timer;

    public Text timerUI; // 타이머 시간 표시
    public Text waveCountUI; // 웨이브 표시
    public Text currentWaveStatusUI; // 게임 진행 상황 표시

    public Button RetryButton; // 씬 리셋 버튼
    public Button EndButton; // 게임 종료 버튼

    float lastWaveEndTime; // 마지막 웨이브 종료 시간
    float restTime = 20f; // 웨이브 사이 쉬는 시간
    float endRestTime; // 쉬는 시간 종료 시간
    public float currentRestTime; // 진행된 정비시간

    public int cursorIndex = 0;
    // 선택된 빵 보관 (행 : 티어, 열 : 선택된거), 기본값 0인데 0이면 빈 빵
    public int[,] SelectedBread = new int[3, 2];
    int selectionId = 0;

    // int destroyMobCount = 0;
    // int numberOfCurrentMobs;
    public int waveCount = 1; // 현재 웨이브
    public int lastWave = 8;

    private bool isRestTime = true; // 정비시간인지 게임플레이 중인지 구분하는 boolean 변수
    private bool isSpawned = true; // 몹이 스폰 됐는지 안됐는지 확인하는 boolean 변수

    bool restTimeInit = true;

    //public PoolManager pool;
    public GameObject Bakery; // 빵집 데이터 받아오기
    public BreadData[,] breadDatas;

    // UI
    public GameUIController gameUIController;
    public GameOverUI gameOverUI;
    public VictoryUIController victoryUIController;
    public GameStartUIController gameStartUIController;
    public RoundUIController roundUIController;
    public DeathUIController deathUIController;
    public BakeryUIController bakeryUIController;
    public BuffUIController buffUIController;
    public SelectionUIController selectionUIController;
    public bool onceRoundStartUIOpen = true;
    public bool canOpenBakeryUI = false;
    public bool canOpenBakeryUIShutDown = false;

    // 재화 관리
    public int gold;
    public int wheat;

    // 총알 관통
    public bool isPenetrate;

    bool waveStarted = false;

    #endregion
        
    #region Methods
    private void Awake()
    {
        //instance = this;
        if (instance == null) instance = FindObjectOfType<GameManager>();
        spawner = GetComponent<MobSpawner>();
        bakeryController = GameObject.Find("Bakery").GetComponent<BakeryController>();
        timer = 0f;
        lastWaveEndTime = 0f;
    }

    void Start()
    {
        photonview = GetComponent<PhotonView>();

        breadDatas = Bakery.GetComponent<Breads>().breadDatas;
        // 시작 시 랜덤으로 정해지는 빵(티어 별로 슉슉)
        for (int i = 0; i < 3; i++)
        {
            SelectedBread[i, 0] = i * 4 + Random.Range(1, 5);
        }

        //timerUI.text = $"{currentRestTime} / {endRestTime}";
        //waveCountUI.text = $"다음 웨이브를 준비하세요!";
        //currentWaveStatusUI.text = "";
    }

    void Update()
    {
        ChangeCursor();

        timer += Time.deltaTime;

        // 막라운드 전까지 종료이자 시작조건
        // int budgetSum = spawner.budgets[waveCount] + spawner.budgetBonus[waveCount] * (PhotonNetwork.CountOfPlayersInRooms - 1);
        // Debug.Log($">>>> budget left : {budgetSum - currentWaveBudgetSum}");
        if (PhotonNetwork.IsMasterClient && currentWave.Count <= 0 && isSpawned)
        {
            // 만약 마지막 라운드였다면 종료
            if (waveCount >= lastWave)
            {
                photonview.RPC(nameof(RPC_Victory), RpcTarget.All);
                return;
            }

            // 빵 선택 라운드면 선택하기
            if ((waveCount == 0 || waveCount == 2 || waveCount == 4))
            {
                Invoke("WaitAMinute", 2.0f);
            }

            // 웨이브 끝나면서 필요한 변수 초기화
            photonview.RPC("RPC_BakeryHeal", RpcTarget.All); // 빵집 체력 올리기
            isSpawned = false;
            waveStarted = true;
            currentWaveBudgetSum = 0;
            currentWave.Clear();
            photonview.RPC("RPC_NextWave", RpcTarget.All);
        }
        
        if (PhotonNetwork.IsMasterClient && waveStarted && timer >= endRestTime && !isSpawned)
        {
            spawner.SpawnRandomObject();
            isSpawned = true;
        }
        
/*        // 웨이브 중간에 정비 시간이면
        if (isRestTime)
        {
            // 정비시간 타이머 on
            RestTimeSet();

            // 만약 시간이 지나면 다음 웨이브 실행
            if (PhotonNetwork.IsMasterClient && currentRestTime > endRestTime)
            {
                photonview.RPC("RPC_NextWave", RpcTarget.All);
            }

            // 라운드마다 시간 째깍째깍
            if (onceRoundStartUIOpen)
            {
                gameStartUIController.SetRoundStart((int)endRestTime);
                onceRoundStartUIOpen = false;

                // 카드 선택하는 웨이브
                if ((waveCount == 0 || waveCount == 2 || waveCount == 4) & PhotonNetwork.IsMasterClient)
                {
                    Invoke("WaitAMinute", 2.0f);
                }
            }
        }
        // 웨이브 시간이면
        else
        {
            // 없으면 생성
            if (!isSpawned)
            {
                spawner.SpawnRandomObject();
                isSpawned = true;
            }

            // 남은 몹수
            numberOfCurrentMobs = currentWave.Count;

            // 웨이브 종료 조건을 만족하면
            if (numberOfCurrentMobs == 0 && PhotonNetwork.IsMasterClient)
            {
                // 웨이브 종료, 쉬는 시간 표시
                photonview.RPC("RPC_WaveOver", RpcTarget.All);

                // rpc -> isRestTime = true;

                if (!isRestTime)
                {
                    photonview.RPC("RPC_RestTime", RpcTarget.All);
                }

                onceRoundStartUIOpen = true;
                //waveCountUI.text = $"다음 웨이브를 준비하세요!";
                
                // 몹 리스폰 여부 초기화
                isSpawned = false;

                // 킬 수 초기화
                destroyMobCount = 0;

                // 웨이브 모두 마치면 승리조건 만족
                if (waveCount >= lastWave)
                {
                    // 승리조건에 추가
                    Debug.Log("게임 종료!");
                    EndGame();
                }
            }
            // 아니면 계속 웨이브 중이니까 표시
            else
            {
                //waveCountUI.text = $"wave {waveCount}";
            }
        }*/
    }

    [PunRPC]
    void RPC_NextWave()
    {
        // Debug.Log(">>> RPC_NextWave called!!!");
        lastWaveEndTime = timer;
        currentRestTime = lastWaveEndTime;
        endRestTime = lastWaveEndTime + restTime;
        waveCount++; // 다음 웨이브 라운드 표시
        gameStartUIController.SetRoundStart((int)restTime);
        onceRoundStartUIOpen = false;

        //UI에 round +1
        roundUIController.SetRound(waveCount);
    }
    
    [PunRPC]
    void RPC_BakeryHeal()
    {
        // Debug.Log(">>> RPC_RestTime called!!!");
        bakeryController.entity.Heal(150); // 빵집 피 채우기
        // isRestTime = true;
        // restTimeInit = false; 
    }

        private void ChangeCursor()
    {
        // 마우스 위치에 Ray로 오브젝트 검사
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            // 마우스 hover된 오브젝트의 tag를 파악
            switch (hit.collider.tag)
            {
                case "Character":
                    cursorIndex = 1;
                    break;
                case "Enemy":
                    cursorIndex = 2;
                    break;
                case "Structure":
                    cursorIndex = 3;
                    break;
                case "Plant":
                    cursorIndex = 4;
                    break;
                case "Mineral":
                    cursorIndex = 5;
                    break;
                default:
                    cursorIndex = 0;
                    break;
            }
        }
        else
        {
            cursorIndex = 0;
        }
    }

    public void InputSelectionBread(int breadId)
    {
        SelectedBread[selectionId, 1] = breadId;
        selectionId++;  // 나중에 바꿀 수 있으면 변환
    }

    // 재화 사용 함수 > 포톤포톤포포톤
    public bool useGoods(int cost, int wheatcost)
    {
        // 비용이 더 나가면 아웃
        if (cost > gold || wheatcost > wheat)
        {
            return true;
        }

        // 비용 차출
        photonview.RPC("RPC_UseGold", RpcTarget.All, cost);
        photonview.RPC("RPC_UseWheat", RpcTarget.All, wheatcost);


        return false;
    }

    [PunRPC]
    void RPC_UseWheat(int wheatcost)
    {
        wheat -= wheatcost;
    }

    [PunRPC]
    void RPC_UseGold(int cost)
    {
        gold -= cost;
    }

    /// <summary>
    /// 타이머 메소드
    /// </summary>
    public void RestTimeSet()
    {
        currentRestTime += Time.deltaTime;
    }

    /// <summary>
    /// 게임 승리 조건
    /// </summary>
    public void Victory()
    {
        // 승리표시
        victoryUIController.SetVictory();
        SoundManager.Instance.StartSound(1);
        // 5초 후 종료
        Invoke("EndGame", 5);
    }

    /// <summary>
    /// 게임 패배 조건
    /// 게임을 종료시킨다
    /// </summary>
    public void GameOver()
    {
        // 패배 ui 호출
        gameOverUI.SetGameOver();
        SoundManager.Instance.StartSound(2);
        // 5초 후 종료
        Invoke("EndGame", 5);
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Room");
        }
    }

    void WaitAMinute()
    {
        // selectionUIController.SelectCardWave();
        photonview.RPC(nameof(RPC_SelectCard), RpcTarget.All);
    }

    [PunRPC]
    void RPC_SelectCard()
    {
        selectionUIController.SelectCardWave();
    }

    [PunRPC]
    void RPC_Victory()
    {
        victoryUIController.SetVictory();
    }

    public void UpdateGold(int gold1)
    {
        gold = gold1;
    }

    #endregion
}
