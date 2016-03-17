using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerActionManager : MonoBehaviour {

	//プレイヤー情報
	private Rigidbody playerRb;
	private CapsuleCollider playerCc;
	//private PlayerStatus playerStatus;

	//カメラのY値を更新するためのオブジェクト
	public GameObject cameraCollisionBox;
	public GameObject thirdPersonControllerObject;

	//ジャンプ用
	public CollisionCounter floorProbe;
	public bool onFloor; 
	public float jumpEnergy = 0.0f;

	private SceneController sceneController;
	private ThirdPersonController thirdPersonController;
	private CameraMover cameraMover;

	//アタック用
	private PlayerAttackManager playerAttackManager;
	public PauseManager pauseManager;

	//アニメーターコントローラー
	//private Animator at;
	private List<Animator> ats;
	private PlayerChangeForm playerChangeForm;

	public PlayerActionVoiceManager playerActionVoiceManager;

	public enum ANIMATION_TYPE{
		action,
		state
	};

	public enum ACTION{
		ACTION_STAND,
		ACTION_WALK,
		ACTION_RUN,
		ACTION_JUMP
	};

	public enum STATE{
		STATE_READY,
		STATE_SWORD,		//ライト斬撃
		STATE_SWORDMOVE,	//ヘヴィ斬撃
		STATE_GUN,			//シュート
		STATE_ATTCKED,		//攻撃された
		STATE_DEAD,			//ゲームオーバー
		STATE_MAGICALBEAM		//スローモーションモード中に使える必殺技
	};

	public ACTION playerAction;
	public STATE playerState;

	
	void Start () {
		//シーン状態の取得
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null) {
			Debug.Log ("Third person controller cannot find tag 'SceneManager'");
		}
		
		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		thirdPersonController = thirdPersonControllerObject.GetComponent<ThirdPersonController> ();

		cameraMover = cameraCollisionBox.GetComponent<CameraMover> ();
		playerRb = GetComponent<Rigidbody> ();
		playerCc = GetComponent<CapsuleCollider> ();

		playerAction = ACTION.ACTION_STAND;
		playerState = STATE.STATE_READY;

		playerChangeForm = GetComponent<PlayerChangeForm> ();
		//at = GetComponentInChildren<Animator> (); 
		Component[] animatorComponents = GetComponentsInChildren(typeof(Animator));

		ats = new List<Animator>();

		for (int i = 0; i < animatorComponents.Length; i++) 
		{
			ats.Add ((Animator)animatorComponents [i]);
		}
			
		playerAttackManager = GetComponent<PlayerAttackManager> ();


	}


	void Update () {


		if (playerState == STATE.STATE_DEAD || playerState == STATE.STATE_MAGICALBEAM)
			return;

		//プレイヤーの動作制御
		ActionControll ();

		//カメラのY値を更新する
		//カメラのY値 = プレイヤーのY値 + カメラとプレイヤーの相対Y値
		cameraCollisionBox.transform.position = new Vector3(cameraCollisionBox.transform.position.x,
		                                                    cameraMover.offsetY + transform.position.y,
		                                                    cameraCollisionBox.transform.position.z);

	}

	void ActionControll ()
	{
		//ゲームスタート前とポーズの待機状態
		if (!sceneController.isStart () || pauseManager.isGamePaused()) {
			return;
		}

		//攻撃されたら、動けなくなる
		if (playerState.Equals (STATE.STATE_ATTCKED))
		{
			//アニメーション状態の更新

			if (playerChangeForm.isNormal) {
				ats[0].SetInteger ("state", (int)playerState);
			} else {
				ats[1].SetInteger ("state", (int)playerState);
			}
			return;
		}
			
		bool stateChange = false;

		//斬撃1
		if (Input.GetButtonDown ("LightSword") && playerState != STATE.STATE_SWORDMOVE)
		{
			stateChange = playerAttackManager.SwordAttack();
			if (stateChange) 
			{
				playerState = STATE.STATE_SWORD;
				playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.LIGHTATTACK);
			}
		}

		stateChange = false;

		//斬撃2
		if (Input.GetButtonDown ("HeavySword"))
		{
			stateChange = playerAttackManager.HeavySwordAttack();
			if (stateChange)
			{
				playerState = STATE.STATE_SWORDMOVE;
				playerActionVoiceManager.PlayAudio(PlayerActionVoiceManager.AudioType.HEAVYATTACK);
			}
		}

		stateChange = false;

		//ガンファイヤー
		if (Input.GetButtonDown ("GunShot"))
		{
			stateChange = playerAttackManager.GunFire ();
			if (stateChange) 
			{
				playerState = STATE.STATE_GUN;
				playerAttackManager.ResetAllAttack();
				playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.GUNFIRE);
			}
		}

		//もし攻撃の冷却時間を超えたら、待機状態に戻る
		if(!playerAttackManager.isZeroVelocity() && !playerAttackManager.isInShootState())
		{
			playerState = STATE.STATE_READY;
		}

		if (playerChangeForm.isNormal) {
			ats[0].SetInteger ("state", (int)playerState);
		} else {
			ats[1].SetInteger ("state", (int)playerState);
		}


		//足下に踏めるオブジェクトの判定
		if (floorProbe.counter > 0) {
			onFloor = true;
		} else if (floorProbe.counter == 0) {
			onFloor = false;
		}
		
		//ジャンプ
		if (Input.GetButton ("Jump") && onFloor)
		{
			jumpEnergy += 450.0f * Time.unscaledDeltaTime;
		}

		if ((Input.GetButtonUp("Jump") || jumpEnergy > 100.0f) && onFloor) {

			if(jumpEnergy > 100.0f)
				jumpEnergy = 100.0f;
			if(jumpEnergy < 50.0f)
				jumpEnergy = 50.0f;

			playerRb.velocity = new Vector3 (playerRb.velocity.x, 0.0f, playerRb.velocity.z)
				+ new Vector3(0.0f, 18.0f * jumpEnergy / 100.0f, 0.0f) * (1f / Time.timeScale);

			playerAction = ACTION.ACTION_JUMP;
			playerCc.center = new Vector3(playerCc.center.x, playerCc.center.y, 0.18f);

			//攻撃のジャンプキャンセル
			playerState = STATE.STATE_READY;
			playerAttackManager.ResetAllAttack();

			jumpEnergy = 0;

			playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.JUMP);

		} else if (onFloor
		    && (!thirdPersonController.isHoriMove && !thirdPersonController.isVertiMove)) {

			//攻撃のリセット
			if(playerAction == ACTION.ACTION_JUMP)
			{
				playerState = STATE.STATE_READY;
				playerAttackManager.ResetAllAttack();
			}

			playerAction = ACTION.ACTION_STAND;
			playerCc.center = new Vector3(playerCc.center.x, playerCc.center.y, 0.0f);
			
		} else if (onFloor
		           && (thirdPersonController.isHoriMove || thirdPersonController.isVertiMove)) {

			//攻撃のリセット
			if(playerAction == ACTION.ACTION_JUMP)
			{
				playerState = STATE.STATE_READY;
				playerAttackManager.ResetAllAttack();
			}

			//歩くと走るアニメーションの切り替え
			if (thirdPersonController.moveSpeed > thirdPersonController.inputSpeed) {
				playerAction = ACTION.ACTION_RUN;
				playerCc.center = new Vector3(playerCc.center.x, playerCc.center.y, 0.15f);
				
			} else {
				playerAction = ACTION.ACTION_WALK;
				playerCc.center = new Vector3(playerCc.center.x, playerCc.center.y, 0.12f);
			}
		}

		//穴に落ちる
		if (!playerAction.Equals(ACTION.ACTION_JUMP) && !onFloor) {
			playerAction = ACTION.ACTION_JUMP;
			playerCc.center = new Vector3(playerCc.center.x, playerCc.center.y, 0.18f);
		}

		//アニメーション状態の更新
		if (playerChangeForm.isNormal) {
			ats [0].SetInteger ("action", (int)playerAction);
		} else {
			ats [1].SetInteger ("action", (int)playerAction);
		}

	}

	public void SetAnimation(ANIMATION_TYPE type, int number)
	{
		switch (type) 
		{
		case ANIMATION_TYPE.state:
			if (playerChangeForm.isNormal) {
				ats [0].SetInteger ("state", number);
			} else {
				ats [1].SetInteger ("state", number);
			}
			break;

		case ANIMATION_TYPE.action:
			if (playerChangeForm.isNormal) {
				ats [0].SetInteger ("action", number);
			} else {
				ats [1].SetInteger ("action", number);
			}
			break;

		default:
			break;
		}


	}

	public int bulletMax
	{
		get {return playerAttackManager.bulletMax;}
	}

	public int GetBulletCount()
	{
		return playerAttackManager.GetBulletCount();
	}


}

