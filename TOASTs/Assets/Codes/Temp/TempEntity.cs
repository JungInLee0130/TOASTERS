using Photon.Pun;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TempEntity : MonoBehaviour
{
    PhotonView photonview;

    // 데미지를 받을 수 있는 컴포넌트
    [SerializeField]
    int maxHp;
	public int MaxHp
	{
		get { return maxHp; }
		set
		{
			if (value < 0)
			{
				Debug.Log("maxHp : 잘못된 값 입력입니다.");
			}
			else
			{
				maxHp = value;
			}
		}
	}
    [SerializeField]
    int hp;                       // 체력
	public int Hp
	{
		get { return hp; }
		set
		{
			if (value < 0)
			{
				Debug.Log("hp : 잘못된 값 입력입니다.");
			}
			else
			{
				hp = value;
			}
		}
	}
	protected int absorbedDamage = 0;       // 방어력으로 흡수한 피해 (방어력이 공격력보다 높거나 같을 때 사용)
	public int AbsorbedDamage
	{
		get { return absorbedDamage; }
		set
		{
			if (value < 0)
			{
				Debug.Log("absorbedDamage : 잘못된 값 입력입니다.");
			}
			else
			{
				absorbedDamage = value;
			}
		}
	}
    [SerializeField]
    int armor;                    // 방어력
	public int Armor
	{
		get { return armor; }
		set
		{
			if (value < 0)
			{
				Debug.Log("armor : 잘못된 값 입력입니다.");
			}
			else
			{
				armor = value;
			}
		}
	}

	void Awake()
	{
		Restore();
	}

	private void Start()
	{
		photonview = GetComponent<PhotonView>();
	}

        // 무적시간 추가
    public bool GainDamage(int _damage) // 데미지를 맞고 살아있는지를 반환한다.
	{
		if (_damage <= 0) return true;

		int hitDamage = _damage - armor;
		if (hitDamage <= 0)
		{
			absorbedDamage += _damage;
			if (absorbedDamage > armor + 1) hitDamage = 1;
			absorbedDamage -= armor + 1;
		}
		photonview.RPC("RPC_BakeryDamage", RpcTarget.All, hitDamage); //hp -= hitDamage;

		return hp > 0;
    }

    [PunRPC]
    void RPC_BakeryDamage(int hitDamage)
    {
        hp -= hitDamage;
    }

    public void Restore()
	{
		Hp = maxHp;
		AbsorbedDamage = 0;
	}

    public void Heal(int amount)
    {
        Hp = Mathf.Min(MaxHp, Hp + amount);
    }
}
