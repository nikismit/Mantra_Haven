using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingRing : MonoBehaviour {

	public float breathingInTime = 5.0f;
	public float breathingOutTime = 5.0f;
	public float pauseTime = 1.0f;
	public float startAfterSeconds = 4.0f;
	private bool started = false;
	private bool breathingIn = true;
	private bool pause = false;
	private float breathingTimer = 0.0f;
	public Vector3 startScale;
	public Vector3 endScale;

	// Use this for initialization
	void Start () {
		


	}
	
	// Update is called once per frame
	void Update () {
		if(started == false){
			breathingTimer += Time.deltaTime;
			if(breathingTimer >= startAfterSeconds){
				started = true;
				breathingTimer = 0.0f;
			}
		} else {
			if(pause == false){
				if(breathingIn == true){
					this.transform.localScale = Vector3.Lerp(startScale, endScale, breathingTimer/breathingInTime);
					if(breathingTimer < breathingInTime){
						breathingTimer += Time.deltaTime;
					} else {
						pause = true;
						breathingIn = false;
						breathingTimer = 0.0f;
					}
			
				}
				if(breathingIn == false){
					this.transform.localScale = Vector3.Lerp(endScale, startScale, breathingTimer/breathingInTime);
					if (breathingTimer < breathingOutTime){
						breathingTimer += Time.deltaTime;
					} else {
						pause = true;
						breathingIn = true;
						breathingTimer = 0.0f;
					}
			
				}
			} else {
				if(breathingTimer < pauseTime){
					breathingTimer += Time.deltaTime;
				} else {
					pause = false;
					breathingTimer = 0.0f;
				}
			}
		}
	}
}
