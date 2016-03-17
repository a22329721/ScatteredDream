using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CrystalManeuver
{
	public class ActCrystalBehaviour:IActCrystalBehaviour
	{
		public ActCrystalBehaviour (IPCCrystal []crystal,GameObject crystalPrefabGobj,Transform playerTransform,Transform cameraTransform,Transform lookAtTransform)
			:base(crystal,crystalPrefabGobj,playerTransform,cameraTransform,lookAtTransform)
		{
			
		}
	}
}

