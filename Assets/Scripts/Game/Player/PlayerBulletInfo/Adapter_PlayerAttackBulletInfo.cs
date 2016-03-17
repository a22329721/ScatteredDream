using UnityEngine;
using System.Collections;

namespace AdapterPAB
{
	public class Adapter_PlayerAttackBulletInfo : IAdapter_PlayerAttackBulletInfo
	{
		public Adapter_PlayerAttackBulletInfo (PlayerActionManager playeractionmanager)
			:base (playeractionmanager)
		{
		}

	}
}

