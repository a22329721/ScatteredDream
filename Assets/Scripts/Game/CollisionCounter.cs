using UnityEngine;
using System.Collections;

public class CollisionCounter : MonoBehaviour {

	public int counter;
	
	void Start () {
		counter = 0;
	}

	void OnEnable()
	{
		counter = 0;
	}
	
	void OnTriggerEnter(Collider other)
	{
		counter++;
	}

	void OnTriggerExit(Collider other)
	{
		counter--;
	}
}
