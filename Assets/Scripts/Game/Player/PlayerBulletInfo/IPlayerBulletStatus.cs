using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AdapterPAB;
public abstract class IPlayerBulletStatus
{
	protected IAdapter_PlayerAttackBulletInfo bulletInfo;
	protected int bulletCount;
	protected GameObject bulletImgPrefab;
	protected int maxBullet;
	protected GameObject[] bulletImgsObj;
	protected RectTransform[] bulletTrans;
	protected Image[] bulletImgs;
	protected GameObject attachTogobj;
	protected float offset_x = 15.0f;
	protected float radius = 45.0f;

	public IPlayerBulletStatus (IAdapter_PlayerAttackBulletInfo bulletinfo, GameObject bulletimgprefab, GameObject attachTogobj)
	{
		this.bulletInfo = bulletinfo;
		this.bulletImgPrefab = bulletimgprefab;
		this.maxBullet = bulletinfo.GetMaxBulletCount ();
		this.attachTogobj = attachTogobj;
		Initialization ();

	}

	protected virtual void Initialization()
	{
		bulletImgsObj = new GameObject [maxBullet];
		bulletTrans = new RectTransform [maxBullet];
		bulletImgs = new Image[maxBullet];

		//初期化
		for(int i = 0; i < maxBullet; i++)
		{
			bulletImgsObj [i] = MonoBehaviour.Instantiate((Object) bulletImgPrefab, bulletImgPrefab.transform.position, bulletImgPrefab.transform.rotation) as GameObject;
			bulletTrans [i] = bulletImgsObj [i].GetComponent<RectTransform> ();

			bulletTrans [i].position = new Vector2 (bulletTrans [i].position.x + radius * Mathf.Cos ((float)i * 36.0f / 180.0f * Mathf.PI)
				,bulletTrans[i].position.y + radius * Mathf.Sin ((float)i * 36.0f / 180.0f * Mathf.PI));

			bulletImgs[i]=  bulletImgsObj [i].GetComponent<Image> ();

			bulletImgsObj[i].transform.SetParent (attachTogobj.transform, false);
			bulletImgsObj [i].name = "BulletImg" + i;

			bulletImgs [i].color = new Color (bulletImgs [i].color.r, bulletImgs [i].color.g, bulletImgs [i].color.b, 0.0f);
		}


	}
	public virtual void UpdateBulletImg()
	{
		bulletCount = this.bulletInfo.GetBulletCount ();

		//残弾表示
		for (int i = 0; i < bulletCount; i++) 
		{
			bulletImgs [i].color = new Color (1.0f, 1.0f, 1.0f, bulletImgs [i].color.a);
		}

		//使った弾
		for (int i = bulletCount; i < maxBullet; i++) 
		{
			bulletImgs [i].color = new Color (0.0f, 0.0f, 0.0f, bulletImgs [i].color.a);
		}
	}

	public virtual float GetCurrentBulletAlpha()
	{
		return bulletImgs [0].color.a;
	}

	public virtual void SetBulletAlpha(float a)
	{
		for(int i = 0; i < maxBullet; i++) 
		{
			bulletImgs [i].color = new Color(bulletImgs[i].color.r,	bulletImgs[i].color.g, bulletImgs[i].color.b,a);
		}
	}
}


