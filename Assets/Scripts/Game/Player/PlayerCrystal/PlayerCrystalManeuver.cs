using UnityEngine;
using System.Collections;

namespace CrystalManeuver
{
	
	public class PlayerCrystalManeuver : IPlayerCrystalManeuver
	{
		public PlayerCrystalManeuver (GameObject attachtoObj, GameObject prefabObj, int crystals, GameObject crystalPrefabGobj, 
			Transform playerTransform,Transform cameraTransform,Transform lookAtTransform)
			: base(attachtoObj, prefabObj, crystals, crystalPrefabGobj, playerTransform, cameraTransform, lookAtTransform)
		{
			
		}
	}
}

