using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour {

	//部位破壊できるか
	public bool haveParts;
	public bool notRecoverColor;

	public bool canBlow;
	public bool onfloor;

	public float hpMax;
	public float hp;

	public float atk;

	public float stateChangeTime;
	public float recoverTime;

	//public float lastAttackTime;

	//視野の計算
	public float sightAngle;
	public float sightDistance;
	public float angleSpeed;
	//視野外から接近されたら、どの範囲内にプレイヤーを感知できる
	public float detectDistanceWithoutSight;

	private Rigidbody rb;
	private MeshRenderer mr;
	private CollisionCounter collisionCounter;

	private Transform playerTransform;
	private PlayerChangeForm playerChanegeForm;
	private PlayerCrystalSystem playerCrystalSystem;

	private bool isDeadSound;
	private EnemySoundController enemySoundController;

	//DEBUG用
	private bool isDebug = false;

	public enum EnemyState
	{
		ENEMY_SEARCH,
		ENEMY_TRACE,
		ENEMY_ATTACK,
		ENEMY_ATTACKED,
		ENEMY_ATTACKEDMOVE,
		ENEMY_DEAD
	};

	public EnemyState enemyState;

	void Awake()
	{
		enemySoundController = GetComponent<EnemySoundController> ();
		mr = GetComponentInChildren<MeshRenderer> ();
		rb = GetComponent<Rigidbody> ();
		collisionCounter = GetComponentInChildren <CollisionCounter> ();

		GameObject playerObject = GameObject.FindWithTag ("Player");
		playerTransform = playerObject.GetComponent<Transform> ();
		playerChanegeForm = playerObject.GetComponent<PlayerChangeForm> ();
		playerCrystalSystem = playerObject.GetComponent<PlayerCrystalSystem> ();

		enemyState = EnemyState.ENEMY_SEARCH;
		hp = hpMax;
	}

	void OnEnable()
	{
		hp = hpMax;
		enemyState = EnemyState.ENEMY_SEARCH;
		if (mr != null) 
		{
			mr.material.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
		}

		isDeadSound = false;
	}

	void Update ()
	{
		if (collisionCounter != null) 
		{
			if (collisionCounter.counter > 0) {
				onfloor = true;
			} else {
				onfloor = false;
			}
		}

		if (enemyState == EnemyState.ENEMY_ATTACKED)
		{
			rb.velocity = Vector3.zero;
		}

		if (mr != null) {

			//ダメージから回復する
			if ((enemyState == EnemyState.ENEMY_ATTACKED || enemyState == EnemyState.ENEMY_ATTACKEDMOVE) && !notRecoverColor) 
			{
				if (mr.material.color.g <= 1.0) 
				{
					mr.material.color = new Color (mr.material.color.r, mr.material.color.g + 1.7f * Time.deltaTime, mr.material.color.b, mr.material.color.a);
				}

				if (mr.material.color.b <= 1.0)
				{
					mr.material.color = new Color (mr.material.color.r, mr.material.color.g, mr.material.color.b + 1.7f * Time.deltaTime, mr.material.color.a);
				}
			}

			/*
			if (hp <= 0 && enemyState != EnemyState.ENEMY_ATTACKED && enemyState != EnemyState.ENEMY_ATTACKEDMOVE)
			{
				enemyState = EnemyState.ENEMY_DEAD;
			}*/


			if (enemyState == EnemyState.ENEMY_DEAD) 
			{
				if (!isDeadSound && enemySoundController != null) 
				{
					enemySoundController.SetSound (EnemySoundController.SoundType.DEAD);
					isDeadSound = true;
				}

				if (mr.material.color.a > 0.0f) 
				{
					mr.material.color = new Color (1.0f, 1.0f, 1.0f, mr.material.color.a - 2.0f * Time.deltaTime);
				} 
				else 
				{
					EnemyPoolerScript.current.SetLastEnemyPosition (this.transform.position);
					gameObject.SetActive (false);
				}
			}
			else 
			{
				if (mr.material.color.a < 1.0f && !notRecoverColor)
				{
					mr.material.color = new Color (1.0f, 1.0f, 1.0f, mr.material.color.a + 2.0f * Time.deltaTime);
				} 
			}
		}

		///////////////////////
		//DEBUG用
		//敵無敵
		if (Input.GetButtonDown ("Debug")) 
		{
			isDebug = !isDebug;
		}
		//////////////////////


	}

	//普通のダメージ処理
	public void Damage(float dmg)
	{
		if (hp > 0) 
		{
			if(!isDebug)
				hp -= dmg;
			
			enemyState = EnemyState.ENEMY_ATTACKED;
			stateChangeTime = Time.time;
			playerChanegeForm.TimeGageUp (dmg);
			playerCrystalSystem.AddCrystalProcess (dmg);

			if(enemySoundController != null)
				enemySoundController.SetSound (EnemySoundController.SoundType.GETDAMAGE);

			if(!notRecoverColor)
				mr.material.color = new Color (1.0f, 0.5f, 0.5f, mr.material.color.a);
		}
	}

	//スライダー攻撃に飛ばされた処理
	public void DamageWithMove(float dmg)
	{
		if (hp > 0) 
		{
			if(!isDebug)
				hp -= dmg;
			
			enemyState = EnemyState.ENEMY_ATTACKEDMOVE;
			stateChangeTime = Time.time;
			playerChanegeForm.TimeGageUp (dmg);
			playerCrystalSystem.AddCrystalProcess (dmg);

			if(enemySoundController != null)
				enemySoundController.SetSound (EnemySoundController.SoundType.GETDAMAGE);

			if(!notRecoverColor)
				mr.material.color = new Color(1.0f, 0.5f, 0.5f, mr.material.color.a);
		}
	}


	//モンスター共通の視野処理
	public void SearchPlayer()
	{
		Vector3 vectorToPlayer;
		float cosAngle;

		//視野外から接近されたら、どの範囲内にプレイヤーを感知できる
		if ((playerTransform.position - transform.position).magnitude <= detectDistanceWithoutSight)
		{
			enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
		}

		//正面から接近されたら、視野の計算
		if ((playerTransform.position - transform.position).magnitude <= sightDistance)
		{
			vectorToPlayer = playerTransform.position - transform.position;
			cosAngle = Vector3.Dot(transform.forward, vectorToPlayer) / (transform.forward.magnitude * vectorToPlayer.magnitude);

			if (cosAngle >= Mathf.Cos (sightAngle * Mathf.PI / 180.0f))
			{
				enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
			}

		}
	}

	//プレイヤーの方向に向う処理
	public void FacePlayer()
	{
		Vector3 tmpEulerAngle, targetEulerAngle;

		tmpEulerAngle = transform.eulerAngles;
		transform.LookAt (playerTransform);

		targetEulerAngle = transform.eulerAngles;

		if (targetEulerAngle.y > tmpEulerAngle.y)
		{
			if (targetEulerAngle.y - tmpEulerAngle.y > 180.0f)
			{
				tmpEulerAngle.y += 360.0f;
			}
		}
		else if (targetEulerAngle.y < tmpEulerAngle.y)
		{
			if (tmpEulerAngle.y - targetEulerAngle.y > 180.0f)
			{
				tmpEulerAngle.y -= 360.0f;
			}
		}

		targetEulerAngle = new Vector3(tmpEulerAngle.x, targetEulerAngle.y, tmpEulerAngle.z);
		transform.eulerAngles = new Vector3 (tmpEulerAngle.x, tmpEulerAngle.y, tmpEulerAngle.z);

		if (targetEulerAngle.y > tmpEulerAngle.y)
		{
			tmpEulerAngle = new Vector3 (targetEulerAngle.x, tmpEulerAngle.y + angleSpeed * Time.deltaTime, targetEulerAngle.z);	
		} 
		else if (targetEulerAngle.y < tmpEulerAngle.y)
		{
			tmpEulerAngle = new Vector3 (targetEulerAngle.x, tmpEulerAngle.y - angleSpeed * Time.deltaTime, targetEulerAngle.z);	
		}


		if (Mathf.Abs (targetEulerAngle.y - tmpEulerAngle.y) < 3.0f)
		{
			tmpEulerAngle = new Vector3 (targetEulerAngle.x, targetEulerAngle.y, targetEulerAngle.z);	
		}

		transform.eulerAngles = tmpEulerAngle;
	}

	//モンスターはプレイヤーに向かってると判断する
	public bool isFacePlayer()
	{
		Vector3 tmpEulerAngle, targetEulerAngle;

		tmpEulerAngle = transform.eulerAngles;
		transform.LookAt (playerTransform);

		targetEulerAngle = transform.eulerAngles;

		if (targetEulerAngle.y > tmpEulerAngle.y)
		{
			if (targetEulerAngle.y - tmpEulerAngle.y > 180.0f)
			{
				tmpEulerAngle.y += 360.0f;
			}
		}
		else if (targetEulerAngle.y < tmpEulerAngle.y)
		{
			if (tmpEulerAngle.y - targetEulerAngle.y > 180.0f)
			{
				tmpEulerAngle.y -= 360.0f;
			}
		}

		targetEulerAngle = new Vector3(tmpEulerAngle.x, targetEulerAngle.y, tmpEulerAngle.z);
		transform.eulerAngles = new Vector3 (tmpEulerAngle.x, tmpEulerAngle.y, tmpEulerAngle.z);


		if (Mathf.Abs (targetEulerAngle.y - tmpEulerAngle.y) < 10.0f)
		{
			return true;
		}

		return false;

	}

	public float GetAtk()
	{
		return atk;
	}

}
