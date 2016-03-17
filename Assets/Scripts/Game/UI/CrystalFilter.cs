using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalFilter : MonoBehaviour {

	public float maxAlpha;
	public float speed;

	private PlayerCrystalSystem playerCrystalSystem;
	private Image image;

	void Start () 
	{
		image = GetComponent<Image> ();

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerCrystalSystem = playerObj.GetComponent<PlayerCrystalSystem> ();
	}

	void FixedUpdate ()
	{
		if (playerCrystalSystem.IsBlueScreen()) 
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

}
