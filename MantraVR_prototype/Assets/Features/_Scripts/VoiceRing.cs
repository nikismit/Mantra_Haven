using SoundInput;
using System.Collections;
using UnityEngine;

public class VoiceRing : MonoBehaviour
{
	[SerializeField]
	private SoundInputController SIC;

	private VoiceRingData _data;

	private MeshRenderer _meshRenderer;

	private float _pitch = 0.0f;
	private float _currentPitch = 0.0f;
	private float _volume = 0.0f;
	private float _currentVolume = 0.0f;

	private bool _isFading = false;

	private void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		StartCoroutine(WaitForFade(_data.fadeAfterSeconds));
	}

	public void Setup(VoiceRingData voiceRingData, SoundInputController soundInputController)
	{
		_data = voiceRingData;
		SIC = soundInputController;
	}

	private void Update()
	{
		_pitch = SIC.inputData.relativeFrequency;
		_volume = SIC.inputData.relativeAmplitude;

		_currentPitch = LerpPitch(_data.pitchOffsetFactor, 1);
		_currentVolume = (_volume > 0) ? LerpVolume(_data.volumeOffsetFactor, 1) : 0.0f;

		transform.localScale = new Vector3(
			transform.localScale.x + _currentVolume + _data.speed * Time.deltaTime,
			transform.localScale.y + _currentVolume + _data.speed * Time.deltaTime,
			transform.localScale.z
		);

		// Fading
		if (_isFading)
		{
			Color newColor = _meshRenderer.material.color;
			float startAlpha = newColor.a;

			newColor.a = Mathf.Lerp(startAlpha, 0f, Time.deltaTime * _data.fadeSpeed);

			if (newColor.a <= 0.01)
				newColor.a = 0;

			_meshRenderer.material.color = newColor;

			if (newColor.a <= 0)
				Destroy(gameObject);
		}
	}

	private IEnumerator WaitForFade(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_isFading = true;
	}

	#region VoiceRing Lerp
	private float LerpVolume()
	{
		return Mathf.Lerp(_currentVolume, _volume, Time.deltaTime * _data.volumeSpeed);
	}

	private float LerpVolume(float offsetFactor)
	{
		return Mathf.Lerp(_currentVolume, _volume * offsetFactor, Time.deltaTime * _data.volumeSpeed);
	}

	private float LerpVolume(float offsetFactor, float speedFactor)
	{
		return Mathf.Lerp(_currentVolume, _volume * offsetFactor, Time.deltaTime * _data.volumeSpeed * speedFactor);
	}

	private float LerpPitch()
	{
		return Mathf.Lerp(_currentPitch, _pitch, Time.deltaTime * _data.pitchSpeed);
	}

	private float LerpPitch(float offsetFactor)
	{
		return Mathf.Lerp(_currentPitch, _pitch * offsetFactor, Time.deltaTime * _data.pitchSpeed);
	}

	private float LerpPitch(float offsetFactor, float speedFactor)
	{
		return Mathf.Lerp(_currentPitch, _pitch * offsetFactor, Time.deltaTime * _data.pitchSpeed * speedFactor);
	}
	#endregion
}