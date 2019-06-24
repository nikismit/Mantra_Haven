using SoundInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public SoundInputController SIC;
	public VoiceRingSpawner voiceRingSpawner;
	public float pitchSpeed = 3f;
	public float volumeSpeed = 3f;
	public float mantraColorSpeed = 3f;

	private float _pitch = 0.0f;
	private float _currentPitch = 0.0f;
	private float _volume = 0.0f;
	private float _currentVolume = 0.0f;

	private Color _currentMantraColor;

	private void Update()
	{
		_pitch = SIC.inputData.relativeFrequency;
		_volume = SIC.inputData.amp01;

		float normal = Mathf.InverseLerp(0, 1, _pitch);
		_pitch = Mathf.Lerp(-0.25f, 0.02f, normal);

		_currentPitch = Mathf.SmoothStep(_currentPitch, _pitch, Time.deltaTime * pitchSpeed);
		_currentVolume = (_volume > 0) ? Mathf.SmoothStep(_currentVolume, _volume, Time.deltaTime * volumeSpeed) : 0;
		_currentMantraColor = Color.Lerp(_currentMantraColor, voiceRingSpawner.CurrentMantraColor, Time.deltaTime * mantraColorSpeed);

		Shader.SetGlobalFloat("Pitch", _currentPitch);
		Shader.SetGlobalFloat("Volume", _currentVolume);
		Shader.SetGlobalColor("GlowEndColor", _currentMantraColor);
	}
}