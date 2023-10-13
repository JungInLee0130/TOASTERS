using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MobSpawner : MonoBehaviour
{
    #region Fields
        public GameObject[] mobs; // 0: 근거리, 1: 원거리, 2: 크로와상, 3: 보스
        public Transform[] spawnPoints; // 스폰 위치
        public int[] budgets = { 0, 75, 100, 125, 160, 200, 250, 350, 100 };//{2,3,5,7,10,14,20};
        public int[] budgetBonus = { 0, 60, 80, 100, 128, 160, 200, 280, 0 }; // 인원당 보너스
        int[] squadSize = { 0, 1, 2, 3, 4, 4, 5, 5, 1 }; // 라운드 별 그룹 사이즈
    #endregion


    #region Methods
    void Start()
    {
    }

        public void SpawnRandomObject()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //Debug.Log(">>> wave : " + GameManager.instance.waveCount);
        int waveNum = GameManager.instance.waveCount; // 현재 웨이브
        int budget = budgets[waveNum];
        bool madeJellyfish = waveNum < 3 || PhotonNetwork.PlayerList.Length == 1; // 해파리는 무조건 하나씩 나오며, 3라운드부터 등장함 그리고 리스트에 안넣음 (안죽여도 다음 웨이브로 넘어감)
        // Debug.Log("플레이어수" + PhotonNetwork.PlayerList.Length);
        budget += budgetBonus[waveNum] * Mathf.Max((PhotonNetwork.PlayerList.Length - 1),0); //PhotonNetwork.CountOfPlayersInRooms
        while (budget >= 5)
        {
            // objectsToSpawn 리스트에서 무작위로 오브젝트 선택
            int spawnMobIndex = Random.Range(0, mobs.Length - 1 - (madeJellyfish ? 1 : 0)); // 보스 만 제외하고 나옴

            //Debug.Log("Try Spawn Mob Num: " + spawnMobIndex);
            int count = 1; // 몇 번 반복할지 정함
            if (waveNum == GameManager.instance.lastWave) // 마지막 웨이브라면
            {
                budget = 0;
                spawnMobIndex = mobs.Length - 1; // 리스트의 맨 마지막에 있는 몹, 즉 보스가 나옴...
            }
            else
            {
                //spawnMobIndex = 3;
                DefaultMob mobData = mobs[spawnMobIndex].GetComponent<DefaultMob>();
                while (budget < mobData.cost) // 가격 내의 몹을 선택함
                {
                    if (spawnMobIndex < 0)
                    {
                        // 몹 소환 불가능
                        return;
                    }
                    spawnMobIndex--; // 더 싼 몹으로 (가격 순으로 정렬되어있음)
                    mobData = mobs[spawnMobIndex].GetComponent<DefaultMob>(); // 해당 몹의 데이터를 가져옴
                }
                if (budget >= mobData.cost * squadSize[waveNum])
                {
                    count = squadSize[waveNum]; // 여러 마리씩 뭉쳐서 생성
                }
                if (spawnMobIndex == 0 && budget >= mobData.cost * count * 2) // 스폰하는 몹이 쥐이고 여유가 있다면...
                {
                    count *= 2; // 2배 많이 생성
                }
                if (spawnMobIndex == 2) // 스폰하는 몹이 크로와상이라면...
                {
                    count = Mathf.Max(1, count / 2); // 2배 적게 생성
                }
                if(spawnMobIndex == 3)
                {
                    count = 1;
                    madeJellyfish = true;
                }
                //budget -= mobData.cost * count;
            }
            //Debug.Log("Budget: " + budget);

            int spawnPointIndex = Random.Range(0, spawnPoints.Length);

            for(int i = 0; i < count; i++)
            {
                float radius = 0.5f * (count-1);
                if (spawnMobIndex == 0) radius /= 2;
                float angle = 360 * i / count;

                float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                //photonview.RPC("RPC_CurrentWaveRemove", RpcTarget.All, spawnMobIndex, spawnPointIndex, x, y);

                // GameObject newMob = .GetMob(spawnMobIndex);
                GameObject newMob = PhotonNetwork.Instantiate(mobs[spawnMobIndex].name, spawnPoints[spawnPointIndex].position + new Vector3(x, y, 0), Quaternion.identity);

                budget -= newMob.GetComponent<DefaultMob>().cost;

                if(spawnMobIndex != 3) // 해파리가 아니라면
                {
                    // 추가
                    // GameManager.instance.currentWave.Add(newMob);
                    RPC_CurrentWaveAdd(newMob.GetComponent<PhotonView>().ViewID);
                }
            }
        }

        [PunRPC]
        void RPC_CurrentWaveAdd(int pvID)
        {
            GameObject mob = PhotonView.Find(pvID).GetComponent<DefaultMob>().gameObject;
            GameManager.instance.currentWave.Add(mob);
        }
    }
    #endregion
}
