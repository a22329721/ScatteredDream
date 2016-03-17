using UnityEngine;
using System.Collections;

public class BossHPScript : MonoBehaviour {

	public float maxScaleX;
	private RectTransform rt;
	private float moveSpeed = 2.0f;

	private BossEmptyBody bossEmptyBody;
	private float bossMaxHP;
	public float bossRealHP, bossShowHP;

	void Awake()
	{
		bossMaxHP = 1.0f;
	}

	void OnEnable () 
	{
		rt = GetComponent<RectTransform> ();
		maxScaleX = rt.localScale.x;
		/*
		GameObject bossObj = GameObject.FindWithTag ("BossController");
		if (bossObj != null)
		{
			bossEmptyBody = bossObj.GetComponent<BossEmptyBody> ();

			bossRealHP = bossEmptyBody.hp;
			bossShowHP = bossEmptyBody.hp;
			bossMaxHP = bossEmptyBody.hpMax;
		}
		*/
		rt.localScale = new Vector3 (bossShowHP / bossMaxHP, rt.localScale.y, rt.localScale.z);
	}
	

	void Update () 
	{

		if (bossEmptyBody != null)
		{
			bossRealHP = bossEmptyBody.hp;
			bossMaxHP = bossEmptyBody.hpMax;
		}

		if (bossRealHP < 0)
		{
			bossRealHP = 0.0f;
		}

		if (Mathf.Abs (bossShowHP - bossRealHP) > 0.1f) 
		{
			bossShowHP = Mathf.Lerp (bossShowHP, bossRealHP, moveSpeed * Time.unscaledDeltaTime);
		} 
		else 
		{
			bossShowHP = bossRealHP;
		}

		rt.localScale = new Vector3 (bossShowHP / bossMaxHP, rt.localScale.y, rt.localScale.z);
	}

	public void SetOn()
	{
		GameObject bossObj = GameObject.FindWithTag ("BossController");
		if (bossObj != null) 
		{
			bossEmptyBody = bossObj.GetComponent<BossEmptyBody> ();
			bossRealHP = bossEmptyBody.hp;
			bossShowHP = bossEmptyBody.hp;
			bossMaxHP = bossEmptyBody.hpMax;
		}

		gameObject.SetActive (true);
	}

	public void SetOff()
	{
		gameObject.SetActive (false);
	}

}
