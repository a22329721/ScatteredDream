using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseUIScript : MonoBehaviour {

	private bool isUse;
	private float size;
	private float speed = 3.0f;

	private RectTransform rt;

	public Text resume;
	public Text cameraLR, normalLR, reverseLR;
	public Text cameraUD, normalUD, reverseUD;
	public Text backToTitle;

	private bool cameraLRSetting, cameraUDSetting;

	void Start () 
	{
		isUse = false;
		size = 0.0f;
		cameraLRSetting = true;
		//SetCamera (1, cameraLRSetting);

		cameraUDSetting = true;
		//SetCamera (2, cameraUDSetting);

		rt = GetComponent<RectTransform> ();
	}
	

	void Update ()
	{
		if (isUse && size < 1.0f) 
		{
			size += Time.unscaledDeltaTime * speed;
			if (size > 1.0f)
				size = 1.0f;
			
			rt.localScale = new Vector3 (size, size, size);
		}
		else if (!isUse && size > 0.0f) 
		{
			size -= Time.unscaledDeltaTime * speed;
			if (size < 0.0f)
				size = 0.0f;

			rt.localScale = new Vector3 (size, size, size);
		}
	}

	public void SetUse(bool input)
	{
		isUse = input;
	}

	public void SetOption(int option)
	{
		resume.color = Color.black;
		cameraLR.color = Color.black;
		cameraUD.color = Color.black;
		backToTitle.color = Color.black;

		switch (option) 
		{
		case 0:
			{
				resume.color = Color.white;
				break;
			}
		case 1:
			{
				cameraLR.color = Color.white;
				break;
			}
		case 2:
			{
				cameraUD.color = Color.white;
				break;
			}
		case 3:
			{
				backToTitle.color = Color.white;
				break;
			}
		default:
			break;
		}
	}

	public void SetCamera(int option, bool reverse)
	{
		if (option == 1) 
		{
			normalLR.color = Color.black;
			reverseLR.color = Color.black;
			this.cameraLRSetting = reverse;
		}
		else if (option == 2) 
		{
			normalUD.color = Color.black;
			reverseUD.color = Color.black;
			this.cameraUDSetting = reverse;
		}

		if (!cameraLRSetting) 
		{
			normalLR.color = Color.white;
		}
		else
		{
			reverseLR.color = Color.white;
		}

		if (!cameraUDSetting) 
		{
			normalUD.color = Color.white;
		}
		else
		{
			reverseUD.color = Color.white;
		}

	}

	public bool GetCameraReverse(int option)
	{
		if (option == 1) 
		{
			return this.cameraLRSetting;
		}
		else if (option == 2) 
		{
			return this.cameraUDSetting;
		}
		return true;
	}

}
