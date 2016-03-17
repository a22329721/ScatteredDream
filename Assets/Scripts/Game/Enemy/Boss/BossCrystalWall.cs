using UnityEngine;
using System.Collections;

public class BossCrystalWall : MonoBehaviour {

	private EnemyStatus enemyStatus;
	private Rigidbody rb;

	public float lifeTime;

	public Vector3 moveVector;
	private float moveSpeed = 6.0f;


	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		enemyStatus = GetComponent<EnemyStatus> ();
	}

	void OnEnable()
	{
		lifeTime = 3.0f;
	}

	void FixedUpdate () 
	{
		rb.velocity = moveVector * moveSpeed;

		if (enemyStatus.enemyState != EnemyStatus.EnemyState.ENEMY_DEAD) 
		{
			
			lifeTime -= Time.fixedDeltaTime;

			if (lifeTime <= 0.0f || enemyStatus.hp <= 0.0f) 
			{
				enemyStatus.enemyState = EnemyStatus.EnemyState.ENEMY_DEAD;
			}
		}
	}
}
