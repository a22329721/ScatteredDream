using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

	private bool start;
	private bool closing;
	private bool finish;
	private int levelNow;


	void Start () 
	{
		start = false;
		closing = false;
		finish = false;

		levelNow = Application.loadedLevel;
	}

	void FixedUpdate () {

		if (finish) 
		{
			if(levelNow > 3)
				levelNow = 0;

			Time.timeScale = 1.0f;
			Application.LoadLevel(levelNow);
		}
	}

	public bool isStart()
	{
		return start;
	}

	public void sceneStart()
	{
		start = true;
	}

	public bool isClosing()
	{
		return closing;
	}

	public void sceneClose()
	{
		closing = true;
	}

	public bool isFinish()
	{
		return finish;
	}

	public void sceneFinish()
	{
		finish = true;
	}

	public void LevelNowPlus(int num)
	{
		levelNow += num;
	}

	public void LevelEqual(int num)
	{
		levelNow = num;
	}

}
