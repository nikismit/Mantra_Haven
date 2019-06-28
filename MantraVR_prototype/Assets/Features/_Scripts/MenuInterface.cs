using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SoundInput;

public class MenuInterface : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private Slider volumeUI;

	[SerializeField]
	private NetworkStarter networkStarter;
	
	void Start()
    {
		volumeUI = GetComponentInChildren<Slider>();
		AdjustVolume(0);
    }

    // Update is called once per frame
    void Update()
    {
		Vector2 touchposition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		bool doUpdateUI = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);

		if (
			touchposition.y > 0 && OVRInput.GetDown(OVRInput.Button.One) ||
			touchposition.y < 0 && OVRInput.GetDown(OVRInput.Button.One) || doUpdateUI
		)
		{
			AdjustVolume(0);
		}		
	}

	public void AdjustVolume(int direction)
	{

		Utility.SoundSettings settings = SoundInputController.instance.settings;
		settings.minVolume = Mathf.Clamp( settings.minVolume + direction, -60, 0);
		volumeUI.value = Mathf.Abs( settings.minVolume );
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickCount >= 2)
			networkStarter.SendBroadCast("StartSession");
	}
}
