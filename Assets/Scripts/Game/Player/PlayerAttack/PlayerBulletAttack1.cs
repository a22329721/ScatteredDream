using UnityEngine;
using System.Collections;

public class PlayerBulletAttack1 : MonoBehaviour {

	public float atk;

	public Vector3 startPoint;
	public Vector3 endPoint;

	private float process;
	public Vector3 controllPoint1;
	public Vector3 controllPoint2;

	public float moveSpeed;
	private float unitDistance = 2.0f;
	private float speedProporty;

	private PlayerComponent playerComponent;
	private PlayerStatus playerStatus;
	private TrailRenderer tr;

	private AudioSource audioSource;


	void Awake ()
	{
		GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
		playerStatus = playerObject.GetComponent<PlayerStatus> ();
		playerComponent = playerObject.GetComponent<PlayerComponent> ();
		startPoint = playerComponent.GetPlayerShoulderPosition ();
		audioSource = GetComponent<AudioSource> ();

		tr = GetComponent<TrailRenderer> ();
	}

	void OnEnable()
	{
		process = 0.0f;

		atk = playerStatus.GetAtk () / 2.0f;

		//速度均等化
		speedProporty = unitDistance / (endPoint - startPoint).magnitude;
		tr.enabled = false;
	}

	//バレットのルート計算(ベジェ曲線)
	void Update ()
	{
		transform.position = Mathf.Pow((1.0f - process), 3) * startPoint 
			+ 3 * process * Mathf.Pow((1.0f - process), 2) * controllPoint1
			+ 3 * Mathf.Pow(process, 2) * (1 - process) * controllPoint2
			+ Mathf.Pow(process, 3) * endPoint;


		if (Time.timeScale != 0.0f) //ポーズ処理
		{
			if (process <= 0.125f)
				process += moveSpeed * Time.unscaledDeltaTime * speedProporty;
			else
				process += moveSpeed * Time.unscaledDeltaTime * speedProporty * 4;
		}
		if (process >= 0.125f && !tr.enabled)
		{
			tr.Clear ();
			tr.enabled = true;
		}
		if (process > 1.2f)
		{
			gameObject.SetActive (false);
			//Debug.Log ("Disappear");
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			GameObject obj;
			BombEffectScript script;
			MagicalEffectScript magicScript;

			obj = EffectManager.current.GetBombEffecrtPooledObject ();
			script = obj.GetComponent<BombEffectScript> ();
			magicScript = obj.GetComponentInChildren<MagicalEffectScript> ();

			//if (obj == null)
				//return;

			obj.transform.position = transform.position;
			//script.SetColor (Color.yellow);
			script.SetSpeed(4.5f);
			obj.SetActive (true);
			script.SetTexture (2);
			magicScript.SetTexture (2);

			EnemyStatus enemyStatus;
			enemyStatus = other.GetComponent<EnemyStatus> ();

			enemyStatus.Damage (atk);
			audioSource.Play ();

			gameObject.SetActive (false);

		}
	}
}
