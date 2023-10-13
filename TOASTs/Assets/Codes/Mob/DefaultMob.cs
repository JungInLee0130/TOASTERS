using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class DefaultMob : MonoBehaviour
{
    PhotonView photonview;
    PhotonView photonview1;

    public GameObject target;               // 움직일 대상 오브젝트
    public Transform player;                // 추후 동적으로 넣는 방식으로 변경 예정 
    public GameObject rangeAttack;
    public GameObject deathEffect;
    GameManager gameManager;
    PoolManager poolManager;

    bool isDead;
    int mobIndex;
    public int cost;
    public int MobIndex
    {
        get { return mobIndex; }
        set { mobIndex = value; }
    }

    protected enum Order
    {
        Idle,
        Move,
        Attack,
        Die,
        Summon,     // 보스 전용
        Spell,
        Charge,     // 보스 전용
    };

    //[SerializeField]
    //protected int hp;                       // 체력
    [SerializeField]
    int damage;                   // 공격력
    public int Damage
    {
        get { return damage; }
        set
        {
            if (value < 0)
            {
                Debug.Log("damage : 잘못된 값 입력입니다.");
            }
            else
            {
                damage = value;
            }
        }
    }

    [SerializeField]
    float attackCoolTime;    // 공격 쿨타임
    public float AttackCoolTime
    {
        get { return attackCoolTime; }
        set
        {
            if (value < 0)
            {
                Debug.Log("attackCoolTime : 잘못된 값 입력입니다.");
            }
            else
            {
                attackCoolTime = value;
            }
        }
    }
    [SerializeField]
    float attackRange;       // 공격 범위
    public float AttackRange
    {
        get { return attackRange; }
        set
        {
            if (value < 0)
            {
                Debug.Log("attackRange : 잘못된 값 입력입니다.");
            }
            else
            {
                attackRange = value;
            }
        }
    }
    //[SerializeField]
    float moveSpeed;       // 이동 속도 -> navmesh의 speed에 접근해서 변경하는데 사용
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set
        {
            if (value < 0)
            {
                Debug.Log("moveSpeed : 잘못된 값 입력입니다.");
            }
            else
            {
                moveSpeed = value;
            }
        }
    }
    [SerializeField]
    float acquisitionRange;  // 적 인지 범위 (a.k.a 확보 거리)
    public float AcquisitionRange
    {
        get { return acquisitionRange; }
        set
        {
            if (value < 0)
            {
                Debug.Log("acquisitionRange : 잘못된 값 입력입니다.");
            }
            else
            {
                acquisitionRange = value;
            }
        }
    }
    [SerializeField]
    float searchTime;             // 적 탐색을 몇초마다 하는지 설정
    public float SearchTime
    {
        get { return searchTime; }
        set
        {
            if (value < 0)
            {
                Debug.Log("searchTime : 잘못된 값 입력입니다.");
            }
            else
            {
                searchTime = value;
            }
        }
    }
    //[SerializeField]
    //protected int armor;                    // 방어력

    //protected int absorbedDamage = 0;       // 방어력으로 흡수한 피해 (방어력이 공격력보다 높거나 같을 때 사용)

    //public bool attackType;               // false = 근접, true = 원거리
    protected float lastAttackTime, lastCallTime = 0;
    protected float waitTime = 0;                     // 얼타는 시간
    Rigidbody2D rigid;
    protected Animator anim;
    protected NavMeshAgent agent;
    protected LayerMask combinedLayerMask;            // 레이어 묶음 저장용
    protected GameObject defaultTarget;               // 빵집 저장용 오브젝트
    RaycastHit2D[] targets;                 // 레이에 맞은 내용들 저장용
    protected SpriteRenderer spr;
    protected TempEntity entity;

    [SerializeField]
    protected Order currentOrder = Order.Idle;
    protected bool orderInit = false;

    protected float minRandomAtkDelay = -0.05f;
    protected float maxRandomAtkDelay = 0.1f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //Debug.Log(">>> Awake()에서 agent 가져옴 : " + agent.name + ", null? : " + (agent == null));
        spr = GetComponent<SpriteRenderer>();
        //defaultTarget = target;             // 추후 저장된 곳에서 가져오기
        // 게임매니저에 등록해야함
        defaultTarget = GameObject.Find("Bakery");
        target = defaultTarget;
        entity = GetComponent<TempEntity>();
        Debug.Log(">>> Awake()의 entity null? : " + (entity == null));
        entity.Restore();
        gameManager = GameManager.instance;
        poolManager = PoolManager.Instance;
    }

    void Start()
    {
        photonview = GameObject.Find("WheatManager").GetComponent<PhotonView>();
        photonview1 = GetComponent<PhotonView>();

        //agent.updateRotation = false;
        //agent.updateUpAxis = false;
        //agent.speed = moveSpeed;
        combinedLayerMask = CreateLayerMask("Player", "Bakery");
        Init();

    }

    // 오브젝트 풀링 후 초기화 작업
    void OnEnable()
    {
        //Debug.Log(">>> agent.name : " + agent);
        //Debug.Log(">>> agent==null : " + (agent == null));
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        poolManager = PoolManager.Instance;
        Init();
    }

    protected virtual void Init()
    {
        //Debug.Log(">>> entity null? : " + (entity == null));
        entity.Restore();
        target = defaultTarget;
        SetOrder(Order.Idle);
        //hp = _maxHp;
        Damage = 100;
        AttackCoolTime = 2f;
        AttackRange = 5f;
        MoveSpeed = 2.5f;
        AcquisitionRange = 3f;
        SearchTime = 2f;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
        mobIndex = 0;
        isDead = false;
    }

    void FixedUpdate()
    {
        switch (currentOrder)
        {
            case Order.Idle:        // 목표가 없거나 대기중임 or 목표가 사거리 안에 있지만 공격 쿨다운을 기다리는 중
                Idle();
                break;
            case Order.Move:        // 목표가 확보 거리 안에 있고 사거리 바깥에 있음 (** 확보거리 > 사거리)
                Move();
                // NavMesh로 하여금 target으로 이동하기
                agent.SetDestination(target.transform.position);
                // 이동중엔 X를 뒤집을 수 있음 (공격중에 뒤집지 않는 이유는 이미 공격중에 방향이 바뀌면 어색해보임)
                spr.flipX = (target.transform.position.x > transform.position.x);
                break;
            case Order.Attack:      // 목표가 사거리 안에 있고 공격중임 (공격 애니메이션 재생, 끝나면 Idle 혹은 Move가 됨)
                Attack();
                break;
            case Order.Die:         // 사망함 (파괴 애니메이션이 있다면 재생함)
                Die();
                break;
        }

    }

    protected virtual void SetOrder(Order _order)
    {
        currentOrder = _order;
        orderInit = false;
    }
    protected virtual void Idle()
    {
        if (!orderInit)
        {
            waitTime = Time.fixedTime + Random.Range(0.05f, 0.5f); // 랜덤 딜레이
            anim.SetTrigger("Idle");
            orderInit = true;
            Stop();
        }

        if (target != null && Time.fixedTime >= waitTime)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= acquisitionRange) // 획득범위 안에 있다면, 
            {
                AttackCheck(distanceToTarget); // 공격할 수 있는지 체크
            }
            else
            {
                // 타겟이 거리 바깥에 있다.
                target = defaultTarget;     // 빵집으로 타겟을 바꾸고 움직이기
                SetOrder(Order.Move);
                //if (target != defaultTarget) // 빵집이 타겟이 아니라면
                //{

                //}
                //else
                //{
                //    SetOrder(Order.Move);       // 
                //}
            }
        }
    }

    protected virtual void Move()
    {
        if (!orderInit)
        {
            anim.SetTrigger("Move");
            agent.speed = moveSpeed;
            orderInit = true;
        }
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= acquisitionRange)
        {
            AttackCheck(distanceToTarget);
        }
        if (currentOrder == Order.Attack)
        {
            lastCallTime = Time.fixedTime; // 공격에 성공했다면, 다른 타겟을 찾지 않음
        }
        if (Time.fixedTime - lastCallTime >= searchTime)
        {
            FindEnemy();
            lastCallTime = Time.fixedTime;
        }

    }

    // 공격 로직을 이곳에 구현 예정
    protected virtual void Attack()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        //Debug.Log("공격 성공!");
        if (!orderInit)
        {
            lastAttackTime = Time.time - Random.Range(minRandomAtkDelay, maxRandomAtkDelay); // 랜덤 딜레이
            anim.SetTrigger("Attack");
            Stop();
            orderInit = true;
        }
        // 애니메이션이 끝나면 Init 혹은 Move로 돌아가야함


        //// 근거리 혹은 원거리 공격이 성공했을때의 로직
        //// 근거리 혹은 원거리 공격이 실패했을때의 로직
    }

    // 데미지를 받았을 때 데미지를 적용하는 로직
    // 몹 종류에 따라 방어력 로직등이 들어갈 수 있으므로 상속받아 사용할 수 있도록 구현
    public virtual void GainDamage(int _damage)
    {
        if (currentOrder != Order.Die && !entity.GainDamage(_damage))
        {
            SetOrder(Order.Die);
        }
    }

    // 체력이 0 이하로 내려가면 사망처리
    protected virtual void Die()
    {
        // 사망 트리거 발동
        //ani.SetTrigger("DieTrigger");

        // 잡았다!
        if (GameManager.instance != null)
        {
            // 오브젝트가 파괴될 때 리스트에서 제거
            if (gameManager.currentWave.Contains(gameObject))
            {
                gameManager.currentWave.Remove(gameObject); //// rpc
                //photonview.RPC("RPC_CurrentWaveRemove", RpcTarget.All);
            }

            if (!isDead)
            {
                GainGold();
                isDead = true;
            }

            // 애니메이션 종료
            photonview1.RPC("RPC_DestroyMob", RpcTarget.MasterClient);
            /*poolManager.ReturnMob(gameObject);
            gameObject.SetActive(false);*/

            if (deathEffect != null)
            {
                PhotonNetwork.Instantiate(deathEffect.name, transform.position, Quaternion.identity);
            }
        }
    }

    /*[PunRPC]
    void RPC_DestroyMob()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }*/

    /*[PunRPC]
    void RPC_CurrentWaveRemove()
    {
        gameManager.currentWave.Remove(gameObject);
    }*/


    void GainGold()
    {
        gameManager.gold += (cost * 3);
        photonview.RPC("RPC_UpdateGold", RpcTarget.All, gameManager.gold);

    }

    // 공격 대상이 플레이어, 타워, 빵집 3종류이기 때문에 해당 객체를 타겟으로 넘겨주기 위해 파라메터를 넘겨주는 방식으로 구현
    // 최초 타겟은 빵집으로 선택할 예정
    // 추후 어떻게 될지 모르기 때문에 프로젝트 종료 전까지는 코드 보존
    /*void MobMove(GameObject target)
    {
        Vector2 dirVec2 = (target.GetComponent<Rigidbody2D>().position - rigid.position).normalized;
        Vector2 nextVec2 = dirVec2 * moveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec2);
        rigid.velocity = Vector2.zero;
    }*/

    // 공격 대상이 플레이어, 타워, 빵집 3종류이기 때문에 해당 객체를 타겟으로 넘겨주기 위해 파라메터를 넘겨주는 방식으로 구현
    void AttackCheck(float distanceToTarget)
    {
        // 목표의 체력 정보를 불러옵니다.
        TempEntity targetEntity = target.GetComponent<TempEntity>();
        if (targetEntity == null)
        {
            // 체력 정보가 없는 타겟이라면?
            target = defaultTarget;
            SetOrder(Order.Move);
            return;
        }
        if (target == defaultTarget)
        {
            distanceToTarget -= 3.5f; // 빵집이면 사거리 증가
        }
        if (distanceToTarget <= attackRange)//타겟이 공격 범위 내에 있고,
        {
            if (Time.fixedTime - lastAttackTime >= attackCoolTime) // 공격 쿨다운이 지났을 때
            {

                if (targetEntity.Hp > 0)
                {
                    SetOrder(Order.Attack);
                }
                else
                {
                    if (target != defaultTarget)
                    {
                        target = defaultTarget;
                        SetOrder(Order.Move);
                        return;
                    }
                }

            }
            // 아니면 대기

        }
        else
        {
            if (currentOrder == Order.Idle)
            {
                SetOrder(Order.Move); // 아니면 이동
            }

        }
    }

    // 여러개의 레이어를 결합
    public static LayerMask CreateLayerMask(params string[] layerNames)
    {
        LayerMask layerMask = 0;
        foreach (string layerName in layerNames)
        {
            layerMask |= (1 << LayerMask.NameToLayer(layerName));
        }

        return layerMask;
    }

    // 원형으로 레이를 발사하여 적을 찾는 함수
    protected void FindEnemy()
    {
        targets = Physics2D.CircleCastAll(transform.position, acquisitionRange, Vector2.zero, 0, combinedLayerMask);
        int len = targets.Length;

        if (len < 1)
        {
            target = defaultTarget;
            return;
        }

        for (int i = 0; i < len; i++)
        {
            if (targets[i].transform.CompareTag("Player"))
            {
                //Debug.Log("플레이어 감지");
                target = targets[i].transform.gameObject;
                return;
            }
            else if (targets[i].transform.CompareTag("Tower"))
            {
                target = targets[i].transform.gameObject;
            }
        }

        if (!targets[len - 1].transform.CompareTag("Bakery")) return;
        else
        {
            target = defaultTarget;
        }
    }

    // 충돌 이나 밀림 후 특정 타이밍에 멈추게 하기 위한 함수
    protected void Stop()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().drag = 5.0f;
        // 몹이 실제로 정지하는 코드
        agent.speed = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().drag = 5.0f;
    }


}
