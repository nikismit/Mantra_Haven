﻿using SoundInput;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VolumeUIController : MonoBehaviour
{
	public SoundInputController SIC;
	public GameObject volumeUI;

	public float volumeUISecondsActive = 3;

	private void Update()
	{
		Vector2 touchposition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		bool doUpdateUI = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);

		if (
			touchposition.y > 0 && OVRInput.GetDown(OVRInput.Button.One) ||
			touchposition.y < 0 && OVRInput.GetDown(OVRInput.Button.One) || doUpdateUI
		)
		{
			StopAllCoroutines();
			volumeUI.SetActive(true);
			volumeUI.GetComponentInChildren<Slider>().value = Mathf.Abs(SIC.settings.minVolume);
			StartCoroutine(WaitingBeforeTurningOff(volumeUISecondsActive));
		}
	}

	private void TurnVolumeUIOff()
	{
		volumeUI.SetActive(false);
	}

	private IEnumerator WaitingBeforeTurningOff(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		TurnVolumeUIOff();
	}
}