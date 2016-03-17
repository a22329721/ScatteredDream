using UnityEngine;
using System.Collections;

public class PlayerStarEffect : MonoBehaviour {

	public float stayTime;

	private ParticleSystem ps;
	private ParticleSystemRenderer psRenderer;


	void Start () 
	{
		ps = GetComponent<ParticleSystem> ();
		psRenderer = ps.GetComponent<ParticleSystemRenderer>();
		stayTime = 0.0001f;

		ps.Pause();
		//ps.Clear ();
	}

	void Update () 
	{
		if (stayTime > 0.0f) 
		{
			stayTime -= Time.unscaledDeltaTime;

			if (stayTime <= 0.0f) 
			{
				//ps.Pause();
				//ps.Clear ();
				gameObject.SetActive (false);
			}
		}
	}

	public void SetStarEffect(Color color, int amount)
	{
		//ps.Pause();
		//ps.Clear ();
		psRenderer.material.SetColor("_TintColor", color);
		stayTime = 1.0f;
		//ps.Play ();
		gameObject.SetActive (true);
		ps.Emit (amount);
	}

}
