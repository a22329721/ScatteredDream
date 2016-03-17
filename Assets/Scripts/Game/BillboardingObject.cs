using UnityEngine;
using System.Collections;

public class BillboardingObject : MonoBehaviour {


	void Update () {
		transform.LookAt (Camera.main.transform);
		transform.Rotate (90.0f, 0.0f, 0.0f);
	}
}
