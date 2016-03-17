using UnityEngine;
using System.Collections;

public class EnemySwordAttack1 : MonoBehaviour {

	private float atk;

	public Vector3 moveVector;
	public float bornTime = 0.0f;
	public float stayTime = 0.5f;
	public float attackSpeed = 5.0f;

	public float attackBackDistance;

	private TrailRenderer tr;

	private PlayerComponent playerComponent;

	private AudioSource audioSource;


	void Awake () 
	{
		bornTime = Time.unscaledTime;

		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		playerComponent = playerObject.GetComponent<PlayerComponent> ();

		tr = GetComponentInChildren<TrailRenderer> ();

		audioSource = GetComponent<AudioSource> ();
	}

	void OnEnable()
	{
		bornTime = Time.time;
		tr.Clear ();
	}

	void FixedUpdate () {

		transform.position += moveVector * attackSpeed * Time.deltaTime;


		if (Time.time >= bornTime + stayTime)
		{
			gameObject.SetActive (false);
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Vector3 playerPosition = playerComponent.GetPlayerShoulderPosition ();
			Vector3 attackVector = playerPosition - transform.position;

			attackVector = new Vector3 (attackVector.x, 0.0f, attackVector.z);
			attackVector.Normalize ();

			PlayerStatus playerStatus;
			playerStatus = other.GetComponent<PlayerStatus> ();

			playerStatus.DamagePlayer (atk, attackVector, attackBackDistance);

			audioSource.Play ();
		}
	}

	public void SetAtk(float num)
	{
		atk = num;
	}

}
