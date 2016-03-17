using UnityEngine;
using System.Collections;

public class SwordBehavior : MonoBehaviour {

	public bool isUse;
	private EnemyStatus enemyStatus;
	private MeshRenderer mr;

	void Start ()
	{
		enemyStatus = GetComponent<EnemyStatus> ();
		mr = GetComponentInChildren<MeshRenderer> ();

		enemyStatus.notRecoverColor = true;
	}

	void OnEnable()
	{
		isUse = true;
	}


	void FixedUpdate () 
	{
		if (isUse) 
		{
			//回転
			transform.Rotate (0.0f, Time.fixedDeltaTime * 100.0f, 0.0f);

			//色変化
			if (mr.material.color.a < 1.0f) 
			{
				mr.material.color = new Color (mr.material.color.r, mr.material.color.g, mr.material.color.b, mr.material.color.a + Time.fixedDeltaTime);
			}
		}
		else 
		{
			if (mr.material.color.a > 0.0f) 
			{
				mr.material.color = new Color (mr.material.color.r, mr.material.color.g, mr.material.color.b, mr.material.color.a - Time.fixedDeltaTime);
			}
		}

		if (enemyStatus.hp <= 0.0f)
		{
			enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;
		}
	}
}
