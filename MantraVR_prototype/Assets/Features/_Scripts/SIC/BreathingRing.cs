using UnityEngine;

public class BreathingRing : MonoBehaviour
{
	private bool breathingIn = true;
	private bool pause = true;
	private float breathingTimer = 0.0f;

	private BreathingRingData _data = new BreathingRingData();

	private void Awake()
	{
		_data.startScale = new Vector3(1, 1, 1);
		_data.endScale = new Vector3(1, 1, 1);
	}

	private void Update()
	{
		if (!pause)
		{
			if (breathingIn)
			{
				// Inhaling
				TimeData breathingInTime = _data.inhaleEndTime - _data.inhaleStartTime;
				transform.localScale = Vector3.Lerp(_data.startScale, _data.endScale, breathingTimer / breathingInTime);
				if (breathingTimer < breathingInTime)
				{
					breathingTimer += Time.deltaTime;
				}
				else
				{
					pause = true;
					breathingIn = false;
					breathingTimer = 0.0f;
				}
			
			}
			else
			{
				// Exhaling
				TimeData breathingOutTime = _data.exhaleEndTime - _data.exhaleStartTime;
				transform.localScale = Vector3.Lerp(_data.endScale, _data.startScale, breathingTimer / breathingOutTime);
				if (breathingTimer < breathingOutTime)
				{
					breathingTimer += Time.deltaTime;
				}
				else
				{
					pause = true;
					breathingIn = true;
					breathingTimer = 0.0f;
				}
			}
		}
	}

	public void SetData(BreathingRingData data)
	{
		_data = data;
	}

	public void Unpause()
	{
		pause = false;
	}

	public void Unpause(bool inhale)
	{
		pause = false;
		breathingIn = inhale;
		breathingTimer = 0.0f;
	}

	public void Pause()
	{
		pause = true;
	}
}
