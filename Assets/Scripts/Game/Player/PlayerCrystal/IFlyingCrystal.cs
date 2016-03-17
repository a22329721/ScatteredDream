using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace CrystalManeuver
{
	public abstract class IFlyingCrystal: MonoBehaviour
	{
		protected Transform start;
		protected Transform target;
		protected Vector3 moveVector;
		protected float moveSpeed = 15.0f;

		//使っている
		protected bool act = false;
		protected float timer = 0.0f;

		protected IActCrystalBehaviour actcrystal;
		protected Renderer render;
		protected float finishtime = 5.0f;
		protected bool isWall;

		protected LineRenderer lr;

		public bool Acted
		{
			get{ return this.act;}
		}

		public Vector3 position
		{
			get{ return this.gameObject.transform.position; }
		}
			
		public void Injection(IActCrystalBehaviour actcrystal)
		{
			this.actcrystal = actcrystal;
			this.render = this.gameObject.GetComponentInChildren<MeshRenderer> ();

		}
			
		//クリスタルを投げる
		public void Act(Transform start, Transform target)
		{
			this.start = start;
			this.target = target;

			moveVector = (target.position - start.position);
			moveVector.Normalize ();
			moveVector = moveVector * moveSpeed;

			act = true;
			timer = 0.0f;
			isWall = false;

			this.gameObject.transform.position = target.position + moveVector.normalized;
			this.gameObject.transform.rotation = Quaternion.Euler (0, 90, 0);
		
			render.material.color = Color.HSVToRGB (225.0f / 360.0f, 1, 1);
		}

		//UIの色を反映するため
		public float GetCrystalDistanceAlert()
		{
			return this.timer / this.finishtime;
		}

		void Update()
		{
			if (act) 
			{
				timer += Time.deltaTime;

				if(!isWall)
					this.gameObject.transform.position += moveVector * Time.deltaTime * (1 / Time.timeScale);


				if (lr == null) 
				{
					Color color = Color.red;
					this.gameObject.AddComponent<LineRenderer> ();
					lr = this.gameObject.GetComponent<LineRenderer> ();
					lr.material = new Material (Shader.Find ("Particles/Additive"));
					lr.SetColors (color,color);
					lr.SetWidth (0.1f, 0.1f);
				}

				lr.SetPosition (0, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z));
				lr.SetPosition (1, new Vector3(this.gameObject.transform.position.x, 0.0f, this.gameObject.transform.position.z));
				//Debug.DrawRay( this.gameObject.transform.position, -Vector3.up * 50.0f, Color.black, 20.0f, true );

				//色を変わる
				render.material.color = Color.HSVToRGB ((225.0f-225.0f*timer/finishtime )/ 360.0f, 1, 1);

				//タイムオーバー リセット
				if (timer > finishtime) 
				{
					act = false;
					timer = 0.0f;
					this.actcrystal.ConsumeCrystal();
					this.gameObject.SetActive (false);


				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			//プレイヤー以外のオブジェクトと接触したら消す
			if (other.tag != "Player" && other.tag != "Enemy" && other.tag != "BossController") 
			{

				isWall = true;

				/*
				act = false;
				timer = 0.0f;
				this.actcrystal.ConsumeCrystal();
				this.gameObject.SetActive (false);*/
			}
		}
	
	}
}

