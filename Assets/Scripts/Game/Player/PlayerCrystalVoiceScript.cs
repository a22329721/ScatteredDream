using UnityEngine;
using System.Collections;

public class PlayerCrystalVoiceScript : MonoBehaviour {

	private AudioSource audioSource;

	public AudioClip CrystalON;
	public AudioClip CrystalShoot;
	public AudioClip CrystalTeleport;

	public enum AudioType{
		CRYSTALON,
		CRYSTALSHOOT,
		CRYSTALTELEPORT
	};

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayAudio(AudioType type)
	{
		switch (type) 
		{
		case AudioType.CRYSTALON:
			audioSource.clip = CrystalON;
			break;

		case AudioType.CRYSTALSHOOT:
			audioSource.clip = CrystalShoot;
			break;

		case AudioType.CRYSTALTELEPORT:
			audioSource.clip = CrystalTeleport;
			break;

		default:
			break;
		}

		audioSource.Play ();
	}
}
