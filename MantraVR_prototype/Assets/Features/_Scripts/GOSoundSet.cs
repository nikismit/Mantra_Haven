using System.Collections;
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

		print(SIC.settings.minVolume);
		if(touchposition.y > 0 && OVRInput.GetDown(OVRInput.Button.One)){
			SIC.settings.minVolume += 5.0f;
		}
		if(touchposition.y < 0 && OVRInput.GetDown(OVRInput.Button.One)){
			SIC.settings.minVolume -= 5.0f;
		}

	}
}
