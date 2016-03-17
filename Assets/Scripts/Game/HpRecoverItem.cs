using UnityEngine;
using System.Collections;

public class HpRecoverItem : MonoBehaviour {

	private bool isUse;

	private float rotationSpeed = 100.0f;
	private float bornTime;

	public CollisionCounter floorProbe;
	public bool fallDown;

	public float avoidTime = 10.0f;
	private bool avoidUp = false;
	private float twinkleSpeed = 1.0f;

	public float stayTime = 15.0f;

	private MeshRenderer mr;
	private Rigidbody rb;

	private AudioSource audioSource;

	void Awake () 
	{
		mr = GetComponent<MeshRenderer> ();
		rb = GetComponent<Rigidbody> ();
		audioSource = GetComponent<AudioSource>();
	}

	void OnEnable()
	{
		mr.material.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);	
		bornTime = Time.time;
		isUse = true;

		floorProbe.counter = 0;
		fallDown = true;
	}

	void FixedUpdate () 
	{
		if (floorProbe.counter > 0)
			fallDown = false;
		else
			fallDown = true;


		if (fallDown) 
		{
			rb.velocity -= new Vector3 (0.0f, 3.0f * Time.fixedDeltaTime, 0.0f);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}


		//回転
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y + rotationSpeed * Time.deltaTime, transform.eulerAngles.z);


		//ピカピカする
		if (Time.time >= bornTime + avoidTime) 
		{
			if (avoidUp) 
			{
				mr.material.color = new Color (1.0f, 1.0f, 1.0f, mr.material.color.a + twinkleSpeed * Time.deltaTime);	
				if (mr.material.color.a >= 0.9f)
				{
					mr.material.color = new Color (1.0f, 1.0f, 1.0f, 0.9f);	
					avoidUp = false;
				}
			} 
			else
			{
				mr.material.color = new Color (1.0f, 1.0f, 1.0f, mr.material.color.a - twinkleSpeed * Time.deltaTime);	
				if (mr.material.color.a <= 0.4f)
				{
					mr.material.color = new Color (1.0f, 1.0f, 1.0f, 0.4f);	
					avoidUp = true;
				}
			}
		}


		if(Time.time >= bornTime + stayTime || (!isUse && !audioSource.isPlaying))
		{
			gameObject.SetActive(false);
		}
	
	}


	void OnTriggerEnter(Collider other)
	{
		//プレイヤー回復
		if (other.tag == "Player" && isUse)
		{
			audioSource.Play ();
			PlayerStatus playerStatus = other.GetComponent<PlayerStatus> ();
			playerStatus.RecoverPlayer (playerStatus.GetMaxHP ());
			isUse = false;
			mr.material.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);	
			//gameObject.SetActive (false);
		}
			
	}
}
