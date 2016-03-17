using UnityEngine;
using System.Collections;

public class PlayerSwordAttack2 : MonoBehaviour {

	//自動的に消える、それともプレイヤーが着地する時に消える。
	public bool disappearAuto;
	public float stayTime;
	public Vector3 moveVector;
	public float attackSpeed;

	public float atk;

	private PlayerActionManager playerActionManager;
	private Rigidbody playerRb;
	private PlayerStatus playerStatus;
	private TrailRenderer tr;

	private AudioSource audioSource;

	private float timeNow;

	void Awake () {
		timeNow = Time.unscaledTime;

		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

		playerActionManager = playerObject.GetComponent<PlayerActionManager> ();
		playerStatus = playerObject.GetComponent<PlayerStatus> ();
		playerRb = playerObject.GetComponent<Rigidbody> ();
		tr = GetComponentInChildren<TrailRenderer> ();
		audioSource = GetComponent<AudioSource> ();
	}

	void OnEnable()
	{
		timeNow = Time.unscaledTime;
		atk = playerStatus.GetAtk () * 1.5f;
		tr.Clear ();
	}

	void Update () {

		if (Time.timeScale != 0.0f) //ポーズ処理
		{
			if (disappearAuto) {
				transform.position += (moveVector * attackSpeed * Time.unscaledDeltaTime);
			} else {
				transform.position += playerRb.velocity * Time.unscaledDeltaTime;
			}
		} 
		else 
		{
			if (stayTime != 0.0f)
			{
				stayTime += Time.unscaledDeltaTime;
			}
		}

		if (disappearAuto && Time.unscaledTime >= timeNow + stayTime) {
			gameObject.SetActive(false);
		}

		if (!disappearAuto && playerActionManager.onFloor && stayTime == 0.0f) {
			gameObject.SetActive(false);
		}

		if (!disappearAuto && Time.unscaledTime >= timeNow + stayTime && stayTime > 0.0f) {
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

			//if (obj == null)
			//return;

			obj.transform.position = transform.position;
			//script.SetColor (Color.red);
			script.SetSpeed(4.5f);
			obj.SetActive (true);
			script.SetTexture (1);
			magicScript.SetTexture (1);

			EnemyStatus enemyStatus;
			enemyStatus = other.GetComponent<EnemyStatus> ();

			audioSource.Play ();
			enemyStatus.DamageWithMove (atk);
		}
	}

	public void SetAtk(float num)
	{
		atk = num;
	}

}
