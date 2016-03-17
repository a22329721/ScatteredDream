using UnityEngine;
using System.Collections;

public class PlayerComponent : MonoBehaviour {

	public Transform playerShoulder;

	public Vector3 GetPlayerShoulderPosition()
	{
		return new Vector3 (playerShoulder.position.x, playerShoulder.position.y, playerShoulder.position.z);
	}

	public Transform GetPlayerShoulderTransform()
	{
		return playerShoulder.transform;
	}

}
