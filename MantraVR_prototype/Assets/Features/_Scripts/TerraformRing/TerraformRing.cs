using UnityEngine;

public class TerraformRing : MonoBehaviour
{
	[SerializeField]
	private TerraformRingData _data;

	private bool _isGrowing = false;
	private float _growTimer = 0.0f;

	public void Setup(TerraformRingData data)
	{
		_data = data;
	}

	private void Update()
	{
		// Move
		transform.position += transform.forward * _data.moveSpeedVar.value * Time.deltaTime;

		// Grow
		if (_isGrowing)
		{
			transform.localScale = Vector3.Lerp(
				_data.startScaleVar.value,
				_data.endScaleVar.value,
				_growTimer / _data.growTimeVar.value
			);
			_growTimer += Time.deltaTime;
		}
		else if (_growTimer != 0.0f)
		{
			_growTimer = 0.0f;
		}
	}

	public void StartGrow()
	{
		_isGrowing = true;
	}
}