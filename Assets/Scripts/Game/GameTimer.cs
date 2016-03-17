using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour {

	public float timeLimit, timeDelay = 0.0f;
	public float timeBegin, timeNow;
	public float timeShow;

	public GameObject sunLight;
	public GameObject enemyManagerObject;
	private EnemyWaveController enemyWaveController;

	private int AlertTime;
	private AudioSource audioSource;
	public AudioClip TimerAlert1, TimerAlert2;

	private bool gameOver;

	void Start () 
	{
		enemyWaveController = enemyManagerObject.GetComponent<EnemyWaveController> ();
		audioSource = GetComponent<AudioSource> ();

		timeBegin = Time.unscaledTime;
		gameOver = false;
		AlertTime = 0;
	}

	void Update () 
	{

		timeNow = Time.unscaledTime;

		if (Time.timeScale != 0.0f && timeDelay != 0.0f) 
		{
			timeBegin += timeDelay;
			timeDelay = 0.0f;
		}

		if (Time.timeScale == 0.0f)
		{
			timeDelay += Time.unscaledDeltaTime;
		}
		else
		{
			sunLight.transform.eulerAngles = new Vector3 (70 - 85 * ((timeNow - timeBegin) / timeLimit), 
				sunLight.transform.eulerAngles.y, sunLight.transform.eulerAngles.z);

			timeShow = timeLimit - (timeNow - timeBegin);
		}

		//タイムアップ
		if (Time.unscaledTime >= timeBegin + timeLimit && !gameOver && Time.timeScale != 0.0f) 
		{
			Debug.Log (Time.unscaledTime);
			Debug.Log (timeBegin);
			Debug.Log (timeLimit);
			enemyWaveController.SetGameOver ();
			gameOver = true;
		}

		if (timeShow <= 60.0f && AlertTime == 0) 
		{
			audioSource.clip = TimerAlert1;
			audioSource.Play ();
			AlertTime = 1;
		}
		else if (timeShow <= 20.0f && AlertTime == 1) 
		{
			audioSource.clip = TimerAlert2;
			audioSource.Play ();
			AlertTime = 2;
		}
	
	}

	public float GetTimeLimit()
	{
		return timeShow;
	}
}
