using SoundInput;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VolumeText : MonoBehaviour
{
	public SoundInputController SIC;

	private Text _text;

	private void Awake()
	{
		_text = GetComponent<Text>();
	}

	private void Update()
	{
		_text.text = Mathf.Abs(SIC.settings.minVolume).ToString();
	}
}