using UnityEngine;
using System.Collections;

public class OptionController : MonoBehaviour {

	enum OC_OptionStage
	{
		PRESS_TO_START,
		CHOICE
	};

	enum OC_Option
	{
		NONE,
		START_GAME, 
		START_TUTORIAL
	};

	private OC_OptionStage stage = OC_OptionStage.PRESS_TO_START;
	private OC_Option option = OC_Option.NONE;

	public GameObject press_enter_gobj;
	public GameObject start_game_gobj;
	public GameObject start_tutorial_gobj;

	public Material start_game_mat;
	public Material start_turtorial_mat;

	private float startTimer;
	private SceneController sceneController;
	public BGMControllerScript BGMController;

	private AudioSource audioSource;
	public AudioClip pressStart, curserSelect, decision; 

	void Start ()
	{
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");

		if (sceneControllerObject == null)
		{
			Debug.Log("Cannot find tag 'SceneManeger'");
		}

		sceneController = sceneControllerObject.GetComponent<SceneController> ();
		BGMController.SetVolume (0.5f);

		audioSource = GetComponent<AudioSource> ();

		startTimer = 0.0f;
	}
	

	void Update () 
	{
		startTimer += Time.deltaTime;

		if (startTimer >= 1.5f) 
		{
			sceneController.sceneStart ();
		}

		if (!sceneController.isClosing ())
		{
			//選択
			if ((Input.GetAxis ("Vertical") > 0.0f || Input.GetAxis ("VerticalSelect") > 0.0f) 
				&& stage == OC_OptionStage.CHOICE && option == OC_Option.START_TUTORIAL) 
			{
				option = OC_Option.START_GAME;

				audioSource.clip = curserSelect;
				audioSource.Play ();
			}

			if ((Input.GetAxis ("Vertical") < 0.0f || Input.GetAxis ("VerticalSelect") < 0.0f)
				&& stage == OC_OptionStage.CHOICE && option == OC_Option.START_GAME)
			{
				option = OC_Option.START_TUTORIAL;

				audioSource.clip = curserSelect;
				audioSource.Play ();
			}

			//確定
			if (option == OC_Option.START_GAME && (Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Pause"))) {
				BGMController.SetVolume (0.0f);
				sceneController.LevelNowPlus (2);
				sceneController.sceneClose ();

				audioSource.clip = decision;
				audioSource.Play ();
			}

			if (option == OC_Option.START_TUTORIAL && (Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Pause"))) {
				BGMController.SetVolume (0.0f);
				sceneController.LevelNowPlus (1);
				sceneController.sceneClose ();

				audioSource.clip = decision;
				audioSource.Play ();
			}

			if (stage == OC_OptionStage.PRESS_TO_START && sceneController.isStart () 
				&& (Input.GetButtonDown ("Submit") || Input.GetButtonDown ("Pause")))
			{
				audioSource.clip = pressStart;
				audioSource.Play ();

				stage = OC_OptionStage.CHOICE;
				option = OC_Option.START_GAME;
				press_enter_gobj.SetActive (false);

				start_game_gobj.SetActive (true);
				start_tutorial_gobj.SetActive (true);
				//sceneController.sceneClose();
			}
		}

		//材質の色を変化
		if (option == OC_Option.START_GAME) 
		{
			start_game_gobj.GetComponent<Renderer>().material.color = start_game_mat.color ;
			start_tutorial_gobj.GetComponent<Renderer>().material.color = new Color (0.5f, 0.5f, 0.5f);
		} 
		else if (option == OC_Option.START_TUTORIAL)
		{
			start_game_gobj.GetComponent<Renderer>().material.color = new Color (0.5f, 0.5f, 0.5f);
			start_tutorial_gobj.GetComponent<Renderer>().material.color = start_turtorial_mat.color;
		}



	}
}
