using System.Collections;
using UnityEngine;

public class TerraformRing : MonoBehaviour
{
	[SerializeField]
	private TerraformRingData _data;

	private MeshRenderer _meshRenderer;

	private bool _isGrowing = false;
	private bool _isFading = false;
	private float _growTimer = 0.0f;

	private void Awake()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		StartCoroutine(WaitForFade(5));
	}

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

		// Fading
		if (_isFading)
		{
			Color newColor = _meshRenderer.material.color;
			float startAlpha = newColor.a;

			newColor.a = Mathf.Lerp(startAlpha, 0f, Time.deltaTime * 1);

			if (newColor.a <= 0.01)
				newColor.a = 0;

			_meshRenderer.material.color = newColor;

			if (newColor.a <= 0)
				Destroy(gameObject);
		}
	}

	public void StartGrow()
	{
		_isGrowing = true;
	}

	private IEnumerator WaitForFade(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_isFading = true;
	}
}