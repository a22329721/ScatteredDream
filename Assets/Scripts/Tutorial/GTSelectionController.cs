using UnityEngine;
using System.Collections;

public class GTSelectionController : MonoBehaviour {
	
	public GameObject nextpage;
	public GameObject previouspage;
	public TextMesh text;

	public AnimationCurve curve;
	private float original_emission;

	private int page = 1;
	public int pageMax = 6;
	private float pageTimer;

	private SceneController sceneController;

	//二回押すことを防止する
	private bool isAxisUse = false;

	private AudioSource audioSource;


	void Start () 
	{
		original_emission = nextpage.GetComponent<Renderer> ().material.GetFloat("_Emission");
		GameObject sceneControllerObject = GameObject.FindGameObjectWithTag("SceneManager");

		if (sceneControllerObject == null)
		{
			Debug.Log("Selection Controller cannot find tag 'SceneManager'");
		}

		sceneController = sceneControllerObject.GetComponent<SceneController> ();

		audioSource = GetComponent<AudioSource> ();

		pageTimer = 0.0f;
	}
	

	void Update ()
	{
		if (!sceneController.isStart () || sceneController.isClosing() ) {
			return;
		}

		pageTimer += Time.deltaTime;

		if (Input.GetAxis("HorizontalSelect") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
		{
			//次のページへ
			if ((Input.GetAxis("HorizontalSelect") > 0.0f || Input.GetAxis("Horizontal") > 0.0f) && !isAxisUse) 
			{
				nextpage.transform.GetComponent<Renderer> ().material.SetFloat ("_Emission", curve.Evaluate (Time.time));

				if (pageTimer >= 0.5f)
				{
					if (page == pageMax) {
						sceneController.LevelNowPlus (1);
						sceneController.sceneClose ();
					}

					page = (page >= pageMax) ? pageMax : page + 1;
					text.text = page.ToString ();
					isAxisUse = true;
			
					audioSource.Play ();
					pageTimer = 0.0f;
				}
			} 
			//先のページへ
			else if ((Input.GetAxis("HorizontalSelect") < 0.0f || Input.GetAxis("Horizontal") < 0.0f) && !isAxisUse)
			{
				previouspage.transform.GetComponent<Renderer> ().material.SetFloat ("_Emission", curve.Evaluate (Time.time));

				if (pageTimer >= 0.5f)
				{
					page = (page <= 1) ? 1 : page - 1;
					text.text = page.ToString ();
					isAxisUse = true;

					audioSource.Play ();
					pageTimer = 0.0f;
				}
			}
				
		} 
		else
		{
			isAxisUse = false;
			nextpage.transform.GetComponent<Renderer> ().material.SetFloat ("_Emission", original_emission);
			previouspage.transform.GetComponent<Renderer> ().material.SetFloat ("_Emission", original_emission);
		}
	
	}

	public int GetPageNow()
	{
		return page;
	}
}
