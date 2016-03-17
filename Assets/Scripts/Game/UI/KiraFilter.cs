using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KiraFilter : MonoBehaviour {

	public bool onOff;
	public float maxAlpha;
	public float speed;

	private Image image;

	void Start () 
	{
		image = GetComponent<Image> ();

		onOff = false;
	}

	void FixedUpdate ()
	{
		if (onOff) 
		{
			if (image.color.a < maxAlpha / 255.0f) 
			{
				image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a + speed * Time.unscaledDeltaTime);
			}
		}
		else
		{
			if (image.color.a > 0.0f) 
			{
				image.color = new Color (image.color.r, image.color.g, image.color.b, image.color.a - speed * Time.unscaledDeltaTime);
			}
		}
	}

	public void SetKiraFilter(bool state)
	{
		onOff = state;
	}

}
