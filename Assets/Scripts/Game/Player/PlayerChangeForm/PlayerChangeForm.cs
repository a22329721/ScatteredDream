using UnityEngine;
using System.Collections;

public class PlayerChangeForm : MonoBehaviour {

	public bool isNormal;

	public GameObject normalForm;
	public GameObject magicForm;

	private bool isReady;
	private float timeGageMax = 300.0f;
	private float timeGage = 0.0f;

	private float timeMax = 15.0f;
	private float timeNow;

	public GameObject missBlue;
	private MissBLUEStarMine starMine;

	private bool canUseBeam;
	private float excaliburTime;
	public float excaliburTimeMax;

	public GameObject kiraFilterObject;
	private KiraFilter kiraFilter;

	private PlayerStarEffect playerStarEffect;
	private PlayerActionManager playerActionManager;
	private Rigidbody rb;

	public GameObject excaliburObject;

	public PauseManager pauseManager;

	public BGMControllerScript BGMController;
	public PlayerChangeFormBGM playerChangeFormBGM;
	private PlayerChangeFormVoice playerChangeFormVoice;

	void Start () 
	{
		kiraFilter = kiraFilterObject.GetComponent<KiraFilter> ();
		playerStarEffect = GetComponentInChildren<PlayerStarEffect> ();
		playerActionManager = GetComponent<PlayerActionManager> ();
		rb = GetComponent<Rigidbody> ();
		playerChangeFormVoice = GetComponentInChildren<PlayerChangeFormVoice> ();

		starMine = missBlue.GetComponent<MissBLUEStarMine> ();
		isNormal = true;
		isReady = false;
		canUseBeam = false;
	}

	void Update ()
	{
		//変身時の処理
		if (!isNormal)
		{

			//エクスキャリバーを使う
			if (Input.GetButtonDown ("ChangeForm") && canUseBeam && !pauseManager.isGamePaused()
				&& (playerActionManager.playerState == PlayerActionManager.STATE.STATE_READY)) 
			{
				canUseBeam = false;
				playerChangeFormVoice.PlayAudio (PlayerChangeFormVoice.AudioType.MAXDRIVE);
				rb.velocity = Vector3.zero;

				starMine.ShootingStar ();
				playerActionManager.playerState = PlayerActionManager.STATE.STATE_MAGICALBEAM;
				playerActionManager.SetAnimation (PlayerActionManager.ANIMATION_TYPE.state, (int)playerActionManager.playerState);

			}
				

			if(Time.timeScale != 0.0f)
				timeNow -= Time.unscaledDeltaTime;
			
			if (timeNow < 0.0f)
				timeNow = 0.0f;
			
			timeGage = (timeNow / timeMax) * timeGageMax;


			//普通状態に戻る
			if (timeNow <= 0.0f && playerActionManager.playerState != PlayerActionManager.STATE.STATE_MAGICALBEAM) 
			{
				BGMController.SetVolume (0.3f);
				playerChangeFormBGM.Stop ();
				playerChangeFormVoice.PlayAudio (PlayerChangeFormVoice.AudioType.CHANGEFORMBACK);
				timeNow = 0.0f;
				Time.timeScale = 1.0f;
				playerStarEffect.SetStarEffect (new Color (55.0f / 255.0f, 138.0f / 255.0f, 234.0f / 255.0f, 0.5f), 200);
				kiraFilter.SetKiraFilter (false);
				isNormal = true;
			}
		}

		//変身
		if (Input.GetButtonDown ("ChangeForm") && isReady && !pauseManager.isGamePaused())
		{
			BGMController.SetVolume (0.0f);
			playerChangeFormBGM.Play ();
			playerChangeFormVoice.PlayAudio (PlayerChangeFormVoice.AudioType.CHANGEFORMTO);
			isNormal = false;
			isReady = false;
			canUseBeam = true;
			timeNow = timeMax;
			playerStarEffect.SetStarEffect (new Color (150.0f / 255.0f, 55.0f / 255.0f, 234.0f / 255.0f, 0.5f), 200);
			kiraFilter.SetKiraFilter (true);
			Time.timeScale = 0.55f;
		}

		/////////////////モデル切り替え////
		//普通状態に戻る
		if (isNormal && magicForm.activeInHierarchy) 
		{
			normalForm.SetActive (true);
			magicForm.SetActive (false);
		}

		//変身
		if (!isNormal && normalForm.activeInHierarchy) 
		{
			normalForm.SetActive (false);
			magicForm.SetActive (true);
		}
		//////////////////////////////////////

	}

	//ゲージ貯め
	public void TimeGageUp(float data)
	{
		if (timeGage < timeGageMax && isNormal)
		{
			timeGage += data;

			if (timeGage >= timeGageMax)
			{
				timeGage = timeGageMax;
				isReady = true;
			}
		}
	}

	//ゲージ状態の取得
	public float GetTimeGagePercentage()
	{
		return timeGage / timeGageMax;
	}

	public void ReleaseBeamState()
	{
		timeNow = 0.0f;
		playerActionManager.playerState = PlayerActionManager.STATE.STATE_READY;
		playerActionManager.SetAnimation (PlayerActionManager.ANIMATION_TYPE.state, (int)playerActionManager.playerState);
	}

}
