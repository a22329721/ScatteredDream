using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform lookObject;
	private Transform target;
	

	void Start()
	{

		target = lookObject;
	}

	void Update()
	{

	}

	void LateUpdate()
	{
		//視点の更新
		transform.LookAt (target);
	}
}
