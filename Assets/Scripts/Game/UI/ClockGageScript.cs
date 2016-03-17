using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClockGageScript : MonoBehaviour {

	public float moveSpeed;
	private float realClock, showClock;

	private RectTransform rt;
	private PlayerChangeForm playerChangeForm;

	private Image image;

	void Start () 
	{
		rt = GetComponent<RectTransform> ();
		image = GetComponent<Image> ();

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerChangeForm = playerObj.GetComponent<PlayerChangeForm> ();

		realClock = playerChangeForm.GetTimeGagePercentage ();
		showClock = realClock;

	}

	void FixedUpdate () 
	{
		realClock = playerChangeForm.GetTimeGagePercentage();

		if (Mathf.Abs (showClock - realClock) >= 0.001f)
			showClock = Mathf.Lerp (showClock, realClock, moveSpeed * Time.unscaledTime);
		else
			showClock = realClock;

		rt.localScale = new Vector3( showClock , 1.0f, 1.0f);;

		if (rt.localScale.x >= 1.0f)
		{
			image.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		}
		else 
		{
			image.color = new Color (50.0f/255.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
