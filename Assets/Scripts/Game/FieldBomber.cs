using UnityEngine;
using System.Collections;

public class FieldBomber : MonoBehaviour {

	public bool isDestroy;
	public GameObject Walls;
	public GameObject FieldEdge;
	public GameObject Floor;

	public int numberOfBig;
	public GameObject bigParticle;

	public int numberOfSmall;
	public GameObject smallParticle;

	private AudioSource audioSource;

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
		isDestroy = false;
	
	}

	public void DestroyField()
	{
		if (!isDestroy) 
		{
			Walls.SetActive (false);
			FieldEdge.SetActive (false);
			Floor.SetActive (false);

			for (int i = 0; i < numberOfBig; i++) 
			{
				bigParticle.transform.GetChild (i).gameObject.SetActive (true);
			}

			for (int i = 0; i < numberOfSmall; i++) 
			{
				smallParticle.transform.GetChild (i).gameObject.SetActive (true);
			}

			audioSource.Play ();
			isDestroy = true;
		}
	}

}
