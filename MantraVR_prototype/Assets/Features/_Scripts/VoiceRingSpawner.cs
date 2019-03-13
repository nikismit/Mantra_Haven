using SoundInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceRingSpawner : MonoBehaviour
{
	[SerializeField]
	private SoundInputController SIC;

	[SerializeField]
	private VoiceRingData _data;

	[SerializeField]
	private Transform _light;
	
	private Vector3 _startPosition;

	private float _pitch = 0.0f;
	private float _currentPitch = 0.0f;
	private float _volume = 0.0f;
	private float _currentVolume = 0.0f;

	private bool _canSpawn = true;

	public VoiceRingData Data
	{
		get
		{
			return _data;
		}
	}

	public float Pitch
	{
		get
		{
			return _currentPitch;
		}
	}
	public float CurrentVolume
	{
		get
		{
			return _currentVolume;
		}
	}

	private void Start()
	{
		_startPosition = transform.position;
	}

	private void Update()
	{
		_pitch = SIC.inputData.relativeFrequency;
		_volume = SIC.inputData.relativeAmplitude;

		_currentPitch = Mathf.Clamp(LerpPitch(_data.pitchOffsetFactor, 1), 0f, _data.maxHeight);
		_currentVolume = (_volume > 0) ? LerpVolume(_data.volumeOffsetFactor, 1) : 0.0f;

		transform.position = new Vector3(transform.position.x, _startPosition.y + _currentPitch, transform.position.z);

		// Light
		Light light = _light.GetComponent<Light>();
		_light.transform.position = new Vector3(_light.transform.position.x, _startPosition.y + _currentPitch, _light.transform.position.z);
		light.intensity = Mathf.Lerp(light.intensity, _currentVolume + 1, Time.deltaTime * _data.lightColorSpeed);

		// Spawn VoiceRing
		if (_volume >= _data.minimumVolume && _canSpawn || _data.minimumVolume == 0)
		{
			_canSpawn = false;
			Spawn();
			StartCoroutine(WaitBeforeSpawn(_data.secondsBetweenSpawning));
		}
	}

	private GameObject Spawn()
	{
		GameObject voiceRing = Instantiate(_data.prefab);
		voiceRing.transform.position = transform.position;

		voiceRing.GetComponent<VoiceRing>().Setup(_data, SIC);

		Color partColor = Color.white;
		//set color of particle based on pitch
		float scaledTime = _currentPitch * (float)(_data.pitchColors.Length - 1);
		int oldColorIndex = (int)(scaledTime);
		Color oldColor = (oldColorIndex <= _data.pitchColors.Length - 1) ? _data.pitchColors[oldColorIndex] : _data.pitchColors[_data.pitchColors.Length - 1];
		int newColorIndex = (int)(scaledTime + 1f);
		Color newColor = (newColorIndex <= _data.pitchColors.Length - 1) ? _data.pitchColors[newColorIndex] : _data.pitchColors[_data.pitchColors.Length - 1];
		float newT = scaledTime - Mathf.Round(scaledTime);
		partColor = Color.Lerp(oldColor, newColor, newT);
		partColor.a = _data.alpha;
		voiceRing.GetComponent<MeshRenderer>().material.color = partColor;

		// Change mesh using Perlin Noise
		Mesh mesh = voiceRing.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		List<int> outerVertices = new List<int> { 0, 3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126 };
		List<int> innerVertices = new List<int> { 1, 2, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37, 39, 41, 43, 45, 47, 49, 51, 53, 55, 57, 59, 61, 63, 65, 67, 69, 71, 73, 75, 77, 79, 81, 83, 85, 87, 89, 91, 93, 95, 97, 99, 101, 103, 105, 107, 109, 111, 113, 115, 117, 119, 121, 123, 125, 127 };

		float perlinNoiseStart = Mathf.PerlinNoise(1f, 0f);
		float random = Random.Range(1f, 1f + _data.noiseRandomizer);
		for (int i = 0; i < outerVertices.Count; i++)
		{
			Vector3 outerPosition = Vector3.zero;
			Vector3 centerPosition = Vector3.zero;
			Vector3 innerPosition = Vector3.zero;

			float noise = Mathf.Abs((Mathf.PerlinNoise(random + i / _data.noiseSmoothness, 0f) - perlinNoiseStart) * _currentPitch * _data.noiseHeight);

			// Center Position
			centerPosition = vertices[outerVertices[i]];
			centerPosition.z += _currentPitch / 1000f;

			// Outer Position
			outerPosition = centerPosition;

			outerPosition.x += Random.Range(-_data.verticesRandomizer, _data.verticesRandomizer);
			outerPosition.y += Random.Range(-_data.verticesRandomizer, _data.verticesRandomizer);
			outerPosition.z = (_currentVolume > 0) ? noise + Mathf.Pow(_currentVolume, _data.noiseWidthExponent) * _data.noiseWidth : 0;

			// Inner Position
			innerPosition = centerPosition;

			innerPosition.x += Random.Range(-_data.verticesRandomizer, _data.verticesRandomizer);
			innerPosition.y += Random.Range(-_data.verticesRandomizer, _data.verticesRandomizer);
			innerPosition.z = (_currentVolume > 0) ? noise - Mathf.Pow(_currentVolume, _data.noiseWidthExponent) * _data.noiseWidth : 0;

			// Set vertices
			vertices[outerVertices[i]] = outerPosition;
			vertices[innerVertices[i]] = innerPosition;
		}

		// Update vertices
		mesh.vertices = vertices;
		mesh.RecalculateBounds();

		return voiceRing;
	}

	public IEnumerator WaitBeforeSpawn(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_canSpawn = true;
	}

	#region VoiceRingSpawner Lerp
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