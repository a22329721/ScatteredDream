using UnityEngine;
using System.Collections;

public class BossAreaDetectorScript : MonoBehaviour {

	private bool isInArea;

	void Start()
	{
		isInArea = false;
	}

	public bool IsInArea()
	{
		return isInArea;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			isInArea = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
			isInArea = true;
	}


}
