using UnityEngine;
using System.Collections;

public class PlayerChangeFormVoice : MonoBehaviour {

	private AudioSource audioSource;

	public AudioClip ChangeFormTo;
	public AudioClip ChangeFormBack;
	public AudioClip MaxDrive;

	public enum AudioType{
		CHANGEFORMTO,
		CHANGEFORMBACK,
		MAXDRIVE
	};

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayAudio(AudioType type)
	{
		switch (type) 
		{
		case AudioType.CHANGEFORMTO:
			audioSource.clip = ChangeFormTo;
			break;

		case AudioType.CHANGEFORMBACK:
			audioSource.clip = ChangeFormBack;
			break;

		case AudioType.MAXDRIVE:
			audioSource.clip = MaxDrive;
			break;

		default:
			break;
		}

		audioSource.Play ();
	}
}
