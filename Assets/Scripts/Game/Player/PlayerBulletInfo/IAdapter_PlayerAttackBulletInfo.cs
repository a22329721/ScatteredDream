using UnityEngine;
using System.Collections;

namespace AdapterPAB
{
	public abstract class IAdapter_PlayerAttackBulletInfo
	{
		protected PlayerActionManager playerActionManager;
		protected int Maxbullet;

		public IAdapter_PlayerAttackBulletInfo (PlayerActionManager playerActionManager)
		{
			this.playerActionManager = playerActionManager;
			this.Maxbullet = playerActionManager.bulletMax;
		}

		virtual public int GetBulletCount()
		{
			return this.playerActionManager.GetBulletCount ();
		}

		virtual public int GetMaxBulletCount()
		{
			return this.playerActionManager.bulletMax;
		}
	}
}

