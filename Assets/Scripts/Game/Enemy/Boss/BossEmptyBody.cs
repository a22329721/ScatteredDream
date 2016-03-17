using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ボスコントローラー
public class BossEmptyBody : MonoBehaviour {

	public float hp, hpMax;
	private List<EnemyStatus> partsEnemyStatuses;
	private Rigidbody rb;

	//死んたら、何秒後に消える
	private float deadCountDown;

	//プレイヤーに近づかれたら、何秒後に逃げる
	public float escapeCount;

	//各部位
	private BossBody bossBody;
	private BossLeftSword bossLeftSword;
	private BossRightSword bossRightSword;
	private BossWings bossWings;

	private bool isSwordUse;

	//スキル範囲 & 冷却時間
	private float nearSwordDistance = 6.0f;
	private float swordCooldown = 2.0f;
	private float swordTimer;

	private float crystalWallDistanceMin = 8.0f;
	private float crystalWallDistanceMax = 20.0f;
	private float crystalCooldown = 3.0f;
	private float crystalTakeBreak = 1.2f;
	private float crystalTimer;
	private int crystalAtOnce = 7;
	private int crystalNow = 0;

	private float fireDistanceMin = 6.5f;
	private float fireCooldown = 1.0f;
	private float fireTakeBreak = 0.5f;
	private float fireTimer;
	private int fireAtOnce = 20;
	private int fireNow = 0;

	private Animator at;

	//プレイヤー
	private Transform playerTransform;


	public enum BossState
	{
		STAND,
		ESCAPE
	};

	public BossState bossState;


	void Awake () 
	{
		//各部位の状態を取得
		Component[] enemyStatusComponents = GetComponentsInChildren(typeof(EnemyStatus));
		//Debug.Log (enemyStatusComponents.Length);

		rb = GetComponent<Rigidbody> ();
		partsEnemyStatuses = new List<EnemyStatus>();

		for (int i = 0; i < enemyStatusComponents.Length; i++) 
		{
			partsEnemyStatuses.Add ((EnemyStatus)enemyStatusComponents [i]);
		}
			
		//各部位のコントローラー
		bossBody = GetComponentInChildren<BossBody> ();
		bossLeftSword = GetComponentInChildren<BossLeftSword> ();
		bossRightSword = GetComponentInChildren<BossRightSword> ();
		bossWings = GetComponentInChildren<BossWings> ();

		at = GetComponentInChildren<Animator> ();

		//プレイヤーデータ取得
		GameObject playerObj = GameObject.FindWithTag("Player");
		playerTransform = playerObj.GetComponent<Transform> ();

	}

	void OnEnable()
	{
		//全パーツを起動する
		for (int i = 0; i < partsEnemyStatuses.Count - 1; i++) 
		{
			transform.GetChild (i).gameObject.SetActive(true);
		}

		//EmptyBodyはパーツ全部の総HP
		partsEnemyStatuses [0].hp = 0.0f;
		for (int i = 1; i < partsEnemyStatuses.Count; i++) 
		{
			partsEnemyStatuses [0].hp += partsEnemyStatuses [i].hpMax;
			partsEnemyStatuses [i].hp = partsEnemyStatuses [i].hpMax;
			partsEnemyStatuses [i].enemyState = EnemyStatus.EnemyState.ENEMY_TRACE;
			//MonoBehaviour.print (i + " HP: " + partsEnemyStatuses [i].hpMax);
		}
		partsEnemyStatuses [0].hpMax = partsEnemyStatuses [0].hp;
		//MonoBehaviour.print ("Total HP: " + partsEnemyStatuses [0].hp);

		partsEnemyStatuses [0].enemyState = EnemyStatus.EnemyState.ENEMY_SEARCH;

		//画面で表示するHPはボス本体のHPだけ
		hpMax = partsEnemyStatuses [1].hpMax;
		hp = 0.0f;


		//ボスは倒された何秒後に消える
		deadCountDown = 2.0f;

		bossState = BossState.STAND;
		escapeCount = 10.0f;

		isSwordUse = true;
		fireNow = 0;
		fireTimer = 0.0f;
		crystalTimer = 0.0f;
		crystalNow = 0;
		swordTimer = 0.0f;
	}
	

	void FixedUpdate ()
	{

		if (partsEnemyStatuses [0].enemyState == EnemyStatus.EnemyState.ENEMY_SEARCH) 
		{
			partsEnemyStatuses [0].SearchPlayer ();
		}

		if (partsEnemyStatuses [0].enemyState == EnemyStatus.EnemyState.ENEMY_TRACE) 
		{
			//ボスの移動
			TracePlayer ();

			partsEnemyStatuses [0].FacePlayer ();
			if (partsEnemyStatuses [0].isFacePlayer ())
			{
				//ボスの攻撃
				if (partsEnemyStatuses [1].enemyState != EnemyStatus.EnemyState.ENEMY_DEAD) 
					AttackPlayer ();
			}
		}

		if (bossWings.isAlive () && !isSwordUse)
		{
			bossLeftSword.SetOn ();
			bossRightSword.SetOn ();
			isSwordUse = true;
		}
		else if (!bossWings.isAlive () && isSwordUse)
		{
			bossLeftSword.SetOff ();
			bossRightSword.SetOff ();
			isSwordUse = false;
		}


		//HPの表示を更新する
		hp = partsEnemyStatuses [1].hp;

		//ボス主体が倒れた
		if (partsEnemyStatuses [1].enemyState == EnemyStatus.EnemyState.ENEMY_DEAD) 
		{
			if(at != null)
				at.SetInteger ("state", 3);
			
			for (int i = 1; i < partsEnemyStatuses.Count; i++) 
			{
				partsEnemyStatuses [i].enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;
				//MonoBehaviour.print (i + " HP: " + partsEnemyStatuses [i].hpMax);
			}

			//MonoBehaviour.print ("Boss Dead");
			deadCountDown -= Time.unscaledDeltaTime;

			if (deadCountDown <= 0.0f) 
			{
				partsEnemyStatuses [0].enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;
				gameObject.SetActive (false);
			}
		}


	}

	//ボスの移動
	void TracePlayer ()
	{
		Vector3 distanceToPlayer = playerTransform.position - transform.position;
		distanceToPlayer.y = 0.0f;


		if (bossState == BossState.STAND)
		{
			//プレイヤーは身に近づいたら、カウントダウンする
			if (distanceToPlayer.magnitude <= crystalWallDistanceMin)
			{
				escapeCount -= Time.fixedDeltaTime;

				if (escapeCount <= 0.0f) 
				{
					escapeCount = 3.0f;
					bossState = BossState.ESCAPE;
				}
			} 
			else
			{
				escapeCount = 10.0f;

			}
		} 
		else if (bossState == BossState.ESCAPE)
		{
			rb.velocity = transform.forward * -8.0f;
			escapeCount -= Time.fixedDeltaTime;

			if (distanceToPlayer.magnitude >= crystalWallDistanceMax || escapeCount <= 0.0f) 
			{
				bossState = BossState.STAND;
				escapeCount = 10.0f;
			}
		}


	}

	void AttackPlayer()
	{
		Vector3 distanceToPlayer = playerTransform.position - transform.position;
		distanceToPlayer.y = 0.0f;

		//ファイヤー
		if (distanceToPlayer.magnitude > fireDistanceMin)
		{
			fireTimer += Time.fixedDeltaTime;

			if (fireNow < fireAtOnce && fireTimer >= fireTakeBreak)
			{
				bossBody.SetCrystallBall ();

				fireNow++;
				fireTimer = 0.0f;
			}
			else if (fireNow >= fireAtOnce)
			{
				if (fireTimer >= fireCooldown) 
				{
					fireTimer = 0.0f;
					fireNow = 0;
				}
			}
		}

		//クリスタルウォール
		if (distanceToPlayer.magnitude >= crystalWallDistanceMin && distanceToPlayer.magnitude <= crystalWallDistanceMax 
			&& (bossLeftSword.canUse () && bossRightSword.canUse()) )
		{
			crystalTimer += Time.fixedDeltaTime;

			if (crystalNow < crystalAtOnce && crystalTimer >= crystalTakeBreak)
			{
				at.SetInteger ("state", 1);
				bossBody.SetCrystallWall ();

				crystalNow++;
				crystalTimer = 0.0f;
			}
			else if (crystalNow >= crystalAtOnce)
			{
				if (crystalTimer >= crystalCooldown) 
				{
					crystalTimer = 0.0f;
					crystalNow = 0;
				}
			}
		}

		//ソード
		if (distanceToPlayer.magnitude <= nearSwordDistance && isSwordUse && (bossLeftSword.canUse () && bossRightSword.canUse ()))
		{
			int randomNum;

			swordTimer += Time.fixedDeltaTime;

			if (swordTimer >= swordCooldown )
			{
				while (true) 
				{
					randomNum = Random.Range (0, 2);
					if (randomNum == 0 && !bossRightSword.isDead) 
					{
						at.SetInteger ("state", 2);
						bossRightSword.Attack ();
						swordTimer = 0.0f;
						break;
					} 
					else if (randomNum == 1 && !bossLeftSword.isDead)
					{
						at.SetInteger ("state", 2);
						bossLeftSword.Attack ();
						swordTimer = 0.0f;
						break;
					} 
					else if (bossLeftSword.isDead && bossRightSword.isDead)
					{
						break;
					}

				}
			}

		}

	}

}
