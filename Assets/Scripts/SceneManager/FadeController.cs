using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour {

	public float fadeSpeed;
	public bool isGame;

	private SceneController sceneController;
	private GUITexture guiTexture;

	public CanvasGroup UICanvasGroup;


	void Start () {

		//シーン状態の取得
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null) {
			Debug.Log ("Cannot find tag 'SceneManager'");
		}

		sceneController = sceneControllerObject.GetComponent<SceneController> ();
	
		guiTexture = GetComponent<GUITexture> ();
	}

	void FixedUpdate () {
		//フェードイン when Scene Start
		Color tmpColor = guiTexture.color;

		if (!sceneController.isStart ())
		{
			tmpColor.a -= fadeSpeed * Time.unscaledDeltaTime;

			if(isGame)
			{
				UICanvasGroup.alpha += fadeSpeed * 2 * Time.unscaledDeltaTime;
			}

			if(tmpColor.a <= 0.0f)
			{
				tmpColor.a = 0.0f;
				if(!isGame)
				{
					sceneController.sceneStart();
				}
			}
			guiTexture.color = tmpColor;
		}

		//フェードアウト when Scene closing
		if (sceneController.isClosing ())
		{
			tmpColor.a += fadeSpeed * Time.unscaledDeltaTime;

			if (isGame)
			{
				UICanvasGroup.alpha -= fadeSpeed * 2 * Time.unscaledDeltaTime;
			}

			if(tmpColor.a >= 0.5f)
			{
				tmpColor.a = 0.5f;
				//if(!isGame)
				//{
					sceneController.sceneFinish();
				//}
			}
			guiTexture.color = tmpColor;
		}

	}

}
