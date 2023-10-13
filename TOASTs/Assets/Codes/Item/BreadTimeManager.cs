using Photon.Pun;
using UnityEngine;

public class BreadTimeManager : MonoBehaviourPunCallbacks
{
    public float curTime;
    float makeTime;

    public bool isBaking = false;
    public int isSpawnLen = 0;

    BakeryController bakeryController;
    BreadSpawn breadSpawn;
    //TempPlayerController tempPlayerController;

    // 소요시간 감소 빵의 개수

    void Awake()
    {
        curTime = 0f;
        makeTime = 0f;
        isBaking = false;
        bakeryController = GetComponent<BakeryController>();
        breadSpawn = GetComponent<BreadSpawn>();
        //tempPlayerController = GetComponent<TempPlayerController>();
    }


    void Update()
    {
        // readyBread 꽉찼을때 return으로 최적화 하기

        int readyBreadLen = bakeryController.readyBreads.Count;

        // 준비 배열에 없으면 return
        if (readyBreadLen == 0) return;


        breadSpawn.checkSpawnPoint();


        isSpawnLen = breadSpawn.getIsSpawnLength();

        // 안만들고있으면 setBreadTime
        if (!isBaking)
        {
            if (isSpawnLen < 5)
            {

                SetBreadTime();
            }
            else
            {
                return;
            }
        }

        if (isSpawnLen >= 5) return;

        curTime += Time.deltaTime;

        Debug.Log("curTime : " + curTime);

        // 다 만들면
        if (curTime > makeTime)
        {
            // 다만듬
            isBaking = false;
            
            // 현재시간 초기화
            curTime = 0f;

            //int comBreadsLen = bakeryController.comBreads.Count;

            // 빵 완료 배열에 추가 : 완료 큐에 넣음
            bakeryController.FinishBread();
        }
    }

    /*public void test()
    {
        Debug.Log("this is test");
    }*/

    public void SetBreadTime()
    {
        // 가장 첫번째 bread 가져오기
        BreadData firstBread = bakeryController.GetFirstBread();

        isBaking = true;
        GetComponent<BakeryController>().BakingAnim();


        makeTime = firstBread.seconds;
    }
}
