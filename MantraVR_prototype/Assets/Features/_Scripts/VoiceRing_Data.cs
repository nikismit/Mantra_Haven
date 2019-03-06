using UnityEngine;

[CreateAssetMenu(menuName = "MantraVR/VoiceRingData", fileName = "VoiceRingData.asset")]
public class VoiceRing_Data : ScriptableObject
{
	public GameObject particlePrefab;
	[Range(10, 500)]
	public int amountOfParticles = 100;
	public float particleMoveSpeed = 4f;
	[Range(0.1f, 5.0f)]
	public float waveAmp = 5f;
	[Range(0.01f, 1.0f)]
	public float waveWidth = 0.5f;

	[Header("Colors from low to high")]
	public Color[] pitchColors = new Color[] { Color.red, Color.green };
}