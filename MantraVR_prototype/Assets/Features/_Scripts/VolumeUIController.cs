using SoundInput;
using System.Collections;
using UnityEngine;

public class VolumeUIController : MonoBehaviour
{
	public SoundInputController SIC;
	public GameObject volumeUI;

	public float volumeUISecondsActive = 3;

	private void Update()
	{
		Vector2 touchposition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

		if (
			touchposition.y > 0 && OVRInput.GetDown(OVRInput.Button.One) ||
			touchposition.y < 0 && OVRInput.GetDown(OVRInput.Button.One)
		)
		{
			StopAllCoroutines();
			volumeUI.SetActive(true);
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