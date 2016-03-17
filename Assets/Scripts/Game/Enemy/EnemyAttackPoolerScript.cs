using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttackPoolerScript : MonoBehaviour {

	public static EnemyAttackPoolerScript current;

	//飛ぶ雑魚とボスファイヤボール
	public GameObject fireBallAttackpooledObject;
	public int fireBallAttackpooledAmount = 3;
	List<GameObject> fireBallAttackpooledObjects;

	//歩く雑魚斬撃
	public GameObject swordAttackpooledObject;
	public int swordAttackpooledAmount = 3;
	List<GameObject> swordAttackpooledObjects;

	//ボスのクリスタルウォール
	public GameObject crystalWallpooledObject;
	public int crystalWallpooledAmount = 5;
	List<GameObject> crystalWallpooledObjects;

	//ボスのクリスタルバレット
	public GameObject crystalBulletpooledObject;
	public int crystalBulletpooledAmount = 5;
	List<GameObject> crystalBulletpooledObjects;


	public bool willGrow = true;

	void Awake()
	{
		current = this;
	}

	void Start () 
	{
		//ファイヤボール初期化
		fireBallAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < fireBallAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(fireBallAttackpooledObject);
			obj.SetActive(false);
			fireBallAttackpooledObjects.Add(obj);
		}

		//斬撃初期化
		swordAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < swordAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(swordAttackpooledObject);
			obj.SetActive(false);
			swordAttackpooledObjects.Add(obj);
		}

		//クリスタルウォール初期化
		crystalWallpooledObjects = new List<GameObject> ();
		for (int i = 0; i < crystalWallpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(crystalWallpooledObject);
			obj.SetActive(false);
			crystalWallpooledObjects.Add(obj);
		}

		//クリスタルバレット初期化
		crystalBulletpooledObjects = new List<GameObject> ();
		for (int i = 0; i < crystalBulletpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(crystalBulletpooledObject);
			obj.SetActive(false);
			crystalBulletpooledObjects.Add(obj);
		}



	}

	public GameObject GetFireBallAttackPooledObject()
	{
		for (int i = 0; i < fireBallAttackpooledObjects.Count; i++)
		{
			if(!fireBallAttackpooledObjects[i].activeInHierarchy)
			{
				return fireBallAttackpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(fireBallAttackpooledObject);
			fireBallAttackpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	public GameObject GetSwordAttackPooledObject()
	{
		for (int i = 0; i < swordAttackpooledObjects.Count; i++)
		{
			if(!swordAttackpooledObjects[i].activeInHierarchy)
			{
				return swordAttackpooledObjects[i];
			}
		}
		
		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(swordAttackpooledObject);
			swordAttackpooledObjects.Add(obj);
			return obj;
		}
		
		return null;
	}

	public GameObject GetCrystalWallPooledObject()
	{
		for (int i = 0; i < crystalWallpooledObjects.Count; i++)
		{
			if(!crystalWallpooledObjects[i].activeInHierarchy)
			{
				return crystalWallpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(crystalWallpooledObject);
			crystalWallpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	public GameObject GetCrystalBulletPooledObject()
	{
		for (int i = 0; i < crystalBulletpooledObjects.Count; i++)
		{
			if(!crystalBulletpooledObjects[i].activeInHierarchy)
			{
				return crystalBulletpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(crystalBulletpooledObject);
			crystalBulletpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}


}
