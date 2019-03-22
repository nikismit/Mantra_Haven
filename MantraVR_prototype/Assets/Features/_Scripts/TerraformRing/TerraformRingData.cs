using UnityEngine;

[CreateAssetMenu(menuName = "MantraVR/TerraformRingData")]
public class TerraformRingData : ScriptableObject
{
	public GameObjectVariable prefabVar;

	[Header("Properties")]
	public FloatVariable moveSpeedVar;
	public Vector3Variable startScaleVar;
	public Vector3Variable endScaleVar;
	public TimeDataVariable growTimeVar;
}