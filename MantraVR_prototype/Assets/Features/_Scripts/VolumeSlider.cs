using SoundInput;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
	public SoundInputController SIC;

	private Slider _slider;

	private void Awake()
	{
		_slider = GetComponent<Slider>();
	}

	private void Update()
	{
		_slider.value = Mathf.Abs(SIC.settings.minVolume);
	}
}