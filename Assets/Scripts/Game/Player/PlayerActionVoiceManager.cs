using UnityEngine;
using System.Collections;

public class PlayerActionVoiceManager : MonoBehaviour {

	private AudioSource audioSource;

	public AudioClip LightAttack;
	public AudioClip HeavyAttack;
	public AudioClip GunFire;
	public AudioClip GunReload;
	public AudioClip Jump;
	public AudioClip GetDamage;
	public AudioClip Dead;

	public enum AudioType{
		LIGHTATTACK,
		HEAVYATTACK,
		GUNFIRE,
		GUNRELOAD,
		JUMP,
		GETDAMAGE,
		DEAD
	};

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayAudio(AudioType type)
	{
		switch (type) 
		{
		case AudioType.LIGHTATTACK:
			audioSource.volume = 1.0f;
			audioSource.clip = LightAttack;
			break;

		case AudioType.HEAVYATTACK:
			audioSource.volume = 1.0f;
			audioSource.clip = HeavyAttack;
			break;

		case AudioType.GUNFIRE:
			audioSource.volume = 0.3f;
			audioSource.clip = GunFire;
			break;

		case AudioType.GUNRELOAD:
			audioSource.volume = 1.0f;
			audioSource.clip = GunReload;
			break;

		case AudioType.JUMP:
			audioSource.volume = 1.0f;
			audioSource.clip = Jump;
			break;

		case AudioType.GETDAMAGE:
			audioSource.volume = 1.0f;
			audioSource.clip = GetDamage;
			break;

		case AudioType.DEAD:
			audioSource.volume = 1.0f;
			audioSource.clip = Dead;
			break;

		default:
			break;
		}

		audioSource.Play ();
	}
}
