using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobCroissantBomb : MonoBehaviour
{
    //public GameObject target;               // 움직일 대상 오브젝트
    //RaycastHit2D[] targets;                 // 레이에 맞은 내용들 저장용
    [SerializeField]
    float lifeTime;
    NavMeshAgent agent = null;
    //float speed;
    GameObject target;
    public GameObject effect;
    public int damage;
    float birthTime;
    public float range;
    public LayerMask targetLayer;
    public MobCroissant parent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    public void Init(GameObject _target, float _lifeTime, float _speed, int _damage, float _range)
    {
        target = _target;
        lifeTime = _lifeTime;
        damage = _damage;
        agent.speed = _speed;
        range = _range;
        birthTime = Time.fixedTime;
    }

    void FixedUpdate()
    {

        // 타겟을 향해 상시 이동
        /*Debug.Log(">>> agent == null? : " + (agent == null));
        Debug.Log(">>> target == null? : " + (target == null));
        Debug.Log(">>> target.transform == null? : " + (target.transform == null));*/
        //Debug.Log(">>>>> 누가 부름? : " + PhotonNetwork.NickName);
        if (target == null) return;
        agent.SetDestination(target.transform.position);

        // 날 죽여줘
        bool killMe = false;

        // 시간이 다 되면 폭발
        if (Time.fixedTime >= birthTime + lifeTime)
        {
            //Debug.Log("Death By Timeout");
            killMe = true;
        }

        // 주변에 타겟이 있으면 폭발
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        //if()
        if (distanceToTarget <= range)
        {
            //Debug.Log("Death By Range");
            killMe = true;
        }

        if (killMe)
        {
            // 폭발하고 데미지 주기
            Explode();
        }
    }

    public void Explode()
    {
        // 폭발 반경은 사거리의 2배
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);// * 1.5f
        foreach (Collider2D hitCollider in hitColliders)
        {
            int resultDamage = damage;
            float dist = Vector2.Distance(transform.position, hitCollider.gameObject.transform.position);
            if (dist >= range * 2 && dist > range)
            {
                
                resultDamage /= 2; // 반샷
            }
            if (hitCollider.gameObject.tag == "Player")
            {
                hitCollider.GetComponent<PlayerController>().GainDamage(resultDamage);
            }
            if (hitCollider.gameObject.tag == "Bakery")
            {
                hitCollider.GetComponent<BakeryController>().GainDamage(resultDamage);
            }
        }
        if(parent != null)
            parent.myBomb = null;
        GameObject tmp = PhotonNetwork.Instantiate(effect.name, transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
        tmpQ.Enqueue(tmp);
        Invoke("DestroyEffect", 3.0f);

        // 다시 집어넣기
        // BulletPoolManagerReal.Instance.ReturnCroissantBomb(this);
    }



    Queue<GameObject> tmpQ = new Queue<GameObject>();
    void DestroyEffect()
    {
        PhotonNetwork.Destroy(tmpQ.Dequeue().GetComponent<PhotonView>());
    }
}
