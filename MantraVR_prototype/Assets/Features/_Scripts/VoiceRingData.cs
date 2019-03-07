using UnityEngine;

[CreateAssetMenu(menuName = "MantraVR/VoiceRingData", fileName = "VoiceRingData.asset")]
public class VoiceRingData : ScriptableObject
{
	//[Range(10, 500)]
	//public int amountOfParticles = 100;
	//public float particleMoveSpeed = 4f;
	//[Range(0.1f, 5.0f)]
	//public float amp = 5f;
	[Header("VoiceRing")]
	public GameObject prefab;
	[Range(0f, 200f)]
	public float speed = 50f;

	[Header("Volume")]
	[Range(0.1f, 10f)]
	public float volumeSpeed = 1f;
	[Range(0.1f, 50f)]
	public float volumeOffsetFactor = 10f;
	[Range(0f, 100f)]
	public float minimumVolume = 0f;

	[Header("Pitch")]
	[Range(0.1f, 10f)]
	public float pitchSpeed = 1f;
	[Range(0.1f, 50f)]
	public float pitchOffsetFactor = 10f;

	[Header("Spawning")]
	[Range(0f, 5f)]
	public float secondsBetweenSpawning = 0.5f;

	[Header("Colors from low to high")]
	public Color[] pitchColors = new Color[] { Color.red, Color.green };
}