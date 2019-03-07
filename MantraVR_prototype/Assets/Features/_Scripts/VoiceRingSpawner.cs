using SoundInput;
using System.Collections;
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

		_currentPitch = LerpPitch(_data.pitchOffsetFactor, 1);
		_currentVolume = (_volume > 0) ? LerpVolume(_data.volumeOffsetFactor, 1) : 0.0f;

		transform.position = new Vector3(transform.position.x, _startPosition.y + _currentPitch, transform.position.z);
		_light.transform.position = new Vector3(_light.transform.position.x, _startPosition.y + _currentPitch, _light.transform.position.z);

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
		voiceRing.GetComponent<MeshRenderer>().material.color = partColor;

		// Change mesh
		Mesh mesh = voiceRing.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

		float perlinNoiseStart = Mathf.PerlinNoise(1f, 0f);
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] += new Vector3(
				vertices[i].x,
				vertices[i].y,
				vertices[i].z + (Mathf.PerlinNoise(Random.Range(0f, 1000f) + i * 0.001f, 0f) - perlinNoiseStart) * _currentPitch / 400f + _currentPitch / 1000f
			);
		}

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