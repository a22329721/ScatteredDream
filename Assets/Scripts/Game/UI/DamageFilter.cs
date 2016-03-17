using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageFilter : MonoBehaviour {

	public float maxAlpha;
	public float speed;

	private Image image;

	void Start()
	{
		image = GetComponent<Image> ();
	}

	void FixedUpdate () 
	{
		if (image.color.a > 0.0f) 
		{
			image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a - speed * Time.unscaledDeltaTime);
		}
	}

	public void SetDamageFilter()
	{
		float inputData = maxAlpha / 255.0f;

		image.color = new Color (image.color.r, image.color.g, image.color.b, inputData);
	}
}
