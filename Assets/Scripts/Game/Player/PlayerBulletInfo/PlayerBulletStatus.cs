using UnityEngine;
using System.Collections;
using AdapterPAB;
public class PlayerBulletStatus : IPlayerBulletStatus
{

	public PlayerBulletStatus (IAdapter_PlayerAttackBulletInfo bulletinfo, GameObject bulletimgprefab, GameObject attachTogobj)
		:base(bulletinfo, bulletimgprefab ,attachTogobj)
	{

	}

}


