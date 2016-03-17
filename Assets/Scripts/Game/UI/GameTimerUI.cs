using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTimerUI : MonoBehaviour {

	public SceneController sceneController;
	private GameTimer gameTimer;

	private Text text;

	void Start () 
	{
		gameTimer = sceneController.GetComponent<GameTimer> ();
		text = GetComponent<Text> ();
	}
	

	void FixedUpdate () 
	{
		if (Time.timeScale == 0.0f)
			return;

		if (gameTimer.GetTimeLimit () <= 30.0f) 
		{
			text.color = new Color (1.0f, 0.0f, 0.0f, 1.0f);
		} 
		else
		{
			text.color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		}

		if (gameTimer.GetTimeLimit () >= 0.0f)
		{
			text.text = (int)gameTimer.GetTimeLimit () + " sec";
		}
		else
		{
			text.text = "0 sec";
		}
	}
}
