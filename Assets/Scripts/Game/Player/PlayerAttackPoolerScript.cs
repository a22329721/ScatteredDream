using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackPoolerScript : MonoBehaviour {

	public static PlayerAttackPoolerScript current;

	//ライト斬撃
	public GameObject lightSwordAttackpooledObject;
	public int lightSwordAttackpooledAmount = 2;
	List<GameObject> lightSwordAttackpooledObjects;

	//ヘヴィ斬撃
	public GameObject heavySwordAttackpooledObject;
	public int heavySwordAttackpooledAmount = 10;
	List<GameObject> heavySwordAttackpooledObjects;

	//バレット
	public GameObject bulletAttackpooledObject;
	public int bulletAttackpooledAmount = 20;
	List<GameObject> bulletAttackpooledObjects;

	//バレット
	public GameObject blowAttackpooledObject;
	public int blowAttackpooledAmount = 1;
	List<GameObject> blowAttackpooledObjects;

	public bool willGrow = true;

	void Awake()
	{
		current = this;
	}

	void Start () 
	{
		//ライト斬撃初期化
		lightSwordAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < lightSwordAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(lightSwordAttackpooledObject);
			obj.SetActive(false);
			lightSwordAttackpooledObjects.Add(obj);
		}

		//ヘヴィ斬撃初期化
		heavySwordAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < heavySwordAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(heavySwordAttackpooledObject);
			obj.SetActive(false);
			heavySwordAttackpooledObjects.Add(obj);
		}

		//バレット初期化
		bulletAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < bulletAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(bulletAttackpooledObject);
			obj.SetActive(false);
			bulletAttackpooledObjects.Add(obj);
		}

		//ブロー初期化
		blowAttackpooledObjects = new List<GameObject> ();
		for (int i = 0; i < blowAttackpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(blowAttackpooledObject);
			obj.SetActive(false);
			blowAttackpooledObjects.Add(obj);
		}

	}

	public GameObject GetLightSwordAttackPooledObject()
	{
		for (int i = 0; i < lightSwordAttackpooledObjects.Count; i++)
		{
			if(!lightSwordAttackpooledObjects[i].activeInHierarchy)
			{
				return lightSwordAttackpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(lightSwordAttackpooledObject);
			lightSwordAttackpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	public GameObject GetHeavySwordAttackPooledObject()
	{
		for (int i = 0; i < heavySwordAttackpooledObjects.Count; i++)
		{
			if(!heavySwordAttackpooledObjects[i].activeInHierarchy)
			{
				return heavySwordAttackpooledObjects[i];
			}
		}
		
		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(heavySwordAttackpooledObject);
			heavySwordAttackpooledObjects.Add(obj);
			return obj;
		}
		
		return null;
	}

	public GameObject GetbulletAttackPooledObject()
	{
		for (int i = 0; i < bulletAttackpooledObjects.Count; i++)
		{
			if(!bulletAttackpooledObjects[i].activeInHierarchy)
			{
				return bulletAttackpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(bulletAttackpooledObject);
			bulletAttackpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

	public GameObject GetblowAttackPooledObject()
	{
		for (int i = 0; i < blowAttackpooledObjects.Count; i++)
		{
			if(!blowAttackpooledObjects[i].activeInHierarchy)
			{
				return blowAttackpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(blowAttackpooledObject);
			blowAttackpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}



}
