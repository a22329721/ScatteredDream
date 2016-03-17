using UnityEngine;
using System.Collections;

public class SwordFloating : MonoBehaviour {
	
	public AnimationCurve curve;

	private Vector3 p2;
	public float a;
	float total;

	void Awake () 
	{
		p2 = transform.position - transform.forward;
	}
	

	void Update()
	{
		transform.position = Vector3.Lerp (transform.position, p2, Time.deltaTime);

	}
}
