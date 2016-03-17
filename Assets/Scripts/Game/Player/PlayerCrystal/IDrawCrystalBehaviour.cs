using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CrystalManeuver
{
	public abstract class IDrawCrystalBehaviour
	{
		protected IPCCrystal []crystal; 

		public IDrawCrystalBehaviour (IPCCrystal []crystal)
		{
			this.crystal = crystal;
		}

		//クリスタル・ゲージをプラス
		public virtual void AddProgress(float timeinsec)
		{
			//先頭から補充する
			for (int i = 0; i < this.crystal.Length; i++)
			{
				//もし数値は0以下
				if (timeinsec <= 0)
					break;

				//ゲージを溜める
				if (crystal [i].crystalState == CRYSTAL_STATE.WAITING)
				{
					crystal [i].Progress += timeinsec;

					//1個充填完了
					if (crystal [i].Progress >= 1.0f)
					{
						timeinsec = crystal [i].Progress - 1.0f;
						crystal [i].Progress = 1.0f;

						crystal [i].crystalState = CRYSTAL_STATE.FULLFILLED;

					} else 
					{
						timeinsec = 0.0f;
						break;
					}
				}
			}

		}

	}
}

