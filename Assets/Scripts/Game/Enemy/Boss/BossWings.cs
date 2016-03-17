using UnityEngine;
using System.Collections;

//ルーン
public class BossWings : MonoBehaviour {

	private EnemyStatus enemyStatus;
	private MeshRenderer mr;

	private float deadCounter = 0.0f;

	void Start ()
	{
		enemyStatus = GetComponent<EnemyStatus> ();
		mr = GetComponent<MeshRenderer> ();

		enemyStatus.notRecoverColor = true;

	}


	void FixedUpdate () 
	{

		if (enemyStatus.enemyState != EnemyStatus.EnemyState.ENEMY_DEAD) 
		{
			mr.material.color = new Color (1.0f, enemyStatus.hp / enemyStatus.hpMax, enemyStatus.hp / enemyStatus.hpMax, 1.0f);

			if (enemyStatus.hp <= 0.0f) {
				enemyStatus.hp = 0.0f;
				mr.material.color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
				deadCounter += Time.fixedDeltaTime;

				if (deadCounter >= 10.0f) {
					enemyStatus.hp = enemyStatus.hpMax;
					deadCounter = 0.0f;
				}

			}
		}
	}

	public bool isAlive()
	{
		return (enemyStatus.hp > 0.0f ? true : false);
	}

}
