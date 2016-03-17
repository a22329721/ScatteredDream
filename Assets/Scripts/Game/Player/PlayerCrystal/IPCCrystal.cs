using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CrystalManeuver
{
	public enum CRYSTAL_STATE{
		WAITING,	//充填待ち
		FULLFILLED,	//充填完了
		ACTING1,	//使っている
		ACTING2		//投げる(クリスタル．テレポート)
	};

	public abstract class IPCCrystal
	{
		//クリスタル状態のGET SET関数
		public CRYSTAL_STATE crystalState
		{
			get{ return this.crystalStates;}
			set
			{ 
				this.crystalStates = value;

				this.image.color = ColorPattern(this.crystalProgress);
			}
		}

		//クリスタル充填状態のGET SET関数
		public float Progress
		{
			get{ return this.crystalProgress;}
			set
			{
				this.crystalProgress = value;
				this.image.color = ColorPattern(this.crystalProgress);
			}
		}

		protected GameObject gobj;
		protected CRYSTAL_STATE crystalStates;
		protected float crystalProgress;
		protected Image image;
		protected RectTransform rectTrans;

		public IPCCrystal (GameObject prefabgobj)
		{
			this.gobj = MonoBehaviour.Instantiate((Object) prefabgobj, prefabgobj.transform.position, prefabgobj.transform.rotation) as GameObject;
			this.image = this.gobj.GetComponent<Image> ();
			this.rectTrans = this.gobj.GetComponent<RectTransform> ();

			this.crystalStates = CRYSTAL_STATE.WAITING;
			this.crystalProgress = 0.0f;
			this.image.color = new Color (0, 0, 0);
		}

		//クリスタルの色は状態によって変更する
		virtual protected Color ColorPattern(float colorvalue)
		{
			switch(this.crystalStates)
			{
			case CRYSTAL_STATE.WAITING:
				{
					return new Color (colorvalue * 0.8f, colorvalue * 0.8f, colorvalue * 0.8f);
					//break;
				}
			case CRYSTAL_STATE.FULLFILLED:
				{
					return new Color (1.0f, 1.0f, 1.0f);
					//break;
				}
			case CRYSTAL_STATE.ACTING1:
				{
					return new Color (0.5f, 0.5f, colorvalue);
					//break;
				}
			case CRYSTAL_STATE.ACTING2:
				{
					return new Color (0, colorvalue, 0);
					//break;
				}
			default:
				break;
			}
			return new Color (0.0f, 0.0f, 0.0f);
		}

		virtual public void SetColor(Color color)
		{
			this.image.color = color;
		}

		virtual public void SetPosition(float x, float y)
		{
			Vector2 v = new Vector2 (x, y);
			this.rectTrans.position = v;
		}

		virtual public Vector2 GetPosition()
		{
			return this.rectTrans.position;
		}

		virtual public void AttachTo(GameObject attachtarget)
		{
			//UIで描画する位置に貼る
			this.gobj.transform.SetParent (attachtarget.gameObject.transform, false);
		}
			
		virtual public void SetName(string name)
		{
			this.gobj.name = name;
		}
	}

}

