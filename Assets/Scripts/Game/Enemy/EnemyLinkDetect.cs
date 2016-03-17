using UnityEngine;
using System.Collections;

public class EnemyLinkDetect : MonoBehaviour {

	private EnemyStatus enemyStatus;

	//AI Time-Staggering用
	public bool isAction;
	public int delayTime;
	public float delayTimer;

	void Awake()
	{
		enemyStatus = GetComponentInParent<EnemyStatus> ();
	}

	void OnEnable()
	{
		isAction = false;
		delayTimer = 0;
	}

	void Update () 
	{

		//AI Time-Staggering用
		//仲間の反応に百ミニ秒単位のディレイを与える
		if (isAction && enemyStatus.hp > 0) 
		{
			delayTimer += Time.unscaledDeltaTime;

			if (delayTimer >= (float)delayTime / 1000.0f) 
			{
				enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
			}

		}



	}

	void OnTriggerStay(Collider other)
	{
		EnemyStatus linkStatus;

		//隣の仲間は警戒状態に入ったら、警戒状態になる
		if (other.tag == "Enemy")
		{
			linkStatus = other.GetComponent<EnemyStatus> ();

			if (linkStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE && enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH)
			{
				isAction = true;
				delayTime = Random.Range (500, 700);
			}
		}
	}

}
