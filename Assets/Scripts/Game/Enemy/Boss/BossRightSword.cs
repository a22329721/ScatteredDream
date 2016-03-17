using UnityEngine;
using System.Collections;

public class BossRightSword : MonoBehaviour {

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

	private float attackSpeed = 20.0f;
	private Vector3 recordPosition, recordRotation;

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

		//プレイヤーの上に移動
		Vector3 distanceToPlayer = playerTranform.position - transform.position;
		distanceToPlayer.y = playerTranform.position.y + 3.0f - transform.position.y;

		if (distanceToPlayer.magnitude >= 0.1f) 
		{
			distanceToPlayer.Normalize ();
			transform.position += distanceToPlayer * moveSpeed * Time.fixedDeltaTime;
		}
		else 
		{
			transform.position = new Vector3 (playerTranform.position.x, playerTranform.position.y + 3.0f, playerTranform.position.z);
		}

		trailTimer += Time.fixedDeltaTime;
		if (trailTimer >= 2.5f) 
		{
			//色が変わったら、刺す
			if (mr.material.color.r >= 0.1f)
			{
				mr.material.color = Color.Lerp (mr.material.color, Color.cyan, Time.fixedDeltaTime);
				if (mr.material.color.r < 0.1f) 
				{
					audioSource.clip = attackSound;
					audioSource.Play ();
					recordPosition = transform.position;
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
		if (transform.position.y >= 18.0f)
		{
			transform.position = recordPosition + new Vector3(0.0f, -attackSpeed * Time.fixedDeltaTime, 0.0f);
			recordPosition = transform.position;
		}
		else
		{
			//剣の状態を記録して、次のフェーズはそのまま止まる
			mr.material.color = Color.white;
			recordPosition = transform.position;
			recordRotation = transform.eulerAngles;
			bladeState = BladeState.END;
			trailTimer = 0.0f;
		}

	}

	void AttackEnd ()
	{
		//攻撃判定オフ
		collider.isTrigger = false;

		trailTimer += Time.fixedDeltaTime;

		transform.position = recordPosition;
		transform.eulerAngles = recordRotation;

		//リセット
		if (trailTimer >= 2.0f) 
		{
			transform.localPosition = new Vector3(0.9f, -0.8f, 0.0f);
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
		transform.localPosition = new Vector3(0.9f, -0.8f, 0.0f);
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
	}

	//当たり判定
	void OnTriggerEnter(Collider other)
	{
		if (bladeState == BladeState.ATTACK)
		{
			Vector3 playerPosition = playerTranform.position;
			Vector3 attackVector = playerPosition - transform.position;

			//attackVector = new Vector3 (attackVector.x, 0.0f, attackVector.z);
			attackVector = transform.parent.transform.forward;
			attackVector.y = 0.0f;
			attackVector.Normalize ();

			if (other.tag == "Player") 
			{
				audioSource.clip = hitSound;
				audioSource.Play ();
				playerStatus.DamagePlayer (enemyStatus.GetAtk(), attackVector, 25.0f);
			}
		}
	}
}
