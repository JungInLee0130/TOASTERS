using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerData : MonoBehaviour
{
	string name; // 타워 이름
	[SerializeField]
	float abilityRange; // 능력 범위
	public int curHp = 1000; // 현재 hp
	public int maxHp = 1000; // 최대 hp 
	public bool isFixed; // 수리 받는지
}
