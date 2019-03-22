using UnityEngine;

public class TerraformRing : MonoBehaviour
{
	public float moveSpeed;
	public Vector3 startScale;
	public Vector3 endScale;
	public TimeData growTime;

	private bool _isGrowing = false;
	private float _growTimer = 0.0f;

	private void Update()
	{
		// Move
		transform.position += transform.forward * moveSpeed * Time.deltaTime;

		// Grow
		if (_isGrowing)
		{
			transform.localScale = Vector3.Lerp(
				startScale,
				endScale,
				_growTimer / growTime
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