using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace CrystalManeuver
{
	public abstract class IPlayerCrystalManeuver
	{
		protected IPCCrystal [] crystal;
		protected GameObject crystalPrefab;
		protected GameObject attachToGobj;

		protected bool isExSkill;
		protected IDrawCrystalBehaviour drawCrystal;
		protected IActCrystalBehaviour actCrystal;

		protected GameObject crystalPrefabGobj;
		protected Transform cameraTransform;
		protected Transform lookAtTransform;
		protected Transform playerTransform;
		//protected FlyingCrystal flyingCrystal;

		public IPlayerCrystalManeuver(GameObject attachtoObj, GameObject prefabObj, int crystals,GameObject crystalPrefabGobj,Transform playerTransform,Transform cameraTransform,Transform lookAtTransform)
		{
			this.attachToGobj = attachtoObj;
			this.crystalPrefab = prefabObj;
		
			this.cameraTransform = cameraTransform;
			this.lookAtTransform = lookAtTransform;
			this.playerTransform = playerTransform;
			this.crystalPrefabGobj = MonoBehaviour.Instantiate ((Object)crystalPrefabGobj) as GameObject;

			Initialization (crystals);
			//this.flyingCrystal = this.CrystalPrefabGobj.gameObject.AddComponent <FlyingCrystal>() as FlyingCrystal;
		}

		//初期化
		//指定された数量のクリスタルを作る
		virtual protected void Initialization (int crystals)
		{
			//0なら終了
			if (crystals <= 0)
				return;
			
			crystal = new PCCrystal[crystals];

			RectTransform rect = crystalPrefab.GetComponent<RectTransform> ();
			float x_offset = rect.rect.width * rect.localScale.x;

			for(int i = 0; i < crystals; i++)
			{
				crystal [i] = new PCCrystal (this.crystalPrefab);
				crystal [i].SetPosition (crystal[i].GetPosition().x + x_offset * i, crystal[i].GetPosition().y );
				crystal [i].AttachTo (this.attachToGobj);
				crystal [i].SetName ("Crystal" + i);
			}

			this.drawCrystal = new DrawCrystalBehaviour (crystal);
			this.actCrystal = new ActCrystalBehaviour (crystal, crystalPrefabGobj, playerTransform, cameraTransform, lookAtTransform);
		}

		virtual public void DrawCrystal(float progress)
		{
			//ゲージを増える
			this.drawCrystal.AddProgress(progress);
		
		}

		virtual public void UpdateCrystalColorInFly()
		{
			this.actCrystal.UpdateCrystalColor ();
		}

		virtual public int ActCrystal()
		{
			return this.actCrystal.TryActCrystal ();
		}

		virtual public void ConsumeCrystal()
		{
			this.actCrystal.ConsumeCrystal ();
		}

		public bool IsNextExSkill
		{
			get{return this.actCrystal.IsNextExSkill;}
		}

		public bool IsBlueScreen
		{
			get{return this.actCrystal.IsBlueScreen;}
		}
	}
}
