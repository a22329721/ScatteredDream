using UnityEngine;
using System.Collections;

public class BGMControllerScript : MonoBehaviour
{
	private float volumeNow, volumeTarget;
	private float volumeMax = 0.5f;

	private float speed = 1.5f;

	private AudioSource audioSource;

	void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
		volumeNow = audioSource.volume;
		volumeTarget = volumeNow;
	}
	

	void FixedUpdate () 
	{
		if (volumeTarget > volumeMax)
			volumeTarget = volumeMax;

		if (Mathf.Abs (volumeTarget - volumeNow) > 0.05f) 
		{
			volumeNow = Mathf.Lerp (volumeNow, volumeTarget, speed * Time.unscaledDeltaTime);
		}
		else
		{
			volumeNow = volumeTarget;
		}
		audioSource.volume = volumeNow;
	}

	//音量を調整する
	public void SetVolume(float input)
	{
		volumeTarget = input;
	}

	//音楽を変更する
	public void SetClip(AudioClip inputFile)
	{
		audioSource.clip = inputFile;
	}

	//音楽再生
	public void Play()
	{
		audioSource.Play ();
	}

}
