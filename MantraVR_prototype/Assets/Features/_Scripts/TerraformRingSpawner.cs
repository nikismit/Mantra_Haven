using SoundInput;
using UnityEngine;

public class TerraformRingSpawner : MonoBehaviour
{
	public SoundInputController SIC;
	public GameObject terraFormRing;

	public int ringSpawningSpeed = 10;
	public float ringMoveSpeed = 5.0f;
	public Vector3 ringStartScale;
	public Vector3 ringEndScale;
	public TimeData ringGrowTime = new TimeData() { Seconds = 2 };

	private float _speakingTimer;

	private float _pitch = 0.0f;
	private float _volume = 0.0f;
	private bool _isSpeaking = false;

	private void Update()
	{
		_pitch = SIC.inputData.relativeFrequency;
		_volume = SIC.inputData.amp01;

		if (_volume > 0 && !_isSpeaking)
		{
			_isSpeaking = true;
		}
		if (_volume <= 0 && _isSpeaking)
		{
			_isSpeaking = false;
			SIC.NullifyClipData();
		}

		if (_isSpeaking)
		{
			_speakingTimer += Time.deltaTime;
			
			if (_speakingTimer >= 1.0f / ringSpawningSpeed)
			{
				GameObject currentRing = Instantiate(terraFormRing, transform.position, transform.rotation);
				TerraformRing terraFormRingScript = currentRing.GetComponent<TerraformRing>();
				terraFormRingScript.transform.localScale = ringStartScale;
				terraFormRingScript.moveSpeed = ringMoveSpeed;
				terraFormRingScript.growTime = ringGrowTime;
				terraFormRingScript.startScale = ringStartScale;
				terraFormRingScript.endScale = ringEndScale;
				terraFormRingScript.StartGrow();
				_speakingTimer = 0.0f;
			}
		}
	}
}