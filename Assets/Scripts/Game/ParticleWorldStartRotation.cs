using UnityEngine;
using System.Collections;

public class ParticleWorldStartRotation : MonoBehaviour {

	public ParticleSystem particleSystem;


	void FixedUpdate () 
	{
		//度数
		Vector3 startrotate = new Vector3 (this.transform.rotation.eulerAngles.x,
			                     this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
		
		startrotate = startrotate / 180.0f * Mathf.PI;

		//ラジアン
		particleSystem.startRotation3D = startrotate;
			
	}
}
