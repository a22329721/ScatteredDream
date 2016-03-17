using UnityEngine;
using System.Collections;

public class FlyMinionAction : MonoBehaviour {

	public float limitHeightUP;
	public float limitHeightDOWN;
	public float moveSpeed;
	public float attackDistance;

	private float nextAttackTime = 0.0f;
	public bool isFireBall;
	private int fireBallCnt;
	public int maxFireBallOnce;
	public float fireBetweenTime;
	public float fireAttackCooldown;

	public float bodyAttackAtk;
	public float bodyAttackSpeed = 3.0f;
	private float bodyAttackTimer;
	public float bodyAttackDistance;
	public float bodyAttackCooldown;

	private EnemyStatus enemyStatus;
	private Rigidbody rb;
	private MeshRenderer mr;
	private SphereCollider sc;

	private Transform playerTransform;
	private PlayerStatus playerStatus;

	public Animator animate;

	public GameObject angryeyebow;
	public GameObject eye_X;
	public GameObject eye_l;

	void Awake () {

		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		playerTransform = playerObject.GetComponent<Transform> ();
		playerStatus = playerObject.GetComponent<PlayerStatus> ();

		enemyStatus = GetComponent<EnemyStatus> ();
		rb = GetComponent <Rigidbody> ();
		mr = GetComponent<MeshRenderer> ();
		sc = GetComponent<SphereCollider> ();
	
	}

	void OnEnable()
	{
		enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_SEARCH;
		isFireBall = false;
	}


	void FixedUpdate () {
		
		//モーション更新
		UpdateAnimation();

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
				if(enemyStatus.hp > 0.0f)
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
				else 
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;

			}
		}


		//攻撃されてないなら、空で浮ぶ
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE || enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH
			|| enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK)
		{
			//ボディ攻撃の回復判定
			if ((enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK && !isFireBall))
			{
				//if (transform.position.y <= limitHeightDOWN / 3.0f || bodyAttackTimer >= 0.3f) 
				if (bodyAttackTimer >= 0.4f) 
				{
					mr.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
					nextAttackTime = Time.time + bodyAttackCooldown;
					sc.isTrigger = false;
				}
			}

			//飛ぶの高さ判定
			int layerMask = 1;
			RaycastHit hit;
			float distanceToFloor = 0.0f;

			if (Physics.Raycast (transform.position, -transform.up, out hit, Mathf.Infinity, layerMask)) 
			{
				distanceToFloor = (hit.point - transform.position).magnitude;
			}

			if (distanceToFloor <= limitHeightDOWN)
			{

				if (!(enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK && !isFireBall))
					rb.velocity = new Vector3 (0.0f, 5.0f, 0.0f);
			}

			if (distanceToFloor >= limitHeightUP && rb.velocity.y > 0.0f)
			{
				rb.velocity = new Vector3 (rb.velocity.x, 0.0f, rb.velocity.y);
			}
		}

		//プレイヤーを探す
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH)
		{
			enemyStatus.SearchPlayer ();
		}

		//プレイヤーに追跡する
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE)
		{
			TracePlayer ();
		}

		//プレイヤーにファイヤボール攻撃する
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK)
		{
			mr.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			bodyAttackTimer += Time.fixedDeltaTime;

			if (isFireBall) 
			{
				enemyStatus.FacePlayer ();

				//ファイヤボール攻撃途中に近づいたら、ボディ攻撃に変更する
				if ((playerTransform.position - transform.position).magnitude <= bodyAttackDistance)
				{
					if(fireBallCnt > 0)
						nextAttackTime = Time.time + fireAttackCooldown;

					fireBallCnt = 0;
					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
					return;
				}
					
				if (Time.time >= nextAttackTime) 
				{
					FireAttackPlayer ();

					//最大発射数まで連射する
					if (fireBallCnt >= maxFireBallOnce)
					{
						nextAttackTime = Time.time + fireAttackCooldown;
						fireBallCnt = 0;

						enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
					}
				}
			}
		}

	
	}

	//プレイヤーに追跡する
	private void TracePlayer()
	{
		enemyStatus.FacePlayer ();

		//長距離：追跡
		if ((playerTransform.position - transform.position).magnitude > attackDistance) 
		{
			float tmpVelocityY = rb.velocity.y;
			rb.velocity = transform.forward * moveSpeed + new Vector3 (0.0f, tmpVelocityY, 0.0f);
		} 
		//中距離：ファイヤボール
		else if ((playerTransform.position - transform.position).magnitude > bodyAttackDistance) 
		{
			if (enemyStatus.isFacePlayer ())
			{
				enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_ATTACK;
				isFireBall = true;
			}
		} 
		//近距離：ボディ攻撃
		else 
		{
			if (enemyStatus.isFacePlayer () && Time.time >= nextAttackTime)
			{
				//畜力
				mr.material.color = new Color (mr.material.color.r - 2 * Time.deltaTime,
					1.0f, 1.0f, 1.0f);
				
				//攻撃
				if (mr.material.color.r <= 0.0f)
				{
					//added here
					animate.SetBool ("Melee", true);
					animate.SetBool ("Fireball", false);
					Vector3 moveVector;

					enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_ATTACK;
					isFireBall = false;

					moveVector = playerTransform.position - transform.position;
					moveVector = new Vector3 (moveVector.x, moveVector.y + 1.0f, moveVector.z);
					moveVector.Normalize ();
					rb.velocity = moveVector * bodyAttackSpeed;
					sc.isTrigger = true;

					bodyAttackTimer = 0.0f;

				}
			}
		}

	}

	//プレイヤーを攻撃する
	private void FireAttackPlayer()
	{
		GameObject obj;
		EnemyFireBall script1;
		//added here
		animate.SetBool ("Melee", false);
		animate.SetBool ("Fireball", true);
		obj = EnemyAttackPoolerScript.current.GetFireBallAttackPooledObject ();

		script1 = obj.GetComponent<EnemyFireBall> ();

		if (obj == null)
			return;

		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.transform.Translate (0.0f, 0.0f, 1.0f);

		script1.moveSpeed = 4.0f;

		script1.startPoint = obj.transform.position;
		script1.endPoint = playerTransform.position + playerTransform.up * 1.0f;

		script1.atk = enemyStatus.GetAtk () / 2.0f;

		obj.SetActive (true);

		nextAttackTime = Time.time + fireBetweenTime;
		fireBallCnt++;
	}


	//ボディ攻撃
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log (other.name);

		if (other.tag == "Player") 
		{
			if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK && isFireBall == false) 
			{
				
				bodyAttackAtk = enemyStatus.GetAtk ();
				playerStatus.DamagePlayer (bodyAttackAtk, new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
			}
		}
		sc.isTrigger = false;
	}

	//モーション更新
	private void UpdateAnimation()
	{
		if (enemyStatus.enemyState  == EnemyStatus.EnemyState.ENEMY_ATTACKED
			|| enemyStatus.enemyState  == EnemyStatus.EnemyState.ENEMY_ATTACKEDMOVE ) 
		{
			eye_X.SetActive (true);
			eye_l.SetActive (false);
			angryeyebow.SetActive (false);
			animate.SetBool ("Damaged", true);
			animate.SetBool ("Fireball", false);
			animate.SetBool ("Melee", false);
			animate.SetBool ("Idle", false);

		}
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_ATTACK)// && attackReady) 
		{
			eye_X.SetActive (false);
			eye_l.SetActive (true);
			angryeyebow.SetActive (true);

			animate.SetBool ("Damaged", false);
			animate.SetBool ("Idle", false);

		}
		if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH
			||enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_TRACE
		)
		{
			eye_X.SetActive (false);
			eye_l.SetActive (true);
			angryeyebow.SetActive (true);
			if (enemyStatus.enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH) 
			{
				angryeyebow.SetActive (false);
			}
			animate.SetBool ("Idle", true);
			animate.SetBool ("Damaged", false);
			animate.SetBool ("Fireball", false);
			animate.SetBool ("Melee", false);
		}
	}
}
