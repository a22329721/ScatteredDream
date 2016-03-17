using UnityEngine;
using System.Collections;

public class WalkMinionAction : MonoBehaviour {

	public float moveSpeed;
	public float attackDistance;

	private float nextAttackTime = 0.0f;
	private bool attackReady;
	private float chargeSpeed = 1.5f;
	public float AttackCooldown;

	private EnemyStatus enemyStatus;
	private Rigidbody rb;
	private MeshRenderer mr;

	private Transform playerTransform;

	public Animator animate;

	void Awake () {

		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		playerTransform = playerObject.GetComponent<Transform> ();

		enemyStatus = GetComponent<EnemyStatus> ();
		rb = GetComponent <Rigidbody> ();
		mr = GetComponentInChildren<MeshRenderer> ();
	
	}

	void OnEnable()
	{
		enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_SEARCH;
		attackReady = false;
	}


	void FixedUpdate () {
		
		//モーション更新
		UpdateAnimation ();

		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACKED)
		{
			rb.velocity = Vector3.zero;
		}

		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACKED 
			|| enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACKEDMOVE)
		{
			//普通状態に回復する
			if (Time.time >= enemyStatus.stateChangeTime + enemyStatus.recoverTime)
			{
				nextAttackTime = Time.time + 1.5f;

				if (enemyStatus.hp > 0.0f)
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
				else 
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;

			}
		}

		//プレイヤーを探す
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH)
		{
			enemyStatus.SearchPlayer ();
		}

		//プレイヤーに追跡する
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE && enemyStatus.onfloor)
		{
			TracePlayer ();
		}

		//プレイヤーに攻撃する
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK && enemyStatus.onfloor)
		{
			if (mr.material.color.r > 0.0f)
			{
				mr.material.color = new Color (mr.material.color.r - chargeSpeed * Time.deltaTime, 
					                           mr.material.color.g - chargeSpeed * Time.deltaTime, 1.0f, 1.0f);
			}
			else 
			{
				mr.material.color = new Color (0.0f, 0.0f, 1.0f, 1.0f);
				attackReady = true;
			}


			if (attackReady) 
			{
				AttackPlayer ();

				mr.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				attackReady = false;
				nextAttackTime = Time.time + AttackCooldown;
				enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
			}
		}


	}

	//プレイヤーに追跡する
	private void TracePlayer()
	{
		enemyStatus.FacePlayer ();

		Vector3 horizontalDistance = (playerTransform.position - transform.position);
		horizontalDistance.y = 0.0f;

		if (horizontalDistance.magnitude > attackDistance) 
		{
			float tmpVelocityY = rb.velocity.y;
			rb.velocity = transform.forward * moveSpeed + new Vector3 (0.0f, tmpVelocityY, 0.0f);
		}
		else
		{
			Vector3 distance = (playerTransform.position - transform.position);

			if (enemyStatus.isFacePlayer () && Time.time >= nextAttackTime && (distance.magnitude - attackDistance <= 0.5f)) 
			{
				enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_ATTACK;
			}
		}

	}

	//プレイヤーを攻撃する
	private void AttackPlayer()
	{
		GameObject obj;
		EnemySwordAttack1 script1;

		for (int i = 0; i < 3; i++)
		{

			obj = EnemyAttackPoolerScript.current.GetSwordAttackPooledObject ();

			script1 = obj.GetComponent<EnemySwordAttack1> ();

			if (obj == null)
				return;

			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			obj.transform.Translate (1.0f + (0.4f * i), 1.5f - (0.4f * i), 1.7f);
			obj.transform.Rotate (90.0f, 0.0f, 0.0f);

			script1.moveVector = -transform.right - transform.up;
			script1.moveVector.Normalize ();
			script1.SetAtk (enemyStatus.atk / 2.0f);

			obj.SetActive (true);
		}
	}


	//モーション更新
	private void UpdateAnimation()
	{
		if (enemyStatus.enemyState  == EnemyStatus.EnemyState.ENEMY_ATTACKED
			|| enemyStatus.enemyState  == EnemyStatus.EnemyState.ENEMY_ATTACKEDMOVE ) 
		{
			animate.SetBool ("Damaged", true);
			animate.SetBool ("Idle", false);
			animate.SetBool ("Attack", false);
		}
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK)
		{
			animate.SetBool ("Attack", true);
			animate.SetBool ("Damaged", false);
			animate.SetBool ("Idle", false);
		}
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH
			||enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE
		)
		{
			animate.SetBool ("Attack", false);
			animate.SetBool ("Damaged", false);
			animate.SetBool ("Idle", true);
		}
	}

}
