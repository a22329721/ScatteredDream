using UnityEngine;
using System.Collections;

public class EnemyWaveController : MonoBehaviour {

	//敵を全部倒すと、ゲーム終了
	private bool isEnemyOver;
	private float finishTime;
	private float finishStayTime = 3.0f;
	private SceneController sceneController;
	public FieldBomber fieldBomber;

	public BGMControllerScript BGMController;
	public AudioClip GameBGM, BossBGM;
	private float bossBgmTimer;
	private bool changeClip;

	private int waveNow;
	private int waveMax = 6;

	public GameObject RecoveryItem;
	private Transform bornPlace;
	public Transform enemyBornPlace1, enemyBornPlace2, enemyBornPlace3, enemyBornPlace4, enemyBornPlace5;

	private EnemyPoolerScript enemyPoolerScript;

	//ボスHPゲージ用
	private bool isShowBossHP;
	public GameObject BossHpObject;
	private BossHPScript bossHPScript;
	public GameObject bossAreaDetector;
	private BossAreaDetectorScript bossAreaDetectorScript;

	private AudioSource audioSource;
	public AudioClip GameStart, WinSound, LoseSound;

	public GameObject exitArrowPaint;

	void Start () 
	{
		enemyPoolerScript = GetComponent<EnemyPoolerScript> ();
		waveNow = 0;

		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null) {
			Debug.Log("Cannot fin tag 'SceneManeger'");
		}
		sceneController = sceneControllerObject.GetComponent<SceneController> ();
		isEnemyOver = false;
		finishTime = 10000.0f;

		bossHPScript = BossHpObject.GetComponentInChildren<BossHPScript> ();
		bossAreaDetectorScript = bossAreaDetector.GetComponent<BossAreaDetectorScript> ();

		bornPlace = enemyBornPlace1;
		isShowBossHP = false;

		BGMController.SetVolume (0.3f);

		audioSource = GetComponent<AudioSource> ();
	}

	void FixedUpdate () 
	{

		if (enemyPoolerScript.GetEnemyAmount() == 0)
		{
			if(waveNow <= waveMax)
				waveNow++;

			switch (waveNow)
			{
			case 1:
				audioSource.clip = GameStart;
				audioSource.Play ();

				bornPlace = enemyBornPlace1;

				SetWalkMinion (bornPlace.position + new Vector3 (0.0f, 0.5f, 0.0f), new Vector3 (0.0f, 90.0f, 0.0f));
				//SetBoss (bornPlace.position + new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (0.0f, -90.0f, 0.0f));
				break;

			case 2:
				bornPlace = enemyBornPlace2;

				SetFlyMinion (bornPlace.position + new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (0.0f, 90.0f, 0.0f));

				break;

			case 3:
				bornPlace = enemyBornPlace3;

				SetPotion ();

				SetWalkMinion (bornPlace.position + new Vector3 (0.0f, 1.5f, 1.5f), new Vector3 (0.0f, 30.0f, 0.0f));
				SetWalkMinion (bornPlace.position + new Vector3 (0.0f, 1.5f, -1.5f), new Vector3 (0.0f, 150.0f, 0.0f));
				SetWalkMinion (bornPlace.position + new Vector3 (-1.5f, 1.5f, 0.0f), new Vector3 (0.0f, 270.0f, 0.0f));

				break;

			case 4:
				
				bornPlace = enemyBornPlace4;

				SetPotion ();

				SetFlyMinion (bornPlace.position + new Vector3 (8.0f, 1.5f, 10.0f), new Vector3 (0.0f, 150.0f, 0.0f));
				SetFlyMinion (bornPlace.position + new Vector3 (0.0f, 1.5f, 0.0f), new Vector3 (0.0f, 120.0f, 0.0f));
				SetFlyMinion (bornPlace.position + new Vector3 (-8.0f, 1.5f, -10.0f), new Vector3 (0.0f, 90.0f, 0.0f));
				break;

			case 5:
				
				bornPlace = enemyBornPlace5;

				SetPotion ();

				SetBoss (bornPlace.position + new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (0.0f, 180.0f, 0.0f));

				break;

			case 6:

				BossHpObject.SetActive (false);
				isShowBossHP = false;
				fieldBomber.DestroyField ();
				break;

			default:
				break;
			}

		}

		if (waveNow == 5) 
		{
			Color tmpColor;
			tmpColor = exitArrowPaint.GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");

			if (tmpColor.a < 1.0f) {
				tmpColor.a += Time.fixedDeltaTime * 0.6f;
				if (tmpColor.a > 1.0f)
					tmpColor.a = 1.0f;
				
				exitArrowPaint.GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", new Color (1.0f, 1.0f, 1.0f, tmpColor.a));
			}
		}
		else
		{
			exitArrowPaint.GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", new Color (1.0f, 1.0f, 1.0f, 0.0f));
		}

		if (waveNow == 5 && bossAreaDetectorScript.IsInArea() && !isShowBossHP) 
		{
			BossHpObject.SetActive (true);
			bossHPScript.SetOn ();
			bossHPScript.bossShowHP = 0.0f;
			isShowBossHP = true;

			BGMController.SetVolume (0.0f);
			bossBgmTimer = 0.0f;
			changeClip = false;
		}

		bossBgmTimer += Time.unscaledDeltaTime;

		if (bossBgmTimer >= 1.0 && waveNow == 5 && !changeClip && isShowBossHP)
		{
			BGMController.SetClip (BossBGM);
			BGMController.Play ();
			BGMController.SetVolume (0.3f);
			changeClip = true;
		}

		//Win
		if (waveNow == waveMax + 1 && !isEnemyOver)
		{
			audioSource.clip = WinSound;
			audioSource.Play ();
		}

		//Lose
		if (waveNow == waveMax + 2 && !isEnemyOver)
		{
			audioSource.clip = LoseSound;
			audioSource.Play ();
		}


		//全部クリア
		if (waveNow > waveMax && !isEnemyOver)
		{
			isEnemyOver = true;
			finishTime = Time.time + finishStayTime;
			sceneController.LevelNowPlus (1);
		}

		if (Time.time > finishTime)
		{
			BGMController.SetVolume (0.0f);
			sceneController.sceneClose();
		}

	}

	private void SetWalkMinion(Vector3 position, Vector3 rotation)
	{
		GameObject obj;

		obj = EnemyPoolerScript.current.GetWalkMinionPooledObject ();

		if (obj == null)
			return;

		obj.transform.position = position;
		obj.transform.eulerAngles = rotation;

		obj.SetActive (true);
	}

	private void SetFlyMinion(Vector3 position, Vector3 rotation)
	{
		GameObject obj;

		obj = EnemyPoolerScript.current.GetFlyMinionPooledObject ();

		if (obj == null)
			return;

		obj.transform.position = position;
		obj.transform.eulerAngles = rotation;

		obj.SetActive (true);
	}

	private void SetBoss(Vector3 position, Vector3 rotation)
	{
		GameObject obj;

		obj = EnemyPoolerScript.current.GetBossPooledObject ();

		if (obj == null)
			return;

		obj.transform.position = position;
		obj.transform.eulerAngles = rotation;

		obj.SetActive (true);
	}

	private void SetPotion ()
	{
		RecoveryItem.transform.position = EnemyPoolerScript.current.GetLastEnemyPosition() + Vector3.up;
		RecoveryItem.SetActive (true);
	}

	public void SetGameOver()
	{
		waveNow = waveMax + 2;
	}


}
