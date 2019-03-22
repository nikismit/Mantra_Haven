using SoundInput;
using UnityEngine;

public class TerraformRingSpawner : MonoBehaviour
{
	public SoundInputController SIC;

	public int ringSpawningSpeed = 10;
	public TerraformRingData data;

	private float _speakingTimer;

	private float _pitch = 0.0f;
	private float _volume = 0.0f;
	private bool _isSpeaking = false;
	private bool _canSpawn = true;

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

		if (_isSpeaking && _canSpawn)
		{
			_speakingTimer += Time.deltaTime;
			
			if (_speakingTimer >= 1.0f / ringSpawningSpeed)
			{
				GameObject currentRing = Instantiate(data.prefabVar.value, transform.position, transform.rotation);
				TerraformRing terraformRingScript = currentRing.GetComponent<TerraformRing>();
				terraformRingScript.transform.localScale = data.startScaleVar.value;
				terraformRingScript.Setup(data);
				terraformRingScript.StartGrow();
				_speakingTimer = 0.0f;
			}
		}

		if (OVRInput.GetDown(OVRInput.Button.Two))
		{
			_canSpawn = (_canSpawn) ? false : true;
		}
	}
}