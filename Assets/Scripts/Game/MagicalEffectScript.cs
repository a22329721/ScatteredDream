using UnityEngine;
using System.Collections;

//プレイヤーのアタックエフェクト(星)
public class MagicalEffectScript : MonoBehaviour {

	public Texture[] myTexture = new Texture[3];
	private ParticleSystemRenderer psRenderer;

	void Awake()
	{
		psRenderer = GetComponent<ParticleSystemRenderer> ();
	}

	public void SetTexture(int num)
	{
		psRenderer.material.mainTexture = myTexture [num];
	}
}
