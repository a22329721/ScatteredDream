using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackManager : MonoBehaviour {
	
	//使っている技
	public int skillNow = 0;
	public bool useNext = true;
	
	//コマンド受付時間
	private float lastAttackTime = 0.0f;
	private float nextAttackTime = 0.0f;
	private float nextSlideTime = 0.0f;

	//次の技までの冷却時間
	public float attackCooldown = 1.0f;
	public float skillCooldown = 0.5f;
	private float zeroCooldown;

	//フリーズ時間
	public float zeroVelocityTime = 0.0f;


	//バレット管理
	public int bulletNow;
	public int bulletMax;
	public bool[] bulletReady;
	public float autoReloadTime;
	public float lastShootTime = 0.0f;
	public float shootStateTime;
	//ターゲットに近いなら、直線で射撃
	private float limitDistance = 3.0f;


	public GameObject thirdPersonControllerObject;
	private ThirdPersonController thirdPersonController;
	private PlayerComponent playerComponent;
	private PlayerActionManager playerActionManager;
	private Rigidbody playerRb;

	//クリスタルシステム
	private PlayerCrystalSystem playerCrystalSystem;

	//敵位置情報の取得
	public GameObject enemyManager;
	private EnemyPoolerScript enemyPoolerScript;

	public PlayerActionVoiceManager playerActionVoiceManager;

	//アニメーターコントローラー
	//private Animator at;
	private List<Animator> ats;
	private PlayerChangeForm playerChangeForm;
	
	
	void Start () {
		thirdPersonController = thirdPersonControllerObject.GetComponent<ThirdPersonController> ();

		playerComponent = GetComponent<PlayerComponent> ();	
		playerActionManager = GetComponent<PlayerActionManager> ();
		playerRb = GetComponent<Rigidbody> ();	

		playerCrystalSystem = GetComponent<PlayerCrystalSystem> ();

		enemyPoolerScript = enemyManager.GetComponent<EnemyPoolerScript> ();

		zeroCooldown = 3 * skillCooldown;

		//バレット初期化
		bulletNow = bulletMax;

		bulletReady = new bool[bulletMax];
		for (int i = 0; i < bulletMax; i++)
		{
			bulletReady [i] = true;
		}

		playerChangeForm = GetComponent<PlayerChangeForm> ();
		//at = GetComponentInChildren<Animator> (); 
		Component[] animatorComponents = GetComponentsInChildren(typeof(Animator));

		ats = new List<Animator>();

		for (int i = 0; i < animatorComponents.Length; i++) 
		{
			ats.Add ((Animator)animatorComponents [i]);
		}

	}
	
	
	void Update () {

		//アニメーション制御
		if (Time.unscaledTime >= lastAttackTime + attackCooldown)
			SetSkillAnimation (0);


		//コンボリセット
		if (Time.unscaledTime >= lastAttackTime + attackCooldown && playerActionManager.onFloor) 
		{
			ResetAllAttack ();
		}

		if (Time.timeScale == 0) 
		{
			lastShootTime += Time.unscaledDeltaTime;
		}

		//リロード
		if (Time.unscaledTime >= lastShootTime + autoReloadTime && bulletNow < bulletMax 
			&& playerActionManager.playerState != PlayerActionManager.STATE.STATE_DEAD)
		{
			ReloadBullet ();
		}

		//現在残弾数の計算
		int bullet = 0;
		for (int i = 0; i < bulletMax; i++)
		{
			if (bulletReady [i])
				bullet++;
		}
		bulletNow = bullet;
		
	}

	public void ReloadBullet()
	{
		//バレット初期化
		bulletNow = bulletMax;
		playerActionVoiceManager.PlayAudio (PlayerActionVoiceManager.AudioType.GUNRELOAD);

		for (int i = 0; i < bulletMax; i++)
		{
			bulletReady [i] = true;
		}
	}

	public bool isInShootState()
	{
		if (Time.unscaledTime >= lastShootTime + shootStateTime) {
			return false;
		} 
		else 
		{
			return true;
		}
	}

	public int GetBulletCount()
	{
		int count = 0;
		for (int i = 0; i < bulletMax; i++)
		{
			if (bulletReady [i])
				count++;
		}
		return count;

	}

	public bool GunFire()
	{
		bool returnValue = false;

		if (playerCrystalSystem.IsNextExSkill ()) 
		{
			//クリスタルを使う
			playerCrystalSystem.ConsumeCrystal ();

			//EX技
			for(int i = 0; i < bulletMax; i++)
			{
				//returnValue = GunShootOnce ();

				PlayerBulletAttack1 script3;
				GameObject obj;

				Transform gunTransform, targetTransform;
				Vector3 distance, offsetVector;

				obj = PlayerAttackPoolerScript.current.GetbulletAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBulletAttack1> ();

				if (obj == null)
					return false;

				//発射位置とターゲット位置を取得する
				gunTransform = playerComponent.GetPlayerShoulderTransform ();
				targetTransform = enemyPoolerScript.getTargetEnemyTransform ();

				gunTransform.LookAt (targetTransform);

				obj.transform.position = gunTransform.position;
				obj.transform.rotation = gunTransform.rotation;
				obj.transform.Translate (0.0f, 0.1f, 0.3f);


				//ジャンプ状態になったら、弾の発射位置は普通状態と違う
				if (playerActionManager.onFloor) 
				{
					offsetVector.x = Mathf.Cos (Mathf.PI / bulletMax * i) * gunTransform.right.x;
					offsetVector.y = Mathf.Sin (Mathf.PI / bulletMax * i);
					offsetVector.z = Mathf.Cos (Mathf.PI / bulletMax * i) * gunTransform.right.z;
				} 
				else
				{
					offsetVector.x = Mathf.Cos (2 * Mathf.PI / bulletMax * i) * gunTransform.right.x;
					offsetVector.y = Mathf.Sin (2 * Mathf.PI / bulletMax * i);
					offsetVector.z = Mathf.Cos (2 * Mathf.PI / bulletMax * i) * gunTransform.right.z;
				}

				gunTransform.rotation = transform.rotation;

				//ベジェ曲線の必須情報を計算する
				script3.startPoint = obj.transform.position + offsetVector * 0.5f;
				script3.endPoint = targetTransform.position;

				distance = script3.endPoint - script3.startPoint;

				if (distance.magnitude <= limitDistance) 
				{
					script3.controllPoint1 = script3.startPoint + distance / 8;
					script3.controllPoint2 = script3.startPoint + distance * 3 / 4;
				} 
				else 
				{
					script3.controllPoint1 = script3.startPoint + distance / 8;
					script3.controllPoint1 = new Vector3 (script3.controllPoint1.x + offsetVector.x * 2.0f,
						script3.controllPoint1.y + offsetVector.y * 2.0f, 
						script3.controllPoint1.z + offsetVector.z * 2.0f);

					script3.controllPoint2 = script3.startPoint + distance * 3 / 4;
					script3.controllPoint2 = new Vector3 (script3.controllPoint2.x + offsetVector.x * 0.5f,
						script3.controllPoint2.y + offsetVector.y * 0.5f, 
						script3.controllPoint2.z + offsetVector.z * 0.5f);
				}
				obj.SetActive (true);


			}
			ReloadBullet ();
			lastShootTime = Time.unscaledTime;
			returnValue = true;
		} 
		else
		{
			//普通スキル
			returnValue = GunShootOnce ();
		}

		return returnValue;
	}

	private bool GunShootOnce()
	{
		bool passBullet = false;;
		int bulletNum = -1;
		int testTime = 0;

		//使えるバレットからランダムで1つを取る
		while (true) 
		{
			bulletNum = Random.Range (0, bulletMax-1);
			testTime++;

			if (testTime > 5)
			{
				for (int i = 0; i < bulletMax; i++)
				{
					if (bulletReady [i])
						bulletNum = i;
					if (i == bulletMax-1 && !bulletReady [i])
					{
						passBullet = true;
						bulletNum = -1;
					}
				}
				testTime = 0;
			}

			if (passBullet || bulletReady [bulletNum])
				break;
		}

	
		//全弾撃ち切れた
		if (bulletNum == -1)
			return false;


		//銃を撃つ
		bulletReady [bulletNum] = false;

		PlayerBulletAttack1 script3;
		GameObject obj;

		Transform gunTransform, targetTransform;
		Vector3 distance, offsetVector;

		obj = PlayerAttackPoolerScript.current.GetbulletAttackPooledObject ();
		script3 = obj.GetComponent<PlayerBulletAttack1> ();

		if (obj == null)
			return false;

		//発射位置とターゲット位置を取得する
		gunTransform = playerComponent.GetPlayerShoulderTransform ();
		targetTransform = enemyPoolerScript.getTargetEnemyTransform ();

		gunTransform.LookAt (targetTransform);

		obj.transform.position = gunTransform.position;
		obj.transform.rotation = gunTransform.rotation;
		obj.transform.Translate (0.0f, 0.1f, 0.3f);


		//ジャンプ状態になったら、弾の発射位置は普通状態と違う
		if (playerActionManager.onFloor) 
		{
			offsetVector.x = Mathf.Cos (Mathf.PI / bulletMax * bulletNum) * gunTransform.right.x;
			offsetVector.y = Mathf.Sin (Mathf.PI / bulletMax * bulletNum);
			offsetVector.z = Mathf.Cos (Mathf.PI / bulletMax * bulletNum) * gunTransform.right.z;
		} 
		else
		{
			offsetVector.x = Mathf.Cos (2 * Mathf.PI / bulletMax * bulletNum) * gunTransform.right.x;
			offsetVector.y = Mathf.Sin (2 * Mathf.PI / bulletMax * bulletNum);
			offsetVector.z = Mathf.Cos (2 * Mathf.PI / bulletMax * bulletNum) * gunTransform.right.z;
		}
			
		gunTransform.rotation = transform.rotation;

		//ベジェ曲線の必須情報を計算する
		script3.startPoint = obj.transform.position + offsetVector * 0.5f;
		script3.endPoint = targetTransform.position;

		distance = script3.endPoint - script3.startPoint;

		if (distance.magnitude <= limitDistance) 
		{
			script3.controllPoint1 = script3.startPoint + distance / 8;
			script3.controllPoint2 = script3.startPoint + distance * 3 / 4;
		} 
		else 
		{
			script3.controllPoint1 = script3.startPoint + distance / 8;
			script3.controllPoint1 = new Vector3 (script3.controllPoint1.x + offsetVector.x * 2.0f,
				script3.controllPoint1.y + offsetVector.y * 2.0f, 
				script3.controllPoint1.z + offsetVector.z * 2.0f);

			script3.controllPoint2 = script3.startPoint + distance * 3 / 4;
			script3.controllPoint2 = new Vector3 (script3.controllPoint2.x + offsetVector.x * 0.5f,
				script3.controllPoint2.y + offsetVector.y * 0.5f, 
				script3.controllPoint2.z + offsetVector.z * 0.5f);
		}

		obj.SetActive (true);

		lastShootTime = Time.unscaledTime;

		return true;
	}


	public bool isZeroVelocity()
	{
		if (Time.unscaledTime >= zeroVelocityTime) {
			return false;
		} 
		else 
		{
			return true;
		}
	}

	private void SetSkillAnimation(int skill)
	{
		//アニメーション状態の更新

		if (playerChangeForm.isNormal)
		{
			ats[0].SetInteger ("skill", skill);
		}
		else 
		{
			ats[1].SetInteger ("skill", skill);
		}
	}


	//ライト斬撃
	public bool SwordAttack()
	{
		if (Time.unscaledTime >= nextAttackTime && useNext)
		{
			switch(skillNow)
			{
			case 0:	//初期状態
				{
					skillNow = 1;
					SetSkillAnimation(skillNow);
					SetSkill ();	
					break;
				}
			case 1: //ライトパンチ×1
				{
					skillNow = 2;
					SetSkillAnimation(skillNow);
					SetSkill ();	
					break;
				}
			case 2: //ライトパンチ×2
				{
					skillNow = 3;
					SetSkillAnimation(skillNow);
					SetSkill ();	
					break;
				}
			case 3: //ライトパンチ×3
				{
					skillNow = 4;
					SetSkillAnimation(skillNow);
					SetSkill ();	
					break;
				}
			default:
				skillNow = 99;
				break;
			}

			if (skillNow <= 4)
			{
				
				return true;
			}
		}
		return false;
	}

	//ヘヴィ斬撃
	public bool HeavySwordAttack()
	{
		if (Time.unscaledTime >= nextAttackTime)
		{
			//EX技を使う
			if (playerCrystalSystem.IsNextExSkill ()) 
			{
				//クリスタルを使う
				playerCrystalSystem.ConsumeCrystal ();

				if (playerActionManager.onFloor)
				{
					switch(skillNow)
					{
					case 0:	//初期状態
						{
							//クロス斬撃EX
							skillNow = 12;
							SetSkillAnimation (-1);
							SetSkill ();	
							return true;
						}
					case 1: //ライトパンチ×1 -> 空中コンボ EX
						{
							skillNow = 15;
							SetSkillAnimation(2);
							SetSkill ();	
							return true;
						}
					case 2: //ライトパンチ×2 -> 対空攻撃 EX
						{
							skillNow = 16;
							SetSkillAnimation(3);
							SetSkill ();	
							return true;
						}
					case 3: //ライトパンチ×3 -> 「川」字攻撃 EX
						{
							skillNow = 17;
							SetSkillAnimation(4);
							SetSkill ();	
							return true;
						}
					default:
						{
							skillNow = 12;
							SetSkillAnimation(-1);
							SetSkill ();	
							return true;
						}
					}
				}
				else
				{
					skillNow = 13;
					SetSkillAnimation(7);
					SetSkill();	
					return true;
				}
			}


			//地面
			if(playerActionManager.onFloor)
			{
				switch(skillNow)
				{
				case 0:	//初期状態
					if ((thirdPersonController.isHoriMove || thirdPersonController.isVertiMove) && Time.unscaledTime >= nextSlideTime) 
					{
						//スライダー攻撃
						skillNow = 10;
						SetSkillAnimation(0);
						SetSkill();	
						return true;
					}
					else if(!thirdPersonController.isHoriMove && !thirdPersonController.isVertiMove)
					{
						//クロス斬撃
						skillNow = 5;
						SetSkillAnimation(1);
						SetSkill();	
						return true;
					}
					break;
					
				case 1: //ライトパンチ×1 -> 空中コンボ
					{
						skillNow = 6;
						SetSkillAnimation(2);
						SetSkill ();	
						return true;
					}
				case 2: //ライトパンチ×2 -> 対空攻撃
					{
						skillNow = 7;
						SetSkillAnimation(3);
						SetSkill ();	
						return true;
					}
				case 3: //ライトパンチ×3 -> 「川」字攻撃
					{
						skillNow = 8;
						SetSkillAnimation(4);
						SetSkill ();	
						return true;
					}
				default:
					break;
				}
			}
			//空中
			else
			{
				//ライト4以外なら使える
				if(skillNow != 4)
				{

					if ((thirdPersonController.isHoriMove || thirdPersonController.isVertiMove) && Time.unscaledTime >= nextSlideTime) 
					{
						//空中スライダー攻撃
						skillNow = 11;
						SetSkillAnimation(6);
						SetSkill();	
						return true;
					}
					else if(!thirdPersonController.isHoriMove && !thirdPersonController.isVertiMove)
					{
						//空中垂直攻撃
						skillNow = 9;
						SetSkillAnimation(5);
						SetSkill();	
						return true;
					}
				}
			}
		}
		return false;
	}

	//冷却時間あり
	public void ResetAttack()
	{
		skillNow = 0;
		lastAttackTime = Time.unscaledTime;
		//nextAttackTime = Time.unscaledTime;
	}

	//冷却時間なし
	public void ResetAllAttack()
	{
		if(skillNow != 15)
			SetSkillAnimation (0);

		skillNow = 0;
		useNext = true;
		lastAttackTime = Time.unscaledTime;
		nextAttackTime = Time.unscaledTime;
	}

	private void SetSkill()
	{
		Transform tmpTransform;
		GameObject obj;
		PlayerSwordAttack1 script1;
		PlayerSwordAttack2 script2;
		PlayerBlowAttack script3;
		
		switch (skillNow) {
		//ライト1
		case 1:
			{
				obj = PlayerAttackPoolerScript.current.GetLightSwordAttackPooledObject ();
				script1 = obj.GetComponent<PlayerSwordAttack1> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.3f, 0.4f, 0.5f);
				obj.transform.Rotate (35.7f, 0.0f, -33.0f);

				script1.moveNormalVector = Vector3.Cross ((tmpTransform.position - obj.transform.position), obj.transform.up);
				script1.reversedAttack = true;
				script1.stayTime = 0.35f;

				obj.SetActive (true);

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + skillCooldown;

				//空中のコマンド受付時間は地面より長い
				if (playerActionManager.onFloor)
					zeroVelocityTime = nextAttackTime;
				else
					zeroVelocityTime = lastAttackTime + zeroCooldown;
			
				break;
			}

		//ライト2
		case 2:
			{
				obj = PlayerAttackPoolerScript.current.GetLightSwordAttackPooledObject ();
				script1 = obj.GetComponent<PlayerSwordAttack1> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (-0.3f, 0.4f, 0.5f);
				obj.transform.Rotate (35.7f, 0.0f, 33.0f);

				script1.moveNormalVector = Vector3.Cross ((tmpTransform.position - obj.transform.position), obj.transform.up);
				script1.reversedAttack = true;
				script1.stayTime = 0.35f;

				obj.SetActive (true);
			
				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + skillCooldown;

				//空中のコマンド受付時間は地面より長い
				if (playerActionManager.onFloor)
					zeroVelocityTime = nextAttackTime;
				else
					zeroVelocityTime = lastAttackTime + zeroCooldown;
			
				break;
			}

		//ライト3
		case 3:
			{
				obj = PlayerAttackPoolerScript.current.GetLightSwordAttackPooledObject ();
				script1 = obj.GetComponent<PlayerSwordAttack1> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.4f, 0.5f);
				obj.transform.Rotate (35.7f, 0.0f, 0.0f);

				script1.moveNormalVector = Vector3.Cross ((tmpTransform.position - obj.transform.position), obj.transform.up);
				script1.reversedAttack = true;
				script1.stayTime = 0.35f;

				obj.SetActive (true);
			
				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + skillCooldown;

				//空中のコマンド受付時間は地面より長い
				if (playerActionManager.onFloor)
					zeroVelocityTime = nextAttackTime;
				else
					zeroVelocityTime = lastAttackTime + zeroCooldown;
			
				break;
			}

		//ライト4
		case 4:
			{
				obj = PlayerAttackPoolerScript.current.GetLightSwordAttackPooledObject ();
				script1 = obj.GetComponent<PlayerSwordAttack1> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.3f, 0.2f, 0.0f);
				obj.transform.Rotate (35.7f, 0.0f, -90.0f);

				script1.moveNormalVector = new Vector3 (0.0f, 1.0f, 0.0f);
				script1.reversedAttack = false;
				script1.stayTime = 0.35f;

				obj.SetActive (true);
			
				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 2 * skillCooldown;
				zeroVelocityTime = nextAttackTime;

				//この技使ったら、コンボ終了
				if (!playerActionManager.onFloor)
					useNext = false;
				
				break;
			}
		
		//ヘヴィ1
		case 5:
			{
				//クロス斬撃
				for (int i = 0; i < 2; i++) {

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;
					obj.transform.Translate (1.5f - (3.0f * i), 1.5f, 1.3f);
					obj.transform.Rotate (90.0f, 0.0f, 0.0f);

					script2.disappearAuto = true;

					if (i == 0) {
						script2.moveVector = -tmpTransform.right + new Vector3 (0.0f, -1.0f, 0.0f);
					} else {
						script2.moveVector = tmpTransform.right + new Vector3 (0.0f, -1.0f, 0.0f);
					}

					script2.moveVector.Normalize ();
					script2.stayTime = 0.6f;
					script2.attackSpeed = 12;

					obj.SetActive (true);	
				}

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 1.08f);
				obj.transform.localScale = new Vector3 (2.2f, 2.2f, 2.2f);

				script3.disappearAuto = true;
				script3.tracePlayer = false;
				script3.specifiedDirection = true;
				script3.targetTracePlayer = false;
				script3.blowSpeed = 10.0f;
				script3.blowVector = transform.forward * 3 + transform.up;
				script3.blowVector.Normalize ();
				script3.stayTime = 0.2f;

				obj.SetActive (true);	

			
				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 1 * attackCooldown;
				zeroVelocityTime = nextAttackTime;
				break;
			}

		//ヘヴィ2
		case 6:
			{
				//空中コンボ
				obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
				script2 = obj.GetComponent<PlayerSwordAttack2> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.4f, 0.8f);
				obj.transform.Rotate (90.0f, 0.0f, 0.0f);

				script2.disappearAuto = true;
				script2.moveVector = new Vector3 (0.0f, 1.0f, 0.0f);
				script2.attackSpeed = 13f;
				script2.stayTime = 0.25f;
			
				obj.SetActive (true);

				//プレイヤーの速度変化
				//playerRb.velocity = new Vector3 (0.0f, 18.0f, 0.0f) * (1f / Time.timeScale);

				//playerActionManager.playerAction = PlayerActionManager.ACTION.ACTION_JUMP;

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 1.0f);
				obj.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);

				script3.disappearAuto = true;
				script3.tracePlayer = false;
				script3.specifiedDirection = true;
				script3.targetTracePlayer = false;
				script3.blowSpeed = 10.0f;
				script3.blowVector = transform.up;
				script3.blowVector.Normalize ();
				script3.stayTime = 0.2f;

				/*
				script3.disappearAuto = true;
				script3.tracePlayer = true;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = true;
				script3.blowVector = playerRb.velocity;
				script3.blowSpeed = 0.6f;
				script3.stayTime = 0.30f;
				*/

				obj.SetActive (true);


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				zeroVelocityTime = nextAttackTime;
				break;
			}

		//ヘヴィ3
		case 7:
			{
				//対空攻撃
				obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
				script2 = obj.GetComponent<PlayerSwordAttack2> ();
			
				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.8f, 1.3f);
				obj.transform.Rotate (45.0f, 0.0f, 0.0f);

				script2.disappearAuto = true;
				script2.moveVector = tmpTransform.forward + tmpTransform.up;
				script2.moveVector.Normalize ();
				script2.stayTime = 0.25f;
				script2.attackSpeed = 13;

				obj.SetActive (true);

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				zeroVelocityTime = nextAttackTime;
				break;
			}

		//ヘヴィ4
		case 8:
			{
				//「川」字斬撃
				for (int i = 0; i < 3; i++) {

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();
			
					if (obj == null)
						return;
			
					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;
					obj.transform.Translate (0.5f - (0.5f * i), 2.0f, 1.3f);
					obj.transform.Rotate (90.0f, 0.0f, 0.0f);

					script2.disappearAuto = true;
					script2.moveVector = new Vector3 (0.0f, -1.0f, 0.0f);
					script2.stayTime = 0.8f;
					script2.attackSpeed = 12;
			
					obj.SetActive (true);	
				}

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 3 * skillCooldown;
				zeroVelocityTime = nextAttackTime;

				break;
			}
		
		//空中ヘヴィ垂直攻撃
		case 9:
			{
				obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
				script2 = obj.GetComponent<PlayerSwordAttack2> ();

				if (obj == null)
					return;
			
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();
			
				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.4f, 0.8f);
				obj.transform.Rotate (90.0f, 0.0f, 0.0f);

				script2.disappearAuto = false;
				script2.moveVector = new Vector3 (0.0f, -1.0f, 0.0f);
				script2.stayTime = 0.0f;

				obj.SetActive (true);

				//プレイヤーの速度変化
				playerRb.velocity = new Vector3 (0.0f, -20.0f, 0.0f);

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 1.0f);
				obj.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);

				script3.disappearAuto = false;
				script3.tracePlayer = true;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = true;
				script3.blowVector = playerRb.velocity;
				script3.blowSpeed = 0.6f;

				obj.SetActive (true);

			
				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 2 * attackCooldown;
				zeroVelocityTime = Time.unscaledTime;
				break;
			}

		//スライダー斬撃
		case 10:
			{
				
				obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
				script2 = obj.GetComponent<PlayerSwordAttack2> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.4f, 0.8f);
				obj.transform.Rotate (90.0f, 0.0f, 0.0f);

				script2.disappearAuto = false;
				script2.moveVector = tmpTransform.forward;
				script2.stayTime = 0.0f;

				obj.SetActive (true);

				//プレイヤーの速度変化
				transform.Translate (0.0f, 0.5f, 0.0f);
				playerRb.velocity = tmpTransform.forward * 20 * (1f / Time.timeScale) + tmpTransform.up * 2;

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				nextSlideTime = nextAttackTime;
				zeroVelocityTime = nextAttackTime;
				break;
			}

		//空中ヘヴィスライダー攻撃
		case 11:
			{
				obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
				script2 = obj.GetComponent<PlayerSwordAttack2> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.4f, 0.8f);
				obj.transform.Rotate (90.0f, 0.0f, 0.0f);

				script2.disappearAuto = false;
				script2.moveVector = new Vector3 (0.0f, -1.0f, 0.0f);
				script2.stayTime = 0.0f;

				obj.SetActive (true);

				//プレイヤーの速度変化
				playerRb.velocity = new Vector3 (0.0f, -20.0f, 0.0f) + transform.forward * 20.0f;

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 1.0f);
				obj.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);

				script3.disappearAuto = false;
				script3.tracePlayer = true;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = true;
				script3.blowVector = playerRb.velocity;
				script3.blowSpeed = 0.6f;

				obj.SetActive (true);


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 2 * attackCooldown;
				zeroVelocityTime = Time.unscaledTime;
				break;
			}

		//EXヘヴィ(地上)
		case 12:
			{
				for (int i = 0; i < 10; i++) {

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;
					obj.transform.Translate (2.0f * Mathf.Sin (((Mathf.PI * 2.0f) / 10) * i), -1.5f, 2.0f * Mathf.Cos (((Mathf.PI * 2.0f) / 10) * i));
					obj.transform.Rotate (90.0f, 360f / 10.0f * i, 0.0f);

					script2.disappearAuto = true;
					script2.moveVector = tmpTransform.up;

					script2.moveVector.Normalize ();
					script2.stayTime = 0.6f;
					script2.attackSpeed = 50;

					obj.SetActive (true);	
				}

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 0.0f);
				obj.transform.localScale = new Vector3 (5.5f, 5.5f, 5.5f);

				script3.disappearAuto = true;
				script3.tracePlayer = false;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = false;
				script3.blowSpeed = 15.0f;
				script3.blowVector = transform.forward * 3 + transform.up;
				script3.blowVector.Normalize ();
				script3.stayTime = 0.2f;

				obj.SetActive (true);	


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 1 * attackCooldown;
				zeroVelocityTime = nextAttackTime;
				break;
			}

		//EXヘヴィ(空中)
		case 13:
			{
				for (int i = 0; i < 10; i++) {

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;
					obj.transform.Translate (2.0f * Mathf.Sin (((Mathf.PI * 2.0f) / 10) * i), 1.0f, 2.0f * Mathf.Cos (((Mathf.PI * 2.0f) / 10) * i));
					obj.transform.Rotate (90.0f, 360f / 10.0f * i, 0.0f);

					script2.disappearAuto = false;
					script2.moveVector = new Vector3 (0.0f, -1.0f, 0.0f);
					script2.stayTime = 0.0f;

					obj.SetActive (true);	
				}

				//プレイヤーの速度変化
				playerRb.velocity = new Vector3 (0.0f, -20.0f, 0.0f);

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 0.0f);
				obj.transform.localScale = new Vector3 (5.5f, 5.5f, 5.5f);

				script3.disappearAuto = false;
				script3.tracePlayer = true;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = true;
				script3.blowVector = playerRb.velocity;
				script3.blowSpeed = 0.6f;

				obj.SetActive (true);


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 2 * attackCooldown;
				zeroVelocityTime = Time.unscaledTime;
				break;
			}
			/*
			//スライダー斬撃EX
		case 14:
			{
				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				for (int i = 0; i < 3; i++)
				{
					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;

					if (i == 0) 
					{
						obj.transform.Translate (0.0f, 0.6f, 0.8f);
					}
					else if (i == 1) 
					{
						obj.transform.Translate (-1.0f, -0.2f, 0.8f);
					}
					else if (i == 1)
					{
						obj.transform.Translate (1.0f, -0.2f, 0.8f);
					}
						
					obj.transform.Rotate (90.0f, 0.0f, 0.0f);

					script2.disappearAuto = false;
					script2.moveVector = tmpTransform.forward;
					script2.stayTime = 0.0f;

					obj.SetActive (true);
				}

				//プレイヤーの速度変化
				transform.Translate (0.0f, 0.5f, 0.0f);
				playerRb.velocity = tmpTransform.forward * 20 * (1f / Time.timeScale) + tmpTransform.up * 2;

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				nextSlideTime = nextAttackTime;
				zeroVelocityTime = nextAttackTime;
				break;
			}
			*/
			//ヘヴィ2
		case 15:
			{
				//空中コンボEX

				for (int i = 0; i < 2; i++) 
				{

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;

					if(i == 0)
						obj.transform.Translate (-0.5f, 0.4f, 0.8f);
					else if(i == 1)
						obj.transform.Translate (0.5f, 0.4f, 0.8f);

					obj.transform.Rotate (90.0f, 0.0f, 0.0f);

					script2.disappearAuto = false;
					script2.moveVector = new Vector3 (0.0f, 1.0f, 0.0f);
					script2.stayTime = 0.30f;

					obj.SetActive (true);
				}

				//プレイヤーの速度変化
				playerRb.velocity = new Vector3 (0.0f, 18.0f, 0.0f) * (1f / Time.timeScale);

				playerActionManager.playerAction = PlayerActionManager.ACTION.ACTION_JUMP;

				//ブローエリア設置
				obj = PlayerAttackPoolerScript.current.GetblowAttackPooledObject ();
				script3 = obj.GetComponent<PlayerBlowAttack> ();

				if (obj == null)
					return;

				tmpTransform = playerComponent.GetPlayerShoulderTransform ();

				obj.transform.position = tmpTransform.position;
				obj.transform.rotation = tmpTransform.rotation;
				obj.transform.Translate (0.0f, 0.5f, 1.0f);
				obj.transform.localScale = new Vector3 (2.0f, 2.0f, 2.0f);

				script3.disappearAuto = true;
				script3.tracePlayer = true;
				script3.specifiedDirection = false;
				script3.targetTracePlayer = true;
				script3.blowVector = playerRb.velocity;
				script3.blowSpeed = 0.6f;
				script3.stayTime = 0.30f;

				obj.SetActive (true);


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				zeroVelocityTime = Time.unscaledTime;
				break;
			}

			//ヘヴィ3
		case 16:
			{
				//対空攻撃EX

				for (int i = 0; i < 3; i++)
				{

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;

					if(i == 0)
						obj.transform.Translate (0.0f, 0.8f, 1.3f);
					else if(i == 1)
						obj.transform.Translate (0.8f, 0.0f, 1.3f);
					else if(i == 2)
						obj.transform.Translate (-0.8f, 0.0f, 1.3f);

					obj.transform.Rotate (45.0f, 0.0f, 0.0f);

					script2.disappearAuto = true;
					script2.moveVector = tmpTransform.forward + tmpTransform.up;
					script2.moveVector.Normalize ();
					script2.stayTime = 0.25f;
					script2.attackSpeed = 13;

					obj.SetActive (true);
				}

				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + attackCooldown;
				zeroVelocityTime = nextAttackTime;
				break;
			}
			//ヘヴィ4
		case 17:
			{
				//「川」字斬撃EX
				for (int i = 0; i < 5; i++) {

					obj = PlayerAttackPoolerScript.current.GetHeavySwordAttackPooledObject ();
					script2 = obj.GetComponent<PlayerSwordAttack2> ();

					if (obj == null)
						return;

					tmpTransform = playerComponent.GetPlayerShoulderTransform ();

					obj.transform.position = tmpTransform.position;
					obj.transform.rotation = tmpTransform.rotation;

					if (i == 0)
						obj.transform.Translate (0.5f, -1.0f, 1.3f);
					else if (i == 1)
						obj.transform.Translate (-0.5f, 2.0f, 1.3f);
					else if(i == 2)
						obj.transform.Translate (-2.0f, 1.0f, 1.3f);
					else if(i == 3)
						obj.transform.Translate (2.0f, 0.0f, 1.3f);
					else if(i == 4)
						obj.transform.Translate (0.0f, 0.0f, 1.0f);
					
					obj.transform.Rotate (90.0f, 0.0f, 0.0f);

					script2.disappearAuto = true;
					if(i == 0)
						script2.moveVector = -obj.transform.forward;
					else if(i == 1)
						script2.moveVector = obj.transform.forward;
					else if(i == 2)
						script2.moveVector = obj.transform.right;
					else if(i == 3)
						script2.moveVector = -obj.transform.right;
					else if(i == 4)
						script2.moveVector = obj.transform.up;

					script2.stayTime = 0.3f;
					script2.attackSpeed = 12;

					obj.SetActive (true);	
				}


				lastAttackTime = Time.unscaledTime;
				nextAttackTime = lastAttackTime + 3 * skillCooldown;
				zeroVelocityTime = nextAttackTime;

				break;
			}
			
		default:
			break;
			
		}
	}
	
}
