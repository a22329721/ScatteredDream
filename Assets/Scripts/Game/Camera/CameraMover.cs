using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

	public GameObject player;

	public Transform lookAtObject;
	public float angleSpeed;

	private bool isReverseLR, isReverseUD;

	//カメラとプレイヤーのY値の差
	public float offsetY;

	//カメラ垂直移動範囲の上限値
	public float maxYRange;

	private SceneController sceneController;
	public GameObject pauseManagerObject;
	private PauseManager pauseManager;

	public PlayerActionManager playerActionManager;

	
	void Start () 
	{
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null) {
			Debug.Log("CamaraMover cannot find tag 'SceneManager'");
		}
		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		pauseManager = pauseManagerObject.GetComponent<PauseManager> ();
	
		offsetY = transform.position.y - player.transform.position.y;
		isReverseLR = true;
		isReverseUD = true;
	}

	void LateUpdate () {
		if (!sceneController.isStart ()) {
			return;
		}

		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_MAGICALBEAM)
			angleSpeed = 25f;
		else
			angleSpeed = 160f;


		float horizontal = Input.GetAxis("CameraHorizontal");
		float vertical = Input.GetAxis("CameraVertical");

		//二種類のカメラ回転タイプ
		if(isReverseLR)
			horizontal *= -1;

		if(isReverseUD)
			vertical *= -1;

		if (pauseManager.isGamePaused())
		{
			return;
		}

		//水平回転
		transform.RotateAround(lookAtObject.position, Vector3.up, -horizontal * angleSpeed * Time.unscaledDeltaTime);
		if (playerActionManager.playerState == PlayerActionManager.STATE.STATE_MAGICALBEAM) 
		{
			playerActionManager.gameObject.transform.RotateAround (playerActionManager.gameObject.transform.position,
				Vector3.up, -horizontal * angleSpeed * Time.unscaledDeltaTime);
		}

		//もし移動範囲ないなら、垂直回転する。
		if ((transform.position.y < lookAtObject.transform.position.y + maxYRange && vertical > 0.0f) ||
		    (transform.position.y > lookAtObject.transform.position.y - maxYRange && vertical < 0.0f) ) {

			Vector3 moveNormalVector = Vector3.Cross((lookAtObject.transform.position - transform.position), Vector3.up);

			transform.RotateAround(lookAtObject.position, moveNormalVector, -vertical * angleSpeed * Time.unscaledDeltaTime);
			offsetY = transform.position.y - player.transform.position.y;
		}

	}

	public void SetCemeraLRReverse(bool data)
	{
		isReverseLR = data;
	}

	public void SetCemeraUDReverse(bool data)
	{
		isReverseUD = data;
	}

	public void ReserveCamera(int num)
	{
		if (num == 1) 
		{
			isReverseLR = !isReverseLR;
		}
		else if (num == 2)
		{
			isReverseUD = !isReverseUD;
		}
	}

}
