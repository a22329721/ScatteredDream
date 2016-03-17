using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviour {

	public float hpMax;
	public float hp;

	private float atk = 5.0f;
	private float sightDistance = 20.0f;

	private float attackedTime = 0.0f;
	public float recoverTime;

	private Rigidbody rb;

	private PlayerCrystalSystem playerCrystalSystem;
	private PlayerChangeForm playerChangeForm;
	private List<Animator> ats;

	private PlayerBulletImgSystem playerBulletImgSystem;

	private PlayerActionManager playerActionManager;
	public GameObject EnemyManager;
	private EnemyWaveController enemyWaveController;
	public GameObject damageFilterObject;
	private DamageFilter damageFilter;

	public PlayerActionVoiceManager playerActionVoiceManager;

	void Awake()
	{
		hp = hpMax;
		playerActionManager = GetComponent<PlayerActionManager> ();
		rb = GetComponent<Rigidbody> ();

		playerCrystalSystem = GetComponent<PlayerCrystalSystem> ();
		playerChangeForm = GetComponent<PlayerChangeForm> ();
		Component[] animatorComponents = GetComponentsInChildren(typeof(Animator));

		ats = new List<Animator>();

		for (int i = 0; i < animatorComponents.Length; i++) 
		{
			ats.Add ((Animator)animatorComponents [i]);
		}

		enemyWaveController = EnemyManager.GetComponent<EnemyWaveController> ();

		damageFilter = damageFilterObject.GetComponent<DamageFilter> ();
	}

	void Start()
	{
		playerBulletImgSystem = GetComponent<PlayerBulletImgSystem>();
		playerBulletImgSystem.Initialization (playerActionManager);
	}
	

	void FixedUpdate () {

		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_ATTCKED)
		{
			if (Time.unscaledTime >= attackedTime + recoverTime)
			{
				playerActionManager.playerState = PlayerActionManager.STATE.STATE_READY;
			}
		}

		//プレイヤー死亡
		if (hp <= 0.0f && playerActionManager.playerState != PlayerActionManager.STATE.STATE_DEAD)
		{
			playerActionManager.playerState = PlayerActionManager.STATE.STATE_DEAD;
			playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.DEAD);
			//Debug.Log ((int)playerActionManager.playerState);

			if (playerChangeForm.isNormal)
			{
				ats[0].SetInteger ("state", (int)playerActionManager.playerState);
			} 
			else 
			{
				ats[1].SetInteger ("state", (int)playerActionManager.playerState);
			}

			enemyWaveController.SetGameOver ();
		}

		///////////////////////
		//DEBUG用
		//HP Crystal TimeGage全部回復
		if (Input.GetButtonDown ("GageMax")) 
		{
			playerCrystalSystem.AddCrystalProcess(500.0f);
			hp = hpMax;
			playerChangeForm.TimeGageUp (1500.0f);
		}
		//////////////////////
	}

	public float GetMaxHP()
	{
		return this.hpMax;
	}

	public float GetHP()
	{
		return this.hp;
	}

	public float GetAtk()
	{
		if(playerChangeForm.isNormal)
			return this.atk;
		else
			return this.atk * 2.0f;
	}

	public float GetSightDistance()
	{
		return sightDistance;
	}

	//プレイヤーにダメージを渡る
	public void DamagePlayer(float dmg, Vector3 attackVector, float attackBackDistance)
	{
		if (playerActionManager.playerState != PlayerActionManager.STATE.STATE_DEAD) 
		{
			if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_MAGICALBEAM)
				return;

			hp -= dmg;
			damageFilter.SetDamageFilter ();
			playerCrystalSystem.AddCrystalProcess (dmg);

			if (!playerChangeForm.isNormal)
				return;

			playerActionManager.playerState = PlayerActionManager.STATE.STATE_ATTCKED;
			playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.GETDAMAGE);

			//撃退計算
			if(attackBackDistance != 0.0f)
				rb.velocity = attackVector * attackBackDistance;

			attackedTime = Time.unscaledTime;
		}
	}

	//プレイヤー回復
	public void RecoverPlayer(float recoveryHP)
	{
		this.hp += recoveryHP;

		if(this.hp > this.hpMax)
			this.hp = this.hpMax;
	}

}
