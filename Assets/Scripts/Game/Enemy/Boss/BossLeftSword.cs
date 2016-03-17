using UnityEngine;
using System.Collections;

public class BossLeftSword : MonoBehaviour {

	private EnemyStatus enemyStatus;
	private MeshRenderer mr;
	private BoxCollider collider;

	private SwordBehavior swordBehavior;

	private Transform playerTranform;
	private PlayerStatus playerStatus;

	public enum BladeState
	{
		STANDBY,	//使える
		TRAIL,		//プレイヤーに追跡
		ATTACK,		//プレイヤーに攻撃
		END			//床に刺す

	};

	public BladeState bladeState;
	private float moveSpeed = 8.0f;
	private float trailTimer = 0.0f;
	private float angleRotated;

	private float attackSpeed = 25.0f;
	private Vector3 targetPosition;

	private AudioSource audioSource;
	public AudioClip attackSound, hitSound;

	public bool isDead;

	void Awake ()
	{
		enemyStatus = GetComponent<EnemyStatus> ();
		mr = GetComponentInChildren<MeshRenderer> ();
		collider = GetComponent<BoxCollider> ();
		audioSource = GetComponent<AudioSource> ();

		enemyStatus.notRecoverColor = true;

		swordBehavior = GetComponent<SwordBehavior> ();

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerTranform = playerObj.GetComponent<Transform> ();
		playerStatus = playerObj.GetComponent<PlayerStatus> ();
	}

	void OnEnable()
	{
		bladeState = BladeState.STANDBY;
		collider.isTrigger = false;
		trailTimer = 0.0f;
		angleRotated = 0.0f;
		isDead = false;
	}


	void FixedUpdate () 
	{
		switch (bladeState) 
		{
		case BladeState.TRAIL:
			{
				TracePlayer ();

				break;
			}
		case BladeState.ATTACK:
			{
				AttackPlayer ();

				break;
			}
		case BladeState.END:
			{
				AttackEnd ();
				break;
			}
		default:
			break;
		}

		if (enemyStatus.hp <= 0.0f) 
		{
			bladeState = BladeState.STANDBY;
			isDead = true;
		}

	}

	void TracePlayer ()
	{
		//ジャンプを邪魔しないため
		collider.isTrigger = true;

		//ボスの前に移動
		Vector3 distanceToTarget = targetPosition - transform.localPosition;

		//前に指す
		if (angleRotated < 90.0f) 
		{
			angleRotated += 90.0f * Time.fixedDeltaTime;
			transform.RotateAround (transform.position, transform.parent.right, -90.0f * Time.fixedDeltaTime);
			//transform.localEulerAngles += new Vector3 (90.0f * Time.fixedDeltaTime, 0.0f, 0.0f);
		}

		if (distanceToTarget.magnitude >= 0.1f) 
		{
			distanceToTarget.Normalize ();
			transform.localPosition += distanceToTarget * moveSpeed * Time.fixedDeltaTime;
		}
		else 
		{
			transform.localPosition = targetPosition;
		}

		trailTimer += Time.fixedDeltaTime;
		if (trailTimer >= 1.0f) 
		{
			//色が変わったら、刺す
			if (mr.material.color.r >= 0.1f)
			{
				mr.material.color = Color.Lerp (mr.material.color, Color.cyan, 2 * Time.fixedDeltaTime);
				if (mr.material.color.r < 0.1f) 
				{
					audioSource.clip = attackSound;
					audioSource.Play ();
					bladeState = BladeState.ATTACK;
					trailTimer = 0.0f;
				}
			}
		}


	}

	void AttackPlayer()
	{
		//攻撃判定オン
		collider.isTrigger = true;

		//降下
		if (transform.localPosition.x <= 2.4f)
		{
			transform.localPosition += new Vector3 (attackSpeed * Time.fixedDeltaTime, 0.0f, 0.0f);
		}
		else
		{
			//剣の状態を記録して、次のフェーズはそのまま止まる
			mr.material.color = Color.white;
			bladeState = BladeState.END;
			trailTimer = 0.0f;
		}

	}

	void AttackEnd ()
	{
		//攻撃判定オフ
		collider.isTrigger = false;

		trailTimer += Time.fixedDeltaTime;

		//リセット
		if (trailTimer >= 0.3f) 
		{
			transform.localPosition = new Vector3(-0.9f, -0.8f, 0.0f);
			transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
			bladeState = BladeState.STANDBY;
			trailTimer = 0.0f;
		}
		
	}

	//BossWingsが存在していることによって、切り替えるスイッチ
	public void SetOn()
	{
		swordBehavior.isUse = true;
	}

	public void SetOff()
	{
		transform.localPosition = new Vector3(-0.9f, -0.8f, 0.0f);
		transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
		bladeState = BladeState.STANDBY;
		trailTimer = 0.0f;

		swordBehavior.isUse = false;
	}

	//今攻撃できるか
	public bool canUse()
	{
		if (bladeState == BladeState.STANDBY)
			return true;
		else
			return false;
	}

	public bool isAlive()
	{
		if (swordBehavior.isUse)
			return true;
		else
			return false;
	}
		
	//攻撃
	public void Attack()
	{
		bladeState = BladeState.TRAIL;
		targetPosition = new Vector3 (transform.localPosition.x - 1.8f, Random.Range (-0.8f, 0.1f), transform.localPosition.z + 2.0f);
		angleRotated = 0.0f;
	}

	//当たり判定
	void OnTriggerEnter(Collider other)
	{
		if (bladeState == BladeState.ATTACK)
		{
			//ノックバックのベクトルの計算
			Vector3 tmpPosition = transform.position;
			transform.localPosition = Vector3.zero;

			Vector3 playerPosition = playerTranform.position;
			Vector3 attackVector = playerPosition - transform.position;

			attackVector = new Vector3 (attackVector.x, 0.0f, attackVector.z);
			attackVector.Normalize ();

			if (other.tag == "Player") 
			{
				audioSource.clip = hitSound;
				audioSource.Play ();
				playerStatus.DamagePlayer (enemyStatus.GetAtk(), attackVector, 25.0f);
			}

			transform.position = tmpPosition;
		}
	}


}
