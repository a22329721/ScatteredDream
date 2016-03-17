using UnityEngine;
using System.Collections;

public class EnemySoundController : MonoBehaviour {

	private AudioSource audioSource;
	public AudioClip GetDamage, Dead;

	public enum SoundType{
		GETDAMAGE,
		DEAD
	};

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
	}
	

	void Update () 
	{
	
	}

	public void SetSound(SoundType type)
	{
		switch (type) 
		{
		case SoundType.GETDAMAGE:
			audioSource.clip = GetDamage;
			break;

		case SoundType.DEAD:
			audioSource.clip = Dead;
			break;

		default:
			break;
		}

		audioSource.Play ();
	}

}
