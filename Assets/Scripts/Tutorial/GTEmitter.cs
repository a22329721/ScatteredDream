using UnityEngine;
using System.Collections;

public class GTEmitter : MonoBehaviour {

	public AnimationCurve curve;
	private Material mat;

	void Start () 
	{
		this.mat = this.gameObject.GetComponent<Renderer> ().material;
	}
	

	void Update ()
	{
		this.mat.SetFloat ("_Emission", curve.Evaluate (Time.time));
	}
}
