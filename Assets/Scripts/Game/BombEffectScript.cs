using UnityEngine;
using System.Collections;

//プレイヤーのアタックエフェクト(十字)
public class BombEffectScript : MonoBehaviour {

	public Texture[] myTexture = new Texture[4];

	private bool up;
	private float speed = 3.0f;
	private float scaleSpeed = 1.5f;
	private float AlphaNow;

	private MeshRenderer mr;

	void Awake ()
	{
		mr = GetComponent<MeshRenderer> ();
	}
	
	void OnEnable()
	{
		AlphaNow = 0.0f;
		mr.material.SetColor ("_TintColor", new Color (1.0f, 1.0f, 1.0f, AlphaNow));
		up = true;
		transform.localScale = new Vector3 (0.25f, 1.0f, 0.25f);
	}


	void FixedUpdate () 
	{
		//透明度調整
		if (AlphaNow < 0.8f && up)
		{
			AlphaNow += speed * Time.fixedDeltaTime;

			if (AlphaNow > 0.8f) 
			{
				AlphaNow = 0.8f;
				up = false;
			}
		}
		else if(AlphaNow >= 0.0f && !up)
		{
			AlphaNow -= speed * Time.fixedDeltaTime;

			if (AlphaNow < 0.0f)
				gameObject.SetActive (false);
		}

		mr.material.SetColor ("_TintColor", new Color (1.0f, 1.0f, 1.0f, AlphaNow));

		//拡大
		if (!up && AlphaNow >= 0.0f) 
		{
			transform.localScale = new Vector3 (transform.localScale.x + scaleSpeed * Time.fixedDeltaTime
				, 1.0f, transform.localScale.z + scaleSpeed * Time.fixedDeltaTime);
		}

	}

	public void SetSpeed(float inputSpeed)
	{
		this.speed = inputSpeed;
	}

	public void SetTexture(int num)
	{
		mr.material.mainTexture = myTexture [num];
	}

}
