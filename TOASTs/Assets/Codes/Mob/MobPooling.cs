using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPooling : MonoBehaviour
{
    #region Fields
    public static MobPooling instance;

    [SerializeField]
    private GameObject[] MobPrefabs;

    private Queue<GameObject>[] poolingMobQueues = new Queue<GameObject>[3]; // 원래 3이 아니라 MobPrefabs.length임
    #endregion

    #region Methods
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion
}
