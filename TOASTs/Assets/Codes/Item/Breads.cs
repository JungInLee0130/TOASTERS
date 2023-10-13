using UnityEngine;

public class Breads : MonoBehaviour
{
    public BreadData[,] breadDatas; // 12개 빵 정보 : [티어, 인덱스]

    public Sprite[] sprites; // 스프라이트

    int[] seconds // 소요시간(초)
        = { 4,4,4,3,
            8,6,6,4,
            8,180,8,9};

    string[] breadName
        = { "바게트 빵", "쿠키","핫도그", "프레츨",
			"공갈빵","피자빵", "와플", "커스타드",
			"샌드위치", "도라야키", "머핀", "도넛"}; // 빵 이름 저장 배열

    string[] effects // 효과
        = {"공격력 +1 영구 증가", "방어력 +1 영구 증가", "이동속도 +1 영구 증가", "자신의 체력 100 회복",
    "빵 굽는 속도 10% 증가", "공격속도 10% 증가", "추가 체력 30 증가", "전체 팀원 체력 100 회복",
    "30초동안 총알 관통", "제작시 승리합니다!", "10초동안 무적", "공격력,방어력,이동속도 3이 분배되어 증가"};

    int[] golds
        = { 100,100,100,80,
            120,160,150,140,
            200,2000,200,300};

    int[] wheats
        = { 0,0,0,0,
            20,10,20,20,
            50,500,40,35};

    int tierLen;
    int columnLen;

    public bool isReduceTime;


    void Awake()
    {
        breadDatas = new BreadData[3, 4];

        int num = 0;

        tierLen = 3;
        columnLen = 4;

        for (int tier = 0; tier < tierLen; tier++)
        {
            for (int index = 0; index < columnLen; index++)
            {
                InitBread(tier, index, num);
                num++;
            }
        }

        isReduceTime = false;

    }

    void InitBread(int tier, int coulmn, int num)
    {
        // 인덱스 : 1 ~ 15
        // 티어 : 1 ~ 3
        // 이름
        // 소요시간 : 1,2,3 초
        // 스프라이트
        breadDatas[tier, coulmn] = new BreadData(num + 1, tier + 1,
            breadName[num].Substring(0),
            seconds[num],
            sprites[num],
            effects[num],
            golds[num],
            wheats[num]);
    }

   
    public void UpdateBreadTime(float time)
    {
        for (int i = 0; i < tierLen; i++)
        {
            for (int j = 0; j < columnLen; j++)
            {
                breadDatas[i, j].seconds = breadDatas[i, j].seconds - breadDatas[i, j].seconds * time;
            }
        }
        isReduceTime = true;
    }
}

