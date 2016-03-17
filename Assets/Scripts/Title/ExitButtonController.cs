using UnityEngine;
using System.Collections;

public class ExitButtonController : MonoBehaviour {

	private SceneController sceneController;

	public BGMControllerScript BGMController;

	private AudioSource audioSource;


	void Start () 
	{
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");

		if (sceneControllerObject == null) {
			Debug.Log("Cannot fin tag 'SceneManeger'");
		}
		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		audioSource = GetComponent<AudioSource> ();

		BGMController.SetVolume (0.3f);
	}
	

	void Update () {

		if ((Input.GetButtonDown("Submit") || Input.GetButtonDown("Pause")) && !sceneController.isClosing() && sceneController.isStart())
		{
			audioSource.Play ();
			BGMController.SetVolume (0.0f);
			sceneController.LevelNowPlus (1);
			sceneController.sceneClose();
		}
	
	}
}
