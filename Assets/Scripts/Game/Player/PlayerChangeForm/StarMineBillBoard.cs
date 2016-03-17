using UnityEngine;
using System.Collections;

public class StarMineBillBoard : MonoBehaviour {


	private float randomAngle;
	private float currentRandomAngle;
	private float difference;

	void Awake()
	{
		randomAngle = Random.Range (180, 270);
		currentRandomAngle= Random.Range (0, (int)randomAngle);
		difference = currentRandomAngle - randomAngle;
	}
	

	void Update () 
	{
		this.transform.LookAt (Camera.main.transform.position, -Vector3.up);

		this.transform.Rotate (new Vector3 (90, 0, 0));
		this.transform.Rotate (new Vector3 (0, currentRandomAngle, 0));
		currentRandomAngle = currentRandomAngle + Time.deltaTime * difference;

	}
}
