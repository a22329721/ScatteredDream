using UnityEngine;
using System.Collections;

public class PlayerSwordAttack1 : MonoBehaviour {

	public bool reversedAttack = false;

	public float atk;

	public float stayTime;
	public Vector3 moveNormalVector;
	public float attackSpeed;

	private PlayerComponent playerComponent;
	private Vector3 playerRightShoulderPosition;
	private TrailRenderer tr;
	private float timeNow;

	private PlayerStatus playerStatus;

	private AudioSource audioSource;


	void Awake () {
		timeNow = Time.unscaledTime;

		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		playerComponent = playerObject.GetComponent<PlayerComponent> ();
		tr = GetComponentInChildren<TrailRenderer> ();
		audioSource = GetComponent<AudioSource> ();

		playerRightShoulderPosition = playerComponent.GetPlayerShoulderPosition ();
		playerStatus = playerObject.GetComponent<PlayerStatus> ();
	}

	void OnEnable()
	{
		timeNow = Time.unscaledTime;
		playerRightShoulderPosition = playerComponent.GetPlayerShoulderPosition ();
		tr.Clear ();

		atk = playerStatus.GetAtk ();

		if (reversedAttack)
			moveNormalVector *= -1;
	}

	void FixedUpdate () {
		if (Time.timeScale != 0.0f) //ポーズ処理
		{
			//プレイヤーを中心として回転する
			transform.RotateAround (playerRightShoulderPosition, moveNormalVector, -attackSpeed * Time.unscaledDeltaTime);
		}
		else 
		{
			if (stayTime != 0.0f)
			{
				stayTime += Time.unscaledDeltaTime;
			}
		}

		if (Time.unscaledTime >= timeNow + stayTime) {
			reversedAttack = false;
			gameObject.SetActive(false);
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

			obj.transform.position = transform.position;
			//script.SetColor (Color.blue);
			script.SetSpeed(4.5f);
			obj.SetActive (true);
			script.SetTexture (0);
			magicScript.SetTexture (0);

			EnemyStatus enemyStatus;
			enemyStatus = other.GetComponent<EnemyStatus> ();

			audioSource.Play ();
			enemyStatus.Damage (atk);
		}
	}

	public void SetAtk(float num)
	{
		atk = num;
	}

}
