using UnityEngine;
using System.Collections;
using CrystalManeuver;
public class PlayerCrystalSystem :MonoBehaviour
{
	public GameObject attachToGobj;
	public GameObject prefabGobj;
	public GameObject crystalPrefabGobj;
	public Transform playerTransform;
	public Transform cameraTransform;
	public Transform lookAtTransform;
	private bool isExSkill;
	public int crystalNumber;

	private float actcrystalInterval = 0.0f;
	private bool actcrystalAllowed = true;
	private IPlayerCrystalManeuver PCM;

	private float crystalEnergyMax = 60.0f;

	private PlayerActionManager playerActionManager;
	private PlayerCrystalVoiceScript playerCrystalVoice;

	void Awake()
	{
		PCM = new PlayerCrystalManeuver (attachToGobj, prefabGobj, crystalNumber,  crystalPrefabGobj ,playerTransform, cameraTransform, lookAtTransform);
		//Debug.Log (lookAtTransform.position);
	}
	void Start () 
	{
		playerCrystalVoice = GetComponentInChildren<PlayerCrystalVoiceScript> ();
		playerActionManager = GetComponent<PlayerActionManager> ();
	}

	void Update ()
	{

		//クリスタルボタンを押す && actcrystalAllowed
		if (Input.GetButtonDown ("Crystal") && actcrystalAllowed && playerActionManager.playerState != PlayerActionManager.STATE.STATE_MAGICALBEAM) 
		{
			int actType = 0;

			//クリスタルアクションを起動する
			actType = PCM.ActCrystal ();

			if(actType != 0)
				actcrystalAllowed = false;

			if (actType == 1) 
			{
				playerCrystalVoice.PlayAudio (PlayerCrystalVoiceScript.AudioType.CRYSTALON);
			}
			else if (actType == 2)
			{
				playerCrystalVoice.PlayAudio (PlayerCrystalVoiceScript.AudioType.CRYSTALSHOOT);
			}
			else if (actType == 3)
			{
				playerCrystalVoice.PlayAudio (PlayerCrystalVoiceScript.AudioType.CRYSTALTELEPORT);
			}
		}

		//短い冷却時間(二回押すことを防止するため)
		if(!actcrystalAllowed) 
		{
			actcrystalInterval += Time.unscaledDeltaTime;
			if (actcrystalInterval >= 0.2f) 
			{
				actcrystalInterval = 0;
				actcrystalAllowed = true;
			}
		}

		PCM.UpdateCrystalColorInFly ();
	
	}

	//クリスタルアクションリセット
	public void ConsumeCrystal()
	{
		PCM.ConsumeCrystal ();
		actcrystalAllowed = false;
	}

	public void AddCrystalProcess(float energy)
	{
		PCM.DrawCrystal (energy / crystalEnergyMax);
	}

	public bool IsBlueScreen ()
	{
		return PCM.IsBlueScreen;
	}

	public bool IsNextExSkill()
	{
		return PCM.IsNextExSkill;
	}
}
