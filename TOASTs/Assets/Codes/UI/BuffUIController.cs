using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIController : MonoBehaviour
{
    public GameObject buffPrefab;
    //public GameObject parentTransform;

    // �� ����
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

        // ���� ���� ������ ����
        int bufferCount = transform.childCount;

        // ������ ������ ��ġ
        Vector3 bufferPosition = new Vector3((10 + bufferCount * 40) * -1, -10, 0);

        // �θ� Transform�� ���� ��ǥ�� ���� ��ġ�� ���մϴ�.
        Vector3 worldPosition = transform.TransformPoint(bufferPosition);

        GameObject newBuff = Instantiate(buffPrefab, worldPosition, Quaternion.identity, transform);
    
        GaindBreadUIController controller = newBuff.GetComponent<GaindBreadUIController>();

        // ��Ÿ�� ����
        controller.UpdateBuff(duration);

        // �̹��� ����
        controller.changeImg(breadDatas[col,row].sp);

        // ������ ���� UI�� �θ� Transform�� �߰��մϴ�.
        newBuff.transform.SetParent(transform, false);


        
    }

    public void RemoveBuffer()
    {

    }
}
