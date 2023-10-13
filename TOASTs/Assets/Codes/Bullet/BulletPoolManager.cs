/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletPoolManager : MonoBehaviour
{
    #region Fields
    public static BulletPoolManager instance;

    // Non-Photon prefab(포톤 부착 이후 삭제)
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject slimeBulletPrefab;

    [SerializeField]
    GameObject croissantBombPrefab;

    Queue<TempBullet> bulletPool = new Queue<TempBullet>();
    Queue<TempMobBullet> slimeBulletPool = new Queue<TempMobBullet>();
    Queue<TempMobCroissantBomb> croissantBombPool = new Queue<TempMobCroissantBomb>();

    // Photon prefab(포톤 부착 이후 주석 해제)
    *//*[SerializeField]
    bullet bulletPrefab;
    MobBullet slimeBulletPrefab;
    Queue<bullet> bulletPool = new Queue<bullet>();
    Queue<MobBullet> slimeBulletPool = new Queue<MobBulletPool>();
    *//*

    #endregion

    #region Methods
    private void Awake()
    {
        instance = this;
        Pooling(100);
        MobPooling(50);
        CroissantBombPooling(50);
    }

    private void Pooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            instance.bulletPool.Enqueue(CreateNewBullet());
        }
    }

    private void MobPooling(int count)
    {
        for (int i = 0; i < count; i++)
        {
            instance.slimeBulletPool.Enqueue(CreateNewMobBullet());
        }
    }

    private void CroissantBombPooling(int count)
    {
        for (int i = 0;i < count; i++)
        {
            instance.croissantBombPool.Enqueue(CreateNewCroissantBomb());
        }
    }

    /// <summary>
    /// 새로운 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private TempBullet CreateNewBullet()
    {

        //TempBullet newBullet = Instantiate(bulletPrefab).GetComponent<TempBullet>();
        TempBullet newBullet = Instantiate(bulletPrefab).GetComponent<TempBullet>();

        newBullet.gameObject.SetActive(false);
        return newBullet;
    }


    /// <summary>
    /// 새로운 몬스터 총알 생성하는 메소드
    /// Photon 연결하면 반환값 TempBullet -> Bullet으로 변경하면 된다
    /// </summary>
    /// <returns>풀링할 새로운 총알</returns>
    private TempMobBullet CreateNewMobBullet()
    {
        TempMobBullet newBullet = Instantiate(slimeBulletPrefab, transform).GetComponent<TempMobBullet>();
        newBullet.gameObject.SetActive(false);
        return newBullet;
    }

    /// <summary>
    /// 새로운 폭탄 생성하는 메소드
    /// Photon 연결하면 반환값 Temp 떼면 된다
    /// </summary>
    /// <returns>풀링할 새로운 폭탄</returns>
    private TempMobCroissantBomb CreateNewCroissantBomb()
    {
        TempMobCroissantBomb newBomb = Instantiate(croissantBombPrefab, transform).GetComponent<TempMobCroissantBomb>();
        newBomb.gameObject.SetActive(false);
        return newBomb;
    }

    /// <summary>
    /// bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public TempBullet GetBullet()
    {
        Debug.Log($">>> bullets : {instance.bulletPool.Count}");
        if (instance.bulletPool.Count > 0)
        {
            Debug.Log("안에 있으니까 꺼내오기");
            TempBullet newBullet = instance.bulletPool.Dequeue();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
        else
        {
            Debug.Log("안에 없으니까 만들어오기");
            TempBullet newBullet = CreateNewBullet();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public TempMobBullet GetMobBullet()
    {
        if (instance.bulletPool.Count > 0)
        {
            TempMobBullet newBullet = instance.slimeBulletPool.Dequeue();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
        else
        {
            TempMobBullet newBullet = CreateNewMobBullet();
            newBullet.transform.SetParent(null);
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }
    }

    /// <summary>
    /// mob bullet 오브젝트 pooling하기
    /// 만약 미리 pooling 해놓은 값이 바닥나면 새로 생성해서 반환
    /// </summary>
    /// <returns>pooled bullet</returns>
    public TempMobCroissantBomb GetCroissantBomb()
    {
        if (instance.bulletPool.Count > 0)
        {
            TempMobCroissantBomb newBomb = instance.croissantBombPool.Dequeue();
            newBomb.transform.SetParent(null);
            newBomb.gameObject.SetActive(true);
            return newBomb;
        }
        else
        {
            TempMobCroissantBomb newBomb = CreateNewCroissantBomb();
            newBomb.transform.SetParent(null);
            newBomb.gameObject.SetActive(true);
            return newBomb;
        }
    }

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(TempBullet bullet)
    {
        Debug.Log(">>> 다시 저장");
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(instance.transform);
        instance.bulletPool.Enqueue(bullet);
    }

    /// <summary>
    /// 사용한 총알 다시 집어넣기
    /// </summary>
    /// <param name="bullet">다시 집어넣을 총알 오브젝트</param>
    public void ReturnBullet(TempMobBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(instance.transform);
        instance.slimeBulletPool.Enqueue(bullet);
    }

    public void ReturnBullet(TempMobCroissantBomb bomb)
    {
        bomb.gameObject.SetActive(false);
        bomb.transform.SetParent(instance.transform);
        instance.croissantBombPool.Enqueue(bomb);
    }
    #endregion
}
*/
