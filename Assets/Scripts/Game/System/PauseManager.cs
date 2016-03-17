using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour {

	private bool isPause;
	private int option;

	//二回押すことを防止する
	private bool isAxisUse = false;
	private float pauseTimer;

	private SceneController sceneController;

	private PlayerActionManager.ACTION playerAction;
	private PlayerActionManager.STATE playerState;

	private PlayerChangeForm playerChangeForm;
	private List<Animator> ats;

	public GameObject pauseUIObject;
	private PauseUIScript pauseUIScript;

	public BGMControllerScript BGMController;

	public CameraMover cameraMover;

	private AudioSource audioSource;
	public AudioClip MenuOpen, MenuClose, MenuSelect;

	void Start () {

		//シーン状態の取得
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneControllerObject == null){
			Debug.Log ("Pause Manager cannot find tag 'SceneManager'");
		}

		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		GameObject playerObj = GameObject.FindWithTag ("Player");
		playerChangeForm = playerObj.GetComponent<PlayerChangeForm> ();

		Component[] animatorComponents = playerObj.GetComponentsInChildren(typeof(Animator));
		ats = new List<Animator>();
		for (int i = 0; i < animatorComponents.Length; i++) 
		{
			ats.Add ((Animator)animatorComponents [i]);
		}

		pauseUIScript = pauseUIObject.GetComponent<PauseUIScript> ();

		pauseUIScript.SetCamera (1, true);
		pauseUIScript.SetCamera (2, true);

		audioSource = GetComponent<AudioSource> ();

		isPause = false;
		pauseTimer = 0.0f;
	}
	

	void Update () 
	{
		//ゲームスタート前の待機状態
		if (!sceneController.isStart ()) {
			return;
		}

		if (Input.GetButtonDown ("Pause"))
		{
			if (!isPause)
			{
				isPause = true;
				option = 0;
				isAxisUse = false;

				audioSource.clip = MenuOpen;
				audioSource.Play ();

				pauseUIScript.SetUse (true);
				pauseUIScript.SetOption (option);
				Time.timeScale = 0.0f;
				pauseTimer = 0.0f;

				if (playerChangeForm.isNormal)
				{
					ats [0].updateMode = AnimatorUpdateMode.Normal;
				} 
				else 
				{
					ats [1].updateMode = AnimatorUpdateMode.Normal;
				}
			} 
			else 
			{
				isPause = false;
				pauseUIScript.SetUse (false);
				Time.timeScale = 1.0f;

				audioSource.clip = MenuClose;
				audioSource.Play ();

				if (playerChangeForm.isNormal)
				{
					ats [0].updateMode = AnimatorUpdateMode.UnscaledTime;
				} 
				else 
				{
					ats [1].updateMode = AnimatorUpdateMode.UnscaledTime;
				}
			}
		}

		if (isPause) 
		{
			pauseTimer += Time.unscaledDeltaTime;
			PauseMenuControll ();
		}
	}


	public bool isGamePaused()
	{
		return isPause;
	}

	void PauseMenuControll()
	{
		
		if (Input.GetAxisRaw("HorizontalSelect") != 0.0f || Input.GetAxisRaw("VerticalSelect") != 0.0f
			|| Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f)
		{
			
			if ((Input.GetAxisRaw("VerticalSelect") < 0.0f || Input.GetAxisRaw("Vertical") < 0.0f) && !isAxisUse) 
			{
				this.option += 1;
				if (this.option > 3)
				{
					this.option = 3;
				} 
				else
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
				}
				pauseUIScript.SetOption(this.option);
				isAxisUse = true;

			} 
			else if ((Input.GetAxisRaw("VerticalSelect") > 0.0f ||Input.GetAxisRaw("Vertical") > 0.0f) && !isAxisUse)
			{
				this.option -= 1;
				if (this.option < 0) 
				{
					this.option = 0;
				}
				else 
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
				}

				pauseUIScript.SetOption(this.option);
				isAxisUse = true;
			}

			if ((Input.GetAxisRaw("HorizontalSelect") < 0.0f || Input.GetAxisRaw("Horizontal") < 0.0f) && !isAxisUse) 
			{
				if (option == 1) 
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
					pauseUIScript.SetCamera (this.option, false);
					cameraMover.SetCemeraLRReverse (false);
				}
				else if(option == 2)
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
					pauseUIScript.SetCamera (this.option, false);
					cameraMover.SetCemeraUDReverse (false);
				}

				isAxisUse = true;

			} 
			else if ((Input.GetAxisRaw("HorizontalSelect") > 0.0f || Input.GetAxisRaw("Horizontal") > 0.0f) && !isAxisUse)
			{
				if (option == 1) 
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
					pauseUIScript.SetCamera (this.option, true);
					cameraMover.SetCemeraLRReverse (true);
				}
				else if(option == 2)
				{
					audioSource.clip = MenuSelect;
					audioSource.Play ();
					pauseUIScript.SetCamera (this.option, true);
					cameraMover.SetCemeraUDReverse (true);
				}

				isAxisUse = true;
			}

		} 
		else
		{
			if (Input.GetButtonDown ("Submit") && pauseTimer >= 0.2f) 
			{
				switch (option) 
				{
				case 0:
					{
						isPause = false;
						pauseUIScript.SetUse (false);
						Time.timeScale = 1.0f;
						Input.ResetInputAxes ();

						audioSource.clip = MenuClose;
						audioSource.Play ();

						if (playerChangeForm.isNormal)
						{
							ats [0].updateMode = AnimatorUpdateMode.UnscaledTime;
						} 
						else 
						{
							ats [1].updateMode = AnimatorUpdateMode.UnscaledTime;
						}

						break;
					}
				case 1:
					{
						audioSource.clip = MenuSelect;
						audioSource.Play ();

						pauseUIScript.SetCamera (this.option, !pauseUIScript.GetCameraReverse(this.option));
						cameraMover.ReserveCamera (1);
						break;
					}
				case 2:
					{
						audioSource.clip = MenuSelect;
						audioSource.Play ();

						pauseUIScript.SetCamera (this.option, !pauseUIScript.GetCameraReverse(this.option));
						cameraMover.ReserveCamera (2);
						break;
					}
				case 3:
					{
						BGMController.SetVolume (0.0f);
						sceneController.LevelEqual (0);
						sceneController.sceneClose ();

						audioSource.clip = MenuClose;
						audioSource.Play ();

						isPause = false;
						pauseUIScript.SetUse (false);
						Time.timeScale = 1.0f;
						Input.ResetInputAxes ();

						if (playerChangeForm.isNormal)
						{
							ats [0].updateMode = AnimatorUpdateMode.UnscaledTime;
						} 
						else 
						{
							ats [1].updateMode = AnimatorUpdateMode.UnscaledTime;
						}
						break;
					}
				default:
					break;
				}
			}

			isAxisUse = false;

		}
	}
}
