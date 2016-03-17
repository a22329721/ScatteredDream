using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour {

	public GameObject player;
	private PlayerActionManager playerActionManager;

	public GameObject mainCamera;
	public GameObject cameraCollisionBox;

	//重力
	public float gravity;

	//カメラとプレイヤーの最大距離
	public float MaxDistance;
	//プレイヤーの移動速度
	public float inputSpeed;
	public float moveSpeed;

	//Lerpの移動係数
	public float smoothing;

	//rotationアニメーション
	private Vector3 newRotation;
	private Vector3 oldRotation;

	//カメラ当たりセンサー
	public CollisionCounter cameraProbe;
	//カメラはコリーダに影響された垂直移動比例
	public float verticalRatio;

	//プレイヤーの前方向は障害物があれば、カメラはプレイヤーが移動できるまで回転する。
	public CollisionCounter frontProbe;

	//左右と前後移動の合成
	public bool allReset;
	public bool isHoriMove;
	public bool isVertiMove;
	public Vector3 horiVelocity;
	public Vector3 vertiVelocity;

	//Y方向の速度は前のフレームから継承する
	private float originYVelocity;

	//正反対の方向に移動するなら、スピードは一瞬にゼロになって、待機状態に戻る
	//そうしないために速度はすぐにゼロに戻らなくて、新しいインプットがいないことを確認した上でゼロに戻る。
	private float speedResetTimer;

	private SceneController sceneController;
	private Rigidbody playerRb;


	void Start () {

		//シーン状態の取得
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null) {
			Debug.Log ("Third person controller cannot find tag 'SceneManager'");
		}

		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		playerRb = player.GetComponent<Rigidbody> ();
		playerActionManager = player.GetComponent<PlayerActionManager> ();

		moveSpeed = inputSpeed;
		allReset = false;
	}


	void FixedUpdate () {

		//ゲームスタートのカメラズームイン
		if (!sceneController.isStart ()) {
			StartZoomIn();
			return;
		}

		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_SWORD
			|| playerActionManager.playerState == PlayerActionManager.STATE.STATE_MAGICALBEAM)
		{
			playerRb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
		}

		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_DEAD 
			|| playerActionManager.playerState == PlayerActionManager.STATE.STATE_MAGICALBEAM)
			return;

		allReset = false;



		if ((playerActionManager.playerState == PlayerActionManager.STATE.STATE_GUN
			&& playerActionManager.playerAction == PlayerActionManager.ACTION.ACTION_JUMP))
		{
			playerRb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			Vector3 distance = mainCamera.transform.position - player.transform.position;
			distance.y = 0.0f;
			CameraChasePlayer (distance);
			return;
		} 

		CameraMove ();

		PlayerMove();

		this.originYVelocity -= gravity * (1f / Time.timeScale) * (1f / Time.timeScale) * Time.fixedDeltaTime;
		playerRb.velocity = new Vector3 (playerRb.velocity.x, this.originYVelocity, playerRb.velocity.z);

	}

	void CameraMove()
	{
		//入力情報の取得
		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");
		
		//インプット初期化
		if (playerActionManager.playerAction != PlayerActionManager.ACTION.ACTION_JUMP) {
			this.isVertiMove = false;
			this.isHoriMove = false;
		}
		this.horiVelocity = Vector3.zero;
		this.vertiVelocity = Vector3.zero;
		
		this.originYVelocity = playerRb.velocity.y;

		
		//カメラ前方向の取得
		Vector3 foward = mainCamera.transform.forward;
		foward.y = 0.0f;
		foward.Normalize ();
		
		//カメラ右方向の取得
		Vector3 right = mainCamera.transform.right;
		right.y = 0.0f;
		right.Normalize ();

		Vector3 distance = mainCamera.transform.position - player.transform.position;
		distance.y = 0.0f;

		//ジャンプ状態になったら、カメラもプレイヤーにを追跡する
		if (playerActionManager.playerAction == PlayerActionManager.ACTION.ACTION_JUMP
			|| playerActionManager.playerState == PlayerActionManager.STATE.STATE_SWORDMOVE) {
			CameraChasePlayer (distance);
			//return;
		}

		//カメラとプレイヤーの現在距離
		distance = mainCamera.transform.position - player.transform.position;
		distance.y = 0.0f;
		
		//前後移動
		//歩くと走るの速度調整
		if ((Mathf.Abs (vertical) >= 0.65f) || (Mathf.Abs (horizontal) >= 0.65f)) {
			moveSpeed = inputSpeed * 1.8f;
		} else {
			moveSpeed = inputSpeed;
		}
		
		if (vertical > 0.0f) {
			
			this.vertiVelocity = foward * moveSpeed;
			isVertiMove = true;

			if (distance.magnitude > MaxDistance) {
				
				CameraChasePlayer (distance);
				
			}
			CalculateHeight ();
			
		} else if (vertical < 0.0f) {
			
			this.vertiVelocity = -foward * moveSpeed;
			isVertiMove = true;
			
			if (cameraProbe.counter == 0) {
				
				CameraChasePlayer (distance);
				
			} else {
				
				CalculateHeight ();
				
			}
		}

		//左右移動
		if (playerActionManager.playerAction != PlayerActionManager.ACTION.ACTION_JUMP
		   && playerActionManager.playerState != PlayerActionManager.STATE.STATE_SWORD) 
		{
			if (horizontal > 0.0f) {

				this.horiVelocity = right * moveSpeed;
				isHoriMove = true;

				if (frontProbe.counter > 0) {
					cameraCollisionBox.transform.position += -right * moveSpeed / 2 * Time.unscaledDeltaTime;	
				} else {
					cameraCollisionBox.transform.position += this.horiVelocity * Time.fixedDeltaTime * (1f / Time.timeScale);
				}

			} else if (horizontal < 0.0f) {

				this.horiVelocity = -right * moveSpeed;
				isHoriMove = true;

				if (frontProbe.counter > 0) {
					cameraCollisionBox.transform.position += right * moveSpeed / 2 * Time.unscaledDeltaTime;	
				} else {
					cameraCollisionBox.transform.position += this.horiVelocity * Time.fixedDeltaTime * (1f / Time.timeScale);
				}

			}
		}

	}


	public void CameraChasePlayer(Vector3 distance)
	{
		//プレイヤーの追跡
		float originY = cameraCollisionBox.transform.position.y;
		Vector3 newPosition = player.transform.position + distance.normalized * MaxDistance;
		newPosition.y = originY;
		cameraCollisionBox.transform.position = newPosition;
	}

	void CalculateHeight()
	{
		//カメラのワールド座標
		Vector3 originalVector = mainCamera.transform.position;
		//カメラとカメラコリーダの相対座標
		Vector3 localVector = mainCamera.transform.localPosition;

		//Yを消して、平面X-Zの距離を計算する
		originalVector.y = player.transform.position.y;

		float y = MaxDistance - (originalVector - player.transform.position).magnitude;
		localVector.y = y;
		mainCamera.transform.localPosition = localVector * verticalRatio;

	}

	void StartZoomIn()
	{
		Vector3 readyVector = player.transform.position - cameraCollisionBox.transform.position;
		float tmpY = readyVector.y;
		readyVector.y = 0.0f;
		readyVector.Normalize();
		readyVector *= MaxDistance;
		readyVector = new Vector3(readyVector.x, tmpY, readyVector.z);
		cameraCollisionBox.transform.position = Vector3.Lerp (cameraCollisionBox.transform.position,
		                                                      cameraCollisionBox.transform.position + readyVector,
		                                                       Time.deltaTime);
		
		readyVector = player.transform.position - cameraCollisionBox.transform.position;
		tmpY = readyVector.y;
		readyVector.y = 0.0f;
		if(readyVector.magnitude <= MaxDistance)
		{
			sceneController.sceneStart();
		}
	}

	void PlayerMove()
	{
		float tmpVelocityY;

		Vector3 tmpVelocity;
		tmpVelocity = playerRb.velocity;

		Vector3 distance = mainCamera.transform.position - player.transform.position;
		distance.y = 0.0f;

		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_ATTCKED)
		{
			playerRb.velocity = tmpVelocity;
			CameraChasePlayer (distance);
			return;
		}
			

		if (playerActionManager.playerState != PlayerActionManager.STATE.STATE_SWORD) {

			//速度ベクトルの合成
			if (this.isHoriMove && this.isVertiMove) {
				tmpVelocityY = playerRb.velocity.y;
				playerRb.velocity = this.horiVelocity + this.vertiVelocity;
				playerRb.velocity = new Vector3 (playerRb.velocity.x, tmpVelocityY, playerRb.velocity.z) * (1f / Time.timeScale);

			} else if (this.isHoriMove) {
				tmpVelocityY = playerRb.velocity.y;
				playerRb.velocity = this.horiVelocity;
				playerRb.velocity = new Vector3 (playerRb.velocity.x, tmpVelocityY, playerRb.velocity.z) * (1f / Time.timeScale);

			} else if (this.isVertiMove) {
				tmpVelocityY = playerRb.velocity.y;
				playerRb.velocity = this.vertiVelocity;
				playerRb.velocity = new Vector3 (playerRb.velocity.x, tmpVelocityY, playerRb.velocity.z) * (1f / Time.timeScale);

			}
		}


		//プレイヤーの向き方向に回転する
		if ((this.isHoriMove || this.isVertiMove) && playerActionManager.playerState != PlayerActionManager.STATE.STATE_SWORDMOVE) {
			float rotate = Mathf.Atan2 (playerRb.velocity.x, playerRb.velocity.z);
			oldRotation = player.transform.rotation.eulerAngles;

			//方向アナログから手をを離しても、0度に戻らない
			if(rotate != 0.0f)
				newRotation = new Vector3(0.0f, rotate / Mathf.PI * 180.0f, 0.0f);

			Quaternion oldRotate = Quaternion.Euler (oldRotation.x, oldRotation.y, oldRotation.z);
			Quaternion newRotate = Quaternion.Euler (newRotation.x, newRotation.y, newRotation.z);

			if(!oldRotate.Equals(newRotate))
			{
				//oldRotate = Quaternion.Lerp(oldRotate, newRotate, smoothing * Time.unscaledDeltaTime);
				oldRotate.eulerAngles = new Vector3(0.0f,Mathf.LerpAngle(oldRotate.eulerAngles.y, newRotate.eulerAngles.y, smoothing * Time.unscaledDeltaTime) ,0.0f);
				if(oldRotate.y - newRotate.y <= 0.01f)
				{
					oldRotate = newRotate;
				}
			}
			player.transform.rotation = oldRotate;
			this.speedResetTimer = 0.1f;

		}
		else
		{
			if(this.speedResetTimer >= 0.0f){

				this.speedResetTimer -= Time.deltaTime;

			}else{
				playerRb.velocity = Vector3.zero;
				allReset = true;
			}
		}

		if (playerActionManager.playerAction == PlayerActionManager.ACTION.ACTION_JUMP 
			|| playerActionManager.playerState == PlayerActionManager.STATE.STATE_SWORD 
			|| playerActionManager.playerState == PlayerActionManager.STATE.STATE_SWORDMOVE) {
			playerRb.velocity = tmpVelocity;
		}

	}

}
