using UnityEngine;
using System.Collections;

public class GTPageManager : MonoBehaviour {

	public GTSelectionController tutorialController;
	public int pageNow;
	private float speed = 1.5f;

	void Start ()
	{
	
	}
	

	void Update () 
	{
		pageNow = tutorialController.GetPageNow () - 1;

		Color tmpColor;

		for (int i = 0 ; i < tutorialController.pageMax; i++) 
		{
			if (i == pageNow) 
			{
				//tmpColor = transform.GetChild (i).GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");
				tmpColor = transform.GetChild (i).GetComponent<MeshRenderer> ().material.color;

				if (tmpColor.a < 1.0f) 
				{
					tmpColor.a += speed * Time.deltaTime;
					if (tmpColor.a > 1.0f)
						tmpColor.a = 1.0f;

					//transform.GetChild (i).GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", tmpColor);
					transform.GetChild (i).GetComponent<MeshRenderer> ().material.color = tmpColor;
				}
			}
			else
			{
				//tmpColor = transform.GetChild (i).GetComponent<MeshRenderer> ().material.GetColor ("_TintColor");
				tmpColor = transform.GetChild (i).GetComponent<MeshRenderer> ().material.color;

				if (tmpColor.a > 0.0f) 
				{
					tmpColor.a -= speed * Time.deltaTime;
					if (tmpColor.a < 0.0f)
						tmpColor.a = 0.0f;

					//transform.GetChild (i).GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", tmpColor);
					transform.GetChild (i).GetComponent<MeshRenderer> ().material.color = tmpColor;
				}
			}
		}

	}
}
