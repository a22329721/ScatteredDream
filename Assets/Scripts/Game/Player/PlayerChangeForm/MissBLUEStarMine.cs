using UnityEngine;
using System.Collections;

//ビーム
public class MissBLUEStarMine : MonoBehaviour {

	RaycastHit hit;

	LineRenderer line;
	public ParticleSystem startParticle;
	public ParticleSystem bodyParticle;
	private Vector3 toward;
	public int elementSize;
	public Transform startLocation;
	public Transform targetLocation;
	public Transform hitEffect;

	public float rayStartShootingTime;
	public float rayDuration;
	public float rayMaxTime;
	public float rayDepth;


	private bool startShooting = false;
	private float timer = 0;
	private float currentDepth = 1;
	public float widthStart;
	public float widthEnd;
	public float maxWidthStart;
	public float maxWidthEnd;
	private float currentWidthStart;
	private float currentWidthEnd;
	public float bodyParticleSlowdown;

	public AnimationCurve startParticleSizeCurve;

	public PlayerChangeForm playerChangeForm;
	private float beamAtk = 10f;
	private float attackTimer;
	private AudioSource audioSource;


	public void ShootingStar()
	{
		startShooting = true;
		line.enabled = false;
		currentWidthStart = widthStart;
		currentWidthEnd = maxWidthEnd;
		line.SetWidth(currentWidthStart, currentWidthEnd);
		startParticle.gameObject.SetActive (true);

		attackTimer = 5.0f;

		playerChangeForm.gameObject.transform.eulerAngles = new Vector3(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
	}

	public void ShutDown()
	{
		timer = 0;
		startShooting = false;
		startParticle.gameObject.SetActive (false);
		hitEffect.gameObject.SetActive (false);
		bodyParticle.gameObject.SetActive (false);
		line.enabled = false;
		currentDepth = 1;

		playerChangeForm.ReleaseBeamState ();
	}
	void Start()
	{
		line = GetComponent<LineRenderer>();
		line.SetVertexCount(elementSize);

		audioSource = GetComponent<AudioSource> ();
		line.enabled = false;
	}

	void Update()
	{
		
		if (startShooting) 
		{
			if (Time.timeScale != 0.0f)
			{
				timer += Time.unscaledDeltaTime;
				attackTimer += Time.unscaledDeltaTime;
			}

			//ビーム準備期間
			if (timer < rayStartShootingTime)
			{
				startParticle.startSize = startParticleSizeCurve.Evaluate(timer / rayStartShootingTime) ;
				return;
			} 
			else 
			{
				line.enabled = true;
				hitEffect.gameObject.SetActive (true);
				bodyParticle.gameObject.SetActive (true);
			}


			toward =  targetLocation.position - Camera.main.transform.position;
			toward.Normalize ();
			toward  = toward * currentDepth;

			//目標になれるのは敵とフィールドだけ
			RaycastHit hit;
			int layerMask = 1;
			layerMask |= (1 << 11);

			if (Physics.Raycast (startLocation.position, toward, out hit, currentDepth, layerMask)) 
			{
				//当たり
				if (hit.transform.tag == "Enemy" && attackTimer >= 0.2f && Time.timeScale != 0.0f) 
				{
					GameObject obj;
					BombEffectScript script;

					obj = EffectManager.current.GetBombEffecrtPooledObject ();
					script = obj.GetComponent<BombEffectScript> ();

					obj.transform.position = hit.transform.position;
					script.SetSpeed (1.5f);
					obj.SetActive (true);
					script.SetTexture (3);

					EnemyStatus enemyStatus;
					enemyStatus = hit.transform.GetComponent<EnemyStatus> ();

					audioSource.Play ();
					enemyStatus.Damage (beamAtk);

					attackTimer = 0.0f;
				}

				hitEffect.transform.position = hit.point;

				for (int i = 0; i < elementSize; i++) 
				{
					line.SetPosition (i, startLocation.position + toward / (elementSize - 1) * (i));
				}
			} 
			else 
			{
				//当ってないなら、ビームを伸ばす
				for (int i = 0; i < elementSize; i++) 
				{
					line.SetPosition (i, startLocation.position + toward / (elementSize - 1) * (i));
				}
				hitEffect.transform.position = startLocation.position + toward / (elementSize - 1) * (elementSize);
			}


			bodyParticle.transform.LookAt (Camera.main.transform.position , -Vector3.up);

			if (timer <= rayMaxTime + rayStartShootingTime && Time.timeScale != 0.0f) 
			{
				currentDepth += Time.unscaledDeltaTime / rayMaxTime * rayDepth;

				currentWidthStart += Time.unscaledDeltaTime / rayMaxTime * (maxWidthStart - widthStart);
				currentWidthEnd += Time.unscaledDeltaTime / rayMaxTime * (maxWidthEnd - widthEnd);
			}

			line.SetWidth(currentWidthStart, currentWidthEnd);

			//時間切れ
			if (timer >= rayDuration + rayStartShootingTime)
			{
				ShutDown ();
			}

		}
	}
}
