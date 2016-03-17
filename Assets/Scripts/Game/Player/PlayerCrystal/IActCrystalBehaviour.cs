using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CrystalManeuver
{
	public enum CRYSTAL_ACT_STATE
	{ 
		NONE, ACT_NEW, ACT_CRYSTAL
	};

	public class IActCrystalBehaviour
	{
		protected IPCCrystal []crystal; 
		protected CRYSTAL_ACT_STATE actState = CRYSTAL_ACT_STATE.NONE;

		protected bool isBlueScreen = false;
		protected bool isNextExSkill = false;

		protected GameObject crystalPrefabGobj;
		protected Transform cameraTransform;
		protected Transform lookAtTransform;
		protected Transform playerTransform;

		protected IFlyingCrystal flyingcrystal;

		private PlayerStarEffect playerStarEffect;

		public IActCrystalBehaviour (IPCCrystal []crystal,GameObject crystalPrefabGobj,Transform playerTransform,Transform cameraTransform,Transform lookAtTransform)
		{
			this.crystal = crystal;
			this.crystalPrefabGobj = crystalPrefabGobj;
			this.cameraTransform = cameraTransform;
			this.lookAtTransform = lookAtTransform;
			this.playerTransform = playerTransform;
			this.flyingcrystal = this.crystalPrefabGobj.AddComponent <FlyingCrystal>() as IFlyingCrystal;
			this.flyingcrystal.Injection (this);

			//Debug.Log (this.flyingcrystal);
			this.crystalPrefabGobj.SetActive (false);

			GameObject playerObject = GameObject.FindWithTag ("Player");
			this.playerStarEffect = playerObject.GetComponentInChildren<PlayerStarEffect> ();
		}

		/*virtual public void CrystalVanish()
		{
			
		}*/

		//発射状態によって、クリスタルUIの色を変える
		public void UpdateCrystalColor()
		{
			if (flyingcrystal.Acted) 
			{
				float colorAlert = flyingcrystal.GetCrystalDistanceAlert ();

				for (int i = 0; i < this.crystal.Length; i++)
				{
					if( this.crystal [i].crystalState == CRYSTAL_STATE.ACTING2)
					{
						if (colorAlert <= 0.33f) 
						{
							this.crystal [i].SetColor (Color.magenta);
						}
						else if(colorAlert <= 0.66f) 
						{
							this.crystal [i].SetColor (Color.yellow);
						}
						else if(colorAlert <= 1.0f) 
						{
							this.crystal [i].SetColor (Color.red);
						}

						return ;
					}
				}
			}
		}


		//クリスタルボタンを押す
		virtual public int TryActCrystal()
		{

			//クリスタルアクション(2－3回目の場合)
			for (int i = 0; i < this.crystal.Length; i++)
			{
				//クリスタルを投げる
				if (this.crystal [i].crystalState == CRYSTAL_STATE.ACTING1 ) 
				{
					this.crystal [i].crystalState = CRYSTAL_STATE.ACTING2;
					actState = CRYSTAL_ACT_STATE.ACT_CRYSTAL;

					//次のスキルは通常スキルに戻る
					isNextExSkill = false;
					isBlueScreen = true;

					//クリスタルを投げる
					this.crystalPrefabGobj.SetActive (true);
					this.flyingcrystal.Act (cameraTransform, lookAtTransform);
					//Vector3 v = CameraTransform.position - LookAtTransform.position;
					//v.Normalize ();
					//MonoBehaviour.print (v);
					return 2;
				}
				//クリスタルテレポート
				else if( this.crystal [i].crystalState == CRYSTAL_STATE.ACTING2)
				{
					//プレイヤーの位置を移動する
					this.playerTransform.position = this.flyingcrystal.position;
					this.playerStarEffect.SetStarEffect (new Color (135.0f / 255.0f, 234.0f / 255.0f, 55.0f / 255.0f), 50);
					this.crystalPrefabGobj.SetActive (false);
					ConsumeCrystal ();
					return 3;
				}
			}

			//クリスタルモードに入る(1回の場合)
			for (int i = 0; i < this.crystal.Length; i++) 
			{
				if (this.crystal [i].crystalState == CRYSTAL_STATE.FULLFILLED)
				{
					this.crystal [i].crystalState = CRYSTAL_STATE.ACTING1;
					actState = CRYSTAL_ACT_STATE.ACT_NEW;
					isNextExSkill = true;
					isBlueScreen = true;
					//MonoBehaviour.print ("ACT1");
					return 1;
				}
			}

			return 0;
		}

		//クリスタルリセット
		virtual public void ConsumeCrystal()
		{
			//使っているのクリスタルをリセット
			for (int i = 0; i < this.crystal.Length; i++)
			{
				if (this.crystal [i].crystalState == CRYSTAL_STATE.ACTING1 
					|| this.crystal [i].crystalState == CRYSTAL_STATE.ACTING2 ) 
				{
					this.crystal [i].crystalState = CRYSTAL_STATE.WAITING;
					break;
				}
			}

			//クリスタルを整列する
			for (int i = 0; i < this.crystal.Length-1; i++)
			{
				this.crystal [i].crystalState = this.crystal [i + 1].crystalState;
				this.crystal [i].Progress = this.crystal [i + 1].Progress;
			}
			this.crystal [this.crystal.Length-1].crystalState = CRYSTAL_STATE.WAITING;
			this.crystal [this.crystal.Length - 1].Progress = 0;

			//アクションをリセットする
			actState = CRYSTAL_ACT_STATE.NONE;
			isNextExSkill = false;
			isBlueScreen = false;

		}

		//次のスキルはEX技になる
		public bool IsNextExSkill
		{
			get{return this.isNextExSkill;}
		}

		//青いフィルターを表示する
		public bool IsBlueScreen
		{
			get{return this.isBlueScreen;}
		}


	}

}

