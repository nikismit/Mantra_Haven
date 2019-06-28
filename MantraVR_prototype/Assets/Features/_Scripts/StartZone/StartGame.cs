using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TNet;

using Opera.Audio;

public class StartGame : MonoBehaviour 
{
	[SerializeField]
	private Camera leftCamera;
	[SerializeField]
	private Camera rightCamera;
	[SerializeField]
	private Camera mainCamera;
	[SerializeField]
	private GameObject sceneRoot;
	//[SerializeField]
	//private AudioPref audio;
	[SerializeField]
	private AudioText text;

	private AsyncOperation async;
	private Coroutine routine;
	private float pressed;
	private bool messageShown;

	void Start()
	{
		TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
		UnityEngine.Random.InitState((int)t.TotalSeconds);
		
		//UnityEngine.VR.InputTracking.Recenter();
		
		StartCoroutine(LoadGame());
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			//int next = (int)AudioPref.preferedLanguage + 1;
			//if (next > 2) { next = 0; }

			//AudioPref.preferedLanguage = (Language)(next);
			//text.Change(AudioPref.preferedLanguage.ToString());
		}
        /*else if (Input.GetMouseButton(0))
		{
			pressed += Time.deltaTime;
			if (pressed > 2f && messageShown == false)
			{
				text.Change("starting in 2..");
				messageShown = true;
			}

			if (pressed > 4) ChangeScene();
		}
		else
			pressed = 0;*/
	}

	[RFC]
	public void ChangeScene(float time)
	{		
		//Handheld.Vibrate();
		NetworkInfo.timing = time;		
		ChangeScene();
	}

	//[RFC]
	public void ChangeScene()
	{		
		//Handheld.Vibrate();
		sceneRoot.SetActive(false);
		async.allowSceneActivation = true;
	}

	IEnumerator LoadGame()
	{
		yield return new WaitForSeconds(0.5f);
		async = SceneManager.LoadSceneAsync("VoiceRingTransparent");
		async.allowSceneActivation = false;
		
		yield return new WaitWhile(()=> async.progress < 0.9f );
		//text.Change("loaded");		
		yield return new WaitWhile( ()=> async.allowSceneActivation == false );

		yield return async;		
	}
}
