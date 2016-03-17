using UnityEngine;
using System.Collections;

public class PlayerBlowAttack : MonoBehaviour {

	//エリア中心から飛ばされるか、特定のベクトルで飛ばす
	public bool disappearAuto;
	public bool specifiedDirection;
	public bool tracePlayer;
	public bool targetTracePlayer;
	public float blowSpeed;
	public Vector3 blowVector;

	public float bornTime;
	public float stayTime;

	public Vector3 originalScale;

	private Rigidbody playerRb;
	private PlayerComponent playerComponent;
	private PlayerActionManager playerActionManager;

	void Awake ()
	{
		GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
		playerComponent = playerObject.GetComponent<PlayerComponent> ();
		playerRb = playerObject.GetComponent<Rigidbody> ();
		playerActionManager = playerObject.GetComponent<PlayerActionManager> ();

		transform.position = playerComponent.GetPlayerShoulderPosition ();

		originalScale = transform.localScale;
		disappearAuto = true;
	}

	void OnEnable()
	{
		bornTime = Time.unscaledTime;
	}
	

	void Update () 
	{
		if (Time.timeScale != 0.0f) //ポーズ処理
		{
			if (tracePlayer)
			{
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



		if (disappearAuto && Time.unscaledTime >= bornTime + stayTime)
		{
			gameObject.SetActive (false);
		}

		if (!disappearAuto && playerActionManager.onFloor)
		{
			gameObject.SetActive (false);
		}
	}

	public void SetScale()
	{
		
	}

	public void ResetScale()
	{
		transform.localScale = originalScale;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy") 
		{
			Rigidbody rb;
			EnemyStatus es;

			rb = other.GetComponent<Rigidbody> ();
			es = other.GetComponent<EnemyStatus> ();

			if (!es.canBlow)
				return;


			if (targetTracePlayer)
			{
				other.transform.Translate (0.0f, 0.1f, 0.0f);
				rb.velocity = playerRb.velocity * 0.8f;
					
				return;
			}


			if (specifiedDirection) 
			{
				other.transform.Translate (0.0f, 0.1f, 0.0f);
				rb.velocity = blowVector * blowSpeed;
			}
			else
			{
				blowVector = other.transform.position - transform.position;
				blowVector = new Vector3 (blowVector.x, 0.0f, blowVector.z);

				blowVector = blowVector * 2.0f + transform.up;
				blowVector.Normalize ();

				other.transform.Translate (0.0f, 0.1f, 0.0f);
				rb.velocity = blowVector * blowSpeed;
			}
		}
	}


	void OnTriggerStay(Collider other)
	{
		

		if (other.tag == "Enemy") 
		{
			Rigidbody rb;
			EnemyStatus es;

			rb = other.GetComponent<Rigidbody> ();
			es = other.GetComponent<EnemyStatus> ();

			if (!es.canBlow)
				return;

			if (targetTracePlayer)
			{
				if (playerRb.velocity.y < 0.0f && es.onfloor)
					return;
				
				rb.velocity = playerRb.velocity * 0.8f;
				return;
			}

		}
	}

}
