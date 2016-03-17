using UnityEngine;
using System.Collections;

public class PlayerHPScript : MonoBehaviour {

	public float maxScaleX;
	private RectTransform rt;
	private float moveSpeed = 3.0f;

	private PlayerStatus playerStatus;
	private float playerMaxHP;
	public float playerRealHP, playerShowHP;

	void Start () 
	{
		rt = GetComponent<RectTransform> ();
		maxScaleX = rt.localScale.x;

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerStatus = playerObj.GetComponent<PlayerStatus> ();

		playerRealHP = playerStatus.GetHP ();
		playerShowHP = 0.0f;
		playerMaxHP = playerStatus.GetMaxHP ();

		rt.localScale = new Vector3 (playerShowHP / playerMaxHP * maxScaleX, rt.localScale.y, rt.localScale.z);

	}

	void FixedUpdate () 
	{
		playerRealHP = playerStatus.GetHP ();
		playerMaxHP = playerStatus.GetMaxHP ();

		if (playerRealHP < 0)
		{
			playerRealHP = 0.0f;
		}

		if (Mathf.Abs (playerShowHP - playerRealHP) > 0.1f) 
		{
			playerShowHP = Mathf.Lerp (playerShowHP, playerRealHP, moveSpeed * Time.unscaledDeltaTime);
		} 
		else 
		{
			playerShowHP = playerRealHP;
		}

		rt.localScale = new Vector3 (playerShowHP / playerMaxHP * maxScaleX, rt.localScale.y, rt.localScale.z);
	}
}
