using UnityEngine;
using System.Collections;

public class EnemyFireBall : MonoBehaviour {

	public float atk;

	public Vector3 startPoint;
	public Vector3 endPoint;

	private float process;

	public float moveSpeed;
	private float unitDistance = 2.0f;
	private float speedProporty;
	public float attackBackDistance;

	private EnemyStatus enemyStatus;
	private TrailRenderer tr;

	private PlayerComponent playerComponent;
	private PlayerStatus playerStatus;


	void Awake ()
	{
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		playerComponent = playerObject.GetComponent<PlayerComponent> ();
		playerStatus = playerObject.GetComponent<PlayerStatus> ();

		tr = GetComponentInChildren<TrailRenderer> ();
	}

	void OnEnable()
	{
		process = 0.0f;

		//速度均等化
		speedProporty = unitDistance / (endPoint - startPoint).magnitude;

		if(tr != null)
			tr.Clear ();
	}

	//バレットのルート計算(ベジェ曲線)
	void Update ()
	{
		transform.position = (1.0f - process) * startPoint + process * endPoint;

		process += moveSpeed * Time.deltaTime * speedProporty;
			
		if (process > 2.0f)
		{
			gameObject.SetActive (false);
		}
	}

	//命中計算
	void OnTriggerEnter(Collider other)
	{
		
		Vector3 playerPosition = playerComponent.GetPlayerShoulderPosition ();
		Vector3 attackVector = playerPosition - transform.position;

		attackVector = new Vector3 (attackVector.x, 0.0f, attackVector.z);
		attackVector.Normalize ();

		if (other.tag == "Player")
		{
			playerStatus.DamagePlayer (atk, attackVector, attackBackDistance);
		}
		gameObject.SetActive (false);
	}

}
