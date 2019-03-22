using UnityEngine;

[CreateAssetMenu(menuName = "MantraVR/VoiceRingDataSO")]
public class VoiceRingData : ScriptableObject
{
	public GameObjectVariable prefabVar;

	[Header("VoiceRing Properties")]
	public FloatVariable moveSpeedVar;
	public FloatVariable maxHeightVar;
	public FloatVariable alphaVar;
	public FloatVariable fadeAfterSecondsVar;
	public FloatVariable fadeSpeedVar;

	[Header("VoiceRing Noise")]
	public FloatVariable noiseHeightVar;
	public FloatVariable noiseRandomizerVar;
	public FloatVariable noiseSmoothnessVar;
	public FloatVariable noiseWidthVar;
	public FloatVariable noiseWidthExponentVar;
	public FloatVariable verticesRandomizerVar;

	[Header("Volume")]
	public FloatVariable volumeSpeedVar;
	public FloatVariable volumeOffsetFactorVar;
	public FloatVariable minVolumeVar;

	[Header("Pitch")]
	public FloatVariable pitchSpeedVar;
	public FloatVariable pitchOffsetFactorVar;

	[Header("Spawning")]
	public FloatVariable secondsBetweenSpawningVar;

	[Header("Colors from low to high")]
	public ColorListVariable pitchColorsVar;

	[Header("Light")]
	public FloatVariable lightColorSpeedVar;
}
