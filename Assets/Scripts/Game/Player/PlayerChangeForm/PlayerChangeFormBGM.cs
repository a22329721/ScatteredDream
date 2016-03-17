using UnityEngine;
using System.Collections;

public class PlayerChangeFormBGM : MonoBehaviour {

	private AudioSource audioSource;

	public AudioClip ChangeFormBGM, ChangeFormBackEffect;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void Play()
	{
		audioSource.loop = true;
		audioSource.clip = ChangeFormBGM;
		audioSource.volume = 0.22f;
		audioSource.Play ();
	}

	public void Stop()
	{
		audioSource.Stop ();

		audioSource.loop = false;
		audioSource.clip = ChangeFormBackEffect;
		audioSource.volume = 1.0f;
		audioSource.Play ();
	}

}
