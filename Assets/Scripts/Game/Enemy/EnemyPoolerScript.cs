using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPoolerScript : MonoBehaviour {

	public static EnemyPoolerScript current;

	//歩く雑魚
	public GameObject walkMinionPooledObject;
	public int walkMinionPooledAmount = 3;
	List<GameObject> walkMinionPooledObjects;

	//飛ぶ雑魚
	public GameObject flyMinionPooledObject;
	public int flyMinionPooledAmount = 3;
	List<GameObject> flyMinionPooledObjects;

	//ボス
	public GameObject bossPooledObject;
	public int bossPooledAmount = 1;
	List<GameObject> bossPooledObjects;

	public bool willGrow = true;

	//モンスターのターゲット得点計算
	private Transform playerTransform;
	private PlayerStatus playerStatus;
	private Transform cameraTransform;
	public GameObject defaultGunTarget;

	private Vector3 lastEnemyPosition;

	void Awake()
	{
		current = this;

		GameObject playerobject = GameObject.FindWithTag ("Player");
		playerStatus = playerobject.GetComponent<PlayerStatus> ();
		playerTransform = playerobject.GetComponent<Transform> ();
		cameraTransform = Camera.main.transform;
	}

	void Start () 
	{
		//歩く雑魚初期化
		walkMinionPooledObjects = new List<GameObject> ();
		for (int i = 0; i < walkMinionPooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(walkMinionPooledObject);
			obj.SetActive(false);
			walkMinionPooledObjects.Add(obj);
		}

		//飛ぶ雑魚初期化
		flyMinionPooledObjects = new List<GameObject> ();
		for (int i = 0; i < flyMinionPooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(flyMinionPooledObject);
			obj.SetActive(false);
			flyMinionPooledObjects.Add(obj);
		}

		//ボス初期化
		bossPooledObjects = new List<GameObject> ();
		for (int i = 0; i < bossPooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(bossPooledObject);
			obj.SetActive(false);
			bossPooledObjects.Add(obj);
		}


	}

	//歩く雑魚を作る
	public GameObject GetWalkMinionPooledObject()
	{
		for (int i = 0; i < walkMinionPooledObjects.Count; i++)
		{
			if(!walkMinionPooledObjects[i].activeInHierarchy)
			{
				return walkMinionPooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(walkMinionPooledObject);
			walkMinionPooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	//飛ぶ雑魚を作る
	public GameObject GetFlyMinionPooledObject()
	{
		for (int i = 0; i < flyMinionPooledObjects.Count; i++)
		{
			if(!flyMinionPooledObjects[i].activeInHierarchy)
			{
				return flyMinionPooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(flyMinionPooledObject);
			flyMinionPooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	//ボスを作る
	public GameObject GetBossPooledObject()
	{
		for (int i = 0; i < bossPooledObjects.Count; i++)
		{
			if(!bossPooledObjects[i].activeInHierarchy)
			{
				return bossPooledObjects[i];
			}
		}

		/*
		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(bossPooledObject);
			bossPooledObjects.Add(obj);
			return obj;
		}*/

		return null;
	}


	//得点一番高いモンスターは銃のターゲットになる
	public Transform getTargetEnemyTransform()
	{
		GameObject targetEnemy;
		float targetScore = 0.0f, tmpScore = 0.0f;

		Vector3 vectorToPlayer, vectorFromCamera;
		float cosAngleWithCameraLookAt;

		//初期値、もし攻撃範囲内にモンスターがいないなら、デフォルトターゲットを攻撃する
		targetEnemy = defaultGunTarget;

		//歩く雑魚の得点計算
		for (int i = 0; i < walkMinionPooledObjects.Count; i++)
		{
			if(walkMinionPooledObjects[i].activeInHierarchy)
			{
				vectorToPlayer = playerTransform.position - walkMinionPooledObjects[i].transform.position;
				vectorFromCamera = walkMinionPooledObjects[i].transform.position - cameraTransform.position;

				if (vectorToPlayer.magnitude <= playerStatus.GetSightDistance ())
				{
					//カメラとの角度、カメラの中心に近づいたら高い得点を貰える
					cosAngleWithCameraLookAt = Vector3.Dot (vectorFromCamera, cameraTransform.forward)
						/ (vectorFromCamera.magnitude * cameraTransform.forward.magnitude);

					cosAngleWithCameraLookAt += 1.0f;

					//スコア加算
					tmpScore = cosAngleWithCameraLookAt;

					if (tmpScore > targetScore)
					{
						targetEnemy = walkMinionPooledObjects [i];
						targetScore = tmpScore;
					}
				}
			}
		}

		//飛ぶ雑魚の得点計算
		for (int i = 0; i < flyMinionPooledObjects.Count; i++)
		{
			if(flyMinionPooledObjects[i].activeInHierarchy)
			{
				vectorToPlayer = playerTransform.position - flyMinionPooledObjects[i].transform.position;
				vectorFromCamera = flyMinionPooledObjects[i].transform.position - cameraTransform.position;

				if (vectorToPlayer.magnitude <= playerStatus.GetSightDistance ())
				{
					//カメラとの角度、カメラの中心に近づいたら高い得点を貰える
					cosAngleWithCameraLookAt = Vector3.Dot (vectorFromCamera, cameraTransform.forward)
						/ (vectorFromCamera.magnitude * cameraTransform.forward.magnitude);

					cosAngleWithCameraLookAt += 1.0f;

					//スコア加算
					tmpScore = cosAngleWithCameraLookAt;

					if (tmpScore > targetScore)
					{
						targetEnemy = flyMinionPooledObjects [i];
						targetScore = tmpScore;
					}
				}
			}
		}

		//部位破壊できるボスの処理
		EnemyStatus enemyStatus;

		//ボスの得点計算
		for (int i = 0; i < bossPooledObjects.Count; i++)
		{
			if(bossPooledObjects[i].activeInHierarchy)
			{
				enemyStatus = bossPooledObjects [i].GetComponent<EnemyStatus> ();

				//破壊できる部位も全部計算する
				if (enemyStatus.haveParts) 
				{
					for (int j = 0; j < bossPooledObjects [i].transform.childCount; j++) 
					{
						//部位はまだ存在している
						if (bossPooledObjects [i].transform.GetChild (j).gameObject.activeInHierarchy)
						{
							vectorToPlayer = playerTransform.position - bossPooledObjects [i].transform.GetChild (j).gameObject.transform.position;
							vectorFromCamera = bossPooledObjects [i].transform.GetChild (j).gameObject.transform.position - cameraTransform.position;

							if (vectorToPlayer.magnitude <= playerStatus.GetSightDistance ()) {
								//カメラとの角度、カメラの中心に近づいたら高い得点を貰える
								cosAngleWithCameraLookAt = Vector3.Dot (vectorFromCamera, cameraTransform.forward)
								/ (vectorFromCamera.magnitude * cameraTransform.forward.magnitude);

								cosAngleWithCameraLookAt += 1.0f;

								//スコア加算
								tmpScore = cosAngleWithCameraLookAt;

								if (tmpScore > targetScore) {
									targetEnemy = bossPooledObjects [i].transform.GetChild (j).gameObject;
									targetScore = tmpScore;
								}
							}
						}
					}
				}
					
			}
		}


		return targetEnemy.transform;
	}

	//現時点で生きているモンスターの数量
	public int GetEnemyAmount()
	{
		int amount = 0;

		for (int i = 0; i < walkMinionPooledObjects.Count; i++)
		{
			if(walkMinionPooledObjects[i].activeInHierarchy)
			{
				amount++;
			}
		}

		for (int i = 0; i < flyMinionPooledObjects.Count; i++)
		{
			if(flyMinionPooledObjects[i].activeInHierarchy)
			{
				amount++;
			}
		}

		for (int i = 0; i < bossPooledObjects.Count; i++)
		{
			if(bossPooledObjects[i].activeInHierarchy)
			{
				amount++;
			}
		}

		return amount;
	}

	public void SetLastEnemyPosition(Vector3 inputPosition)
	{
		lastEnemyPosition = new Vector3 (inputPosition.x, inputPosition.y, inputPosition.z);
	}

	public Vector3 GetLastEnemyPosition()
	{
		return new Vector3(lastEnemyPosition.x, lastEnemyPosition.y, lastEnemyPosition.z);
	}

}
