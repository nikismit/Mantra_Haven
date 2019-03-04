using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitate_Script : MonoBehaviour {

	public AudioSource begeleidingAudio;
	public float levitateTime = 30.0f;
	public float levitateAmount = 5.0f;
	public float timeToReachLevitateAmount;
	private bool levitate = false;
	private float levitateSpeed = 0.0f;
	public GameObject fadePlane;
	public float fadeTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.X)){
			begeleidingAudio.time = begeleidingAudio.clip.length - (levitateTime+10.0f);
		}

		if(begeleidingAudio.time > begeleidingAudio.clip.length - levitateTime){
			levitate = true;
			print("Levitate Now!");
		}

		if(levitate == true){
			if(this.transform.localPosition.y < 10.0f){
				levitateSpeed += Time.deltaTime/timeToReachLevitateAmount;
				this.transform.Translate(0, levitateSpeed*(Time.deltaTime/levitateAmount), 0);
			} else {
				Color fadeColor = fadePlane.GetComponent<Renderer>().material.color;
				fadeColor.a += Time.deltaTime/fadeTime;
				fadePlane.GetComponent<Renderer>().material.color = fadeColor;
			}
		}

	}
}
