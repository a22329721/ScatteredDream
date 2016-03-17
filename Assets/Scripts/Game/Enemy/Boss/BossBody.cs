using UnityEngine;
using System.Collections;

//ボス本体
public class BossBody : MonoBehaviour {

	private EnemyStatus enemyStatus;

	private Transform playerTransform;

	//空中で浮ぶ
	private bool isUp;

	void Start () 
	{
		enemyStatus = GetComponent<EnemyStatus> ();

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerTransform = playerObj.GetComponent<Transform> ();

		isUp = true;
	}

	void FixedUpdate () 
	{

		//空で浮ぶ
		if (isUp)
		{
			transform.Translate (transform.up * Time.fixedDeltaTime * 0.5f);
			if (transform.localPosition.y >= -0.55f)
				isUp = false;
		}
		else
		{
			transform.Translate (-transform.up * Time.fixedDeltaTime * 0.5f);
			if (transform.localPosition.y <= -1.00f)
				isUp = true;
		}

		if (enemyStatus.hp <= 0.0f)
		{
			enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;
		}
	}

	//弾を発射する
	public void SetCrystallBall()
	{
		GameObject obj;
		EnemyFireBall script1;

		obj = EnemyAttackPoolerScript.current.GetCrystalBulletPooledObject ();

		script1 = obj.GetComponent<EnemyFireBall> ();

		if (obj == null)
			return;

		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.transform.LookAt (playerTransform);
		obj.transform.Translate (0.0f, 0.0f, 1.0f);

		script1.moveSpeed = 6.0f;

		script1.startPoint = obj.transform.position;
		script1.endPoint = playerTransform.position + playerTransform.up * 1.0f;

		script1.atk = enemyStatus.GetAtk () / 2.0f;

		obj.SetActive (true);

	}

	//壁を設置する
	public void SetCrystallWall()
	{
		GameObject obj;
		BossCrystalWall script1;

		obj = EnemyAttackPoolerScript.current.GetCrystalWallPooledObject ();

		script1 = obj.GetComponent<BossCrystalWall> ();

		if (obj == null)
			return;

		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.transform.Translate (Random.Range(-3, 3), 2.0f, 2.0f);

		script1.moveVector = transform.forward;

		obj.SetActive (true);

	}

}
