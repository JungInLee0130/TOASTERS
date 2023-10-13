using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class BakeryHpBar : MonoBehaviour
{
	public GameObject playerManager;
	GameObject player;
	public Slider hpBar; // 빵집 위에 표시

	public Slider UIhpBar; // 빵집에서 멀어질시
	public TextMeshProUGUI hpUIText;

	Vector2 playerPos;
	Vector2 bakeryPos;
	float distance;

	void Start()
	{
		GameManager.instance.Bakery.GetComponent<TempEntity>().Restore();
		UIhpBar.gameObject.SetActive(false);
		hpUIText.gameObject.SetActive(false);
	}

	void Update()
	{
		player = playerManager.GetComponent<PlayerCreate>().curPlayer;

		hpBar.value = (float)GameManager.instance.Bakery.GetComponent<TempEntity>().Hp
			/ GameManager.instance.Bakery.GetComponent<TempEntity>().MaxHp;

		UIhpBar.value = hpBar.value;

		if (hpBar.value == 0)
		{
			hpBar.gameObject.SetActive(false);
		}

		// 플레이어에게 빵집이 보이지 않을때 UI에 빵집 hp 표시
		playerPos = player.transform.position;
		bakeryPos = GameManager.instance.Bakery.transform.position;
		distance = Vector2.Distance(playerPos, bakeryPos);


		// 빵집 - 플레이어
		Vector2 dir = (bakeryPos - playerPos).normalized;

		if (distance > 10)
		{
			/*Debug.Log("dir : " + dir);
			Debug.Log("UIhpBar : " + UIhpBar.transform.position);
			Debug.Log("hpUIText : " + hpUIText.transform.position);

			// 사분면 조건문
			float dirX = dir.x;
			float dirY = dir.y;

			// 기준 기울기
			float linearPlus = 9.0f / 16.0f;
			float linearMinus = -linearPlus;

			// 벡터 기울기
			float dirLinear = dir.y / dir.x;

			Vector2 canvasPos = new Vector2(0,0);

			// 1사분면 : linearPlus
			if (dirX > 0 && dirY == 0)
			{
				canvasPos = new Vector2(210, 0);
			}

			else if (dirX > 0 && dirY > 0)
			{
				// 기준 기울기보다 작음
				if (dirLinear < linearPlus)
				{
					canvasPos = new Vector2(210, 210 * dirLinear);
				}
				// 기준 기울기보다 큼
				else
				{
					canvasPos = new Vector2(120 / dirLinear, 120);
				}
			}

			// 좌표중하나가 0인경우
			else if (dirX == 0 && dirY > 0)
			{
				canvasPos = new Vector2(0, 120);
			}

			// 제2사분면
			else if (dirX < 0 && dirY > 0)
			{
				// 기준 기울기보다 작음
				if (dirLinear < linearMinus)
				{
					canvasPos = new Vector2(120 / dirLinear, 120);
				}
				// 기준 기울기보다 큼
				else
				{
					canvasPos = new Vector2(-210, -210 * dirLinear);
				}
			}

			else if (dirX < 0 && dirY == 0)
			{
				canvasPos = new Vector2(-210, 0);
			}

			else if (dirX < 0 && dirY < 0)
			{
				// 기준 기울기보다 작음
				if (dirLinear < linearPlus)
				{
					canvasPos = new Vector2(-210, -210 * dirLinear);
				}
				// 기준 기울기보다 큼
				else
				{
					canvasPos = new Vector2(-120 / dirLinear, -120);
				}
			}

			else if (dirX == 0 && dirY < 0)
			{
				canvasPos = new Vector2(0, -120);
			}

			else if (dirX > 0 && dirY < 0)
			{
				// 기준 기울기보다 작음
				if (dirLinear < linearMinus)
				{
					canvasPos = new Vector2(-120 / dirLinear, -120);
				}
				// 기준 기울기보다 큼
				else
				{
					canvasPos = new Vector2(210, 210 * dirLinear);
				}
			}

			Vector2 gamePos = canvasPos + new Vector2(240, 135);
			Vector2 textPos = canvasPos + new Vector2(240, 145);
			UIhpBar.transform.position = gamePos;
			hpUIText.transform.position = textPos;*/

			UIhpBar.gameObject.SetActive(true);
			hpUIText.gameObject.SetActive(true);
		}
		else
		{
			UIhpBar.gameObject.SetActive(false);
			hpUIText.gameObject.SetActive(false);
		}

	}
}
