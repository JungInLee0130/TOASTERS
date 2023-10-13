using UnityEngine;

public class BreadData : MonoBehaviour
{
    public int num; // 인덱스
    public int tier; // 티어
    public string name; // 빵 이름 
    public float seconds; // 소요시간 (초)
    public Sprite sp;
    public string effect; // 효과
    public int gold; // 돈
    public int wheat;

    /*    public string content; // 빵 내용
        public int cost;
    */
    public BreadData()
    {
    }

    /*public BreadData(int num, int tier, string name, string content, float seconds, int cost, Sprite sp)
    {
        this.num = num;
        this.tier = tier;
        this.name = name;
        this.content = content;
        this.seconds = seconds;
        this.cost = cost;
        this.sp = sp;
        this.effect = effect;
        this.gold = gold;
    }*/
    public BreadData(int num, int tier, string name, float seconds, Sprite sp, string effect, int gold, int wheat)
    {
        this.num = num;
        this.tier = tier;
        this.name = name;
        this.seconds = seconds;
        this.sp = sp;
        this.effect = effect;
        this.gold = gold;
        this.wheat = wheat;
    }
}
