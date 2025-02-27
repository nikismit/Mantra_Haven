﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundInput;
using Utility;

public class GOSoundSet : MonoBehaviour {

	private SoundInputController SIC;

	// Use this for initialization
	void Start () {
		
		SIC = this.GetComponent<SoundInputController>();

	}
	
	// Update is called once per frame
	void Update () {
		
		Vector2 touchposition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

		if(touchposition.y > 0 && OVRInput.GetDown(OVRInput.Button.One)){
			SIC.settings.minVolume -= 1.0f;
			SIC.settings.minVolume = Mathf.Clamp(SIC.settings.minVolume, -60, 0);
		}
		if(touchposition.y < 0 && OVRInput.GetDown(OVRInput.Button.One)){
			SIC.settings.minVolume += 1.0f;
			SIC.settings.minVolume = Mathf.Clamp(SIC.settings.minVolume, -60, 0);
		}

		if (Input.GetKeyDown(KeyCode.A))
			SIC.settings.minVolume++;
		if (Input.GetKeyDown(KeyCode.D))
			SIC.settings.minVolume--;

		SIC.settings.minVolume = Mathf.Clamp(SIC.settings.minVolume, -60, 0);
	}
}
