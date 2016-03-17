using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CrystalManeuver
{
	public class DrawCrystalBehaviour:IDrawCrystalBehaviour
	{
		
		public DrawCrystalBehaviour (IPCCrystal []crystal):base(crystal)
		{
		}
	}
}

