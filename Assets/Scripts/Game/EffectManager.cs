using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour {

	public static EffectManager current;

	//爆発エフェクト
	public GameObject bombEffecrtpooledObject;
	public int bombEffecrtpooledAmount = 3;
	List<GameObject> bombEffecrtpooledObjects;


	public bool willGrow = true;

	void Awake()
	{
		current = this;
	}

	void Start () 
	{
		//爆発エフェクト初期化
		bombEffecrtpooledObjects = new List<GameObject> ();
		for (int i = 0; i < bombEffecrtpooledAmount; i++) 
		{
			GameObject obj = (GameObject) Instantiate(bombEffecrtpooledObject);
			obj.SetActive(false);
			bombEffecrtpooledObjects.Add(obj);
		}
			
	}

	public GameObject GetBombEffecrtPooledObject()
	{
		for (int i = 0; i < bombEffecrtpooledObjects.Count; i++)
		{
			if(!bombEffecrtpooledObjects[i].activeInHierarchy)
			{
				return bombEffecrtpooledObjects[i];
			}
		}

		if (willGrow)
		{
			GameObject obj = (GameObject)Instantiate(bombEffecrtpooledObject);
			bombEffecrtpooledObjects.Add(obj);
			return obj;
		}

		return null;
	}

}
