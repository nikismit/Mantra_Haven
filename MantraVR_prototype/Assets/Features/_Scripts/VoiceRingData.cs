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
	[Range(0f, 5f)]
	public float maxHeight = 1.7f;
	[Range(0f, 1f)]
	public float alpha = 0.5f;
	[Range(0f, 50f)]
	public float fadeAfterSeconds = 3f;
	[Range(0.1f, 10f)]
	public float fadeSpeed = 5f;

	[Header("VoiceRing Noise")]
	[Range(0f, 1f)]
	public float noiseHeight = 0.2f;
	[Range(0f, 2f)]
	public float noiseRandomizer = 0f;
	[Range(1f, 10f)]
	public float noiseSmoothness = 4f;
	[Range(0f, 0.0005f)]
	public float noiseWidth = 0.01f;
	[Range(1f, 5f)]
	public float noiseWidthExponent = 3f;
	[Range(0f, 0.003f)]
	public float verticesRandomizer = 0f;

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

	[Header("Light")]
	[Range(0f, 1f)]
	public float lightColorSpeed = 0.5f;
}