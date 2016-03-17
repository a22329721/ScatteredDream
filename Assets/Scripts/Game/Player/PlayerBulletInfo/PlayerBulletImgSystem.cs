using UnityEngine;
using System.Collections;
using AdapterPAB;
public class PlayerBulletImgSystem:MonoBehaviour
{
	enum BulletImgStatus
	{
		HIDE,
		SHOW,
		FADEIN, 
		FADEOUT
	};

	private IPlayerBulletStatus playerBulletStatus;
	public GameObject bulletImgPrefab;
	public GameObject bulletImgAttachTo;
	public PauseManager pauseManager;

	public float fadeInTime;
	public float fadeOutTime;
	public float startFadeOutTimeNeeded;

	private float idleTimer = 0;
	private float fadein_initial_alpha = 0;

	private float fadeInTimer = 0;
	private float fadeOutTimer = 0;
	private BulletImgStatus bulletImgStatus = BulletImgStatus.HIDE ;

	public void Initialization(PlayerActionManager playerActionManager)
	{
		playerBulletStatus  = new PlayerBulletStatus (new AdapterPAB.Adapter_PlayerAttackBulletInfo (playerActionManager)
			,bulletImgPrefab, bulletImgAttachTo);
	}

	//残弾のUI表示
	void Update()
	{
		if (pauseManager.isGamePaused())
		{
		} 
		else
		{
			if (bulletImgStatus == BulletImgStatus.HIDE) 
			{
				if(Input.GetButtonDown ("GunShot"))
				{
					bulletImgStatus = BulletImgStatus.FADEIN;
					fadein_initial_alpha = playerBulletStatus.GetCurrentBulletAlpha ();
				}
			}
			else if(bulletImgStatus == BulletImgStatus.FADEIN)
			{
				if (fadeInTimer >= fadeInTime)
				{
					playerBulletStatus.SetBulletAlpha (1.0f);
					bulletImgStatus = BulletImgStatus.SHOW;
					fadeInTimer = 0;
				} 
				else
				{
					fadeInTimer +=Time.deltaTime;
					playerBulletStatus.SetBulletAlpha (fadein_initial_alpha + (1.0f - fadein_initial_alpha ) * fadeInTimer / fadeInTime);
				}

			}
			else if(bulletImgStatus == BulletImgStatus.FADEOUT)
			{
				if (Input.GetButtonDown ("GunShot")) 
				{
					bulletImgStatus = BulletImgStatus.FADEIN;
					fadein_initial_alpha = playerBulletStatus.GetCurrentBulletAlpha ();
					fadeOutTimer = 0;
				}
				else if (fadeOutTimer >= fadeOutTime)
				{
					playerBulletStatus.SetBulletAlpha (0);
					bulletImgStatus = BulletImgStatus.HIDE;
					fadeOutTimer = 0;
				} 
				else
				{
					fadeOutTimer +=Time.deltaTime;
					playerBulletStatus.SetBulletAlpha (1.0f - fadeOutTimer / fadeOutTime);
				}

			}
			else if(bulletImgStatus == BulletImgStatus.SHOW)
			{
				if (Input.GetButtonDown ("GunShot"))
				{
					idleTimer = 0;
				} 
				else 
				{
					idleTimer += Time.deltaTime;
					if (idleTimer >= startFadeOutTimeNeeded)
					{
						idleTimer = 0;
						bulletImgStatus = BulletImgStatus.FADEOUT;
					}
				}

			}
			playerBulletStatus.UpdateBulletImg ();
		}


	}
}

