using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIController : MonoBehaviour
{
    public GameObject buffPrefab;
    //public GameObject parentTransform;

    // 빵 정보
    BreadData[,] breadDatas;

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            AddBuff(1,1, 20);
        }
    }


    public void AddBuff(int col, int row, float duration)
    {
        breadDatas = GameManager.instance.breadDatas;

        // 가동 중인 버프의 개수
        int bufferCount = transform.childCount;

        // 버프가 보여질 위치
        Vector3 bufferPosition = new Vector3((10 + bufferCount * 40) * -1, -10, 0);

        // 부모 Transform의 월드 좌표에 버프 위치를 더합니다.
        Vector3 worldPosition = transform.TransformPoint(bufferPosition);

        GameObject newBuff = Instantiate(buffPrefab, worldPosition, Quaternion.identity, transform);
    
        GaindBreadUIController controller = newBuff.GetComponent<GaindBreadUIController>();

        // 쿨타임 적용
        controller.UpdateBuff(duration);

        // 이미지 변경
        controller.changeImg(breadDatas[col,row].sp);

        // 생성된 버프 UI를 부모 Transform에 추가합니다.
        newBuff.transform.SetParent(transform, false);


        
    }

    public void RemoveBuffer()
    {

    }
}
