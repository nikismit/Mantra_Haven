using UnityEditor;
using UnityEngine;

public class VoiceRingDataWindow : EditorWindow
{
	private VoiceRingData _data;
	private Vector2 _scrollPosition;

	[MenuItem("MantraVR/Voice Ring Data")]
	public static void ShowWindow()
	{
		GetWindow(typeof(VoiceRingDataWindow));
	}

	private void OnGUI()
	{
		EditorGUILayout.Space();

		_data = (VoiceRingData)EditorGUILayout.ObjectField("Data", _data, typeof(VoiceRingData), true);

		EditorGUILayout.Space();

		Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(2));
		r.height = 2;
		r.x -= 2;
		r.width += 6;
		EditorGUI.DrawRect(r, Color.gray);

		if (!_data)
			return;

		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

		GUILayout.Label("Voice Ring Properties", EditorStyles.boldLabel);
		_data.moveSpeedVar.initialValue =				EditorGUILayout.Slider("Move Speed",				_data.moveSpeedVar.initialValue,				0f,		200f);
		_data.maxHeightVar.initialValue =				EditorGUILayout.Slider("Max Height",				_data.maxHeightVar.initialValue,				0f,		5f);
		_data.alphaVar.initialValue =					EditorGUILayout.Slider("Alpha",						_data.alphaVar.initialValue,					0f,		1f);
		_data.fadeAfterSecondsVar.initialValue =		EditorGUILayout.Slider("Fade After Seconds",		_data.fadeAfterSecondsVar.initialValue,			0f,		50f);
		_data.fadeSpeedVar.initialValue =				EditorGUILayout.Slider("Fade Speed",				_data.fadeSpeedVar.initialValue,				0.1f,	10f);

		GUILayout.Label("Voice Ring Noise", EditorStyles.boldLabel);
		_data.noiseHeightVar.initialValue =				EditorGUILayout.Slider("Noise Height",				_data.noiseHeightVar.initialValue,				0f,		1f);
		_data.noiseRandomizerVar.initialValue =			EditorGUILayout.Slider("Noise Randomizer",			_data.noiseRandomizerVar.initialValue,			0f,		2f);
		_data.noiseSmoothnessVar.initialValue =			EditorGUILayout.Slider("Noise Smoothness",			_data.noiseSmoothnessVar.initialValue,			1f,		10f);
		_data.noiseWidthVar.initialValue =				EditorGUILayout.Slider("Noise Width",				_data.noiseWidthVar.initialValue,				0f,		0.0005f);
		_data.noiseWidthExponentVar.initialValue =		EditorGUILayout.Slider("Noise Width Exponent",		_data.noiseWidthExponentVar.initialValue,		1f,		5f);
		_data.verticesRandomizerVar.initialValue =		EditorGUILayout.Slider("Vertices Randomizer",		_data.verticesRandomizerVar.initialValue,		0f,		0.003f);

		GUILayout.Label("Volume", EditorStyles.boldLabel);
		_data.volumeSpeedVar.initialValue =				EditorGUILayout.Slider("Volume Speed",				_data.volumeSpeedVar.initialValue,				0.1f,	10f);
		_data.volumeOffsetFactorVar.initialValue =		EditorGUILayout.Slider("Volume Offset Factor",		_data.volumeOffsetFactorVar.initialValue,		0.1f,	50f);
		_data.minVolumeVar.initialValue =				EditorGUILayout.Slider("Min Volume",				_data.minVolumeVar.initialValue,				0f,		100f);

		GUILayout.Label("Pitch", EditorStyles.boldLabel);
		_data.pitchSpeedVar.initialValue =				EditorGUILayout.Slider("Pitch Speed",				_data.pitchSpeedVar.initialValue,				0.1f,	10f);
		_data.pitchOffsetFactorVar.initialValue =		EditorGUILayout.Slider("Pitch Offset Factor",		_data.pitchOffsetFactorVar.initialValue,		0.1f,	50f);

		GUILayout.Label("Spawning", EditorStyles.boldLabel);
		_data.secondsBetweenSpawningVar.initialValue =	EditorGUILayout.Slider("Seconds Between Spawning",	_data.secondsBetweenSpawningVar.initialValue,	0f,		5f);

		GUILayout.Label("Colors from low to high", EditorStyles.boldLabel);
		SerializedObject pitchColorsVarSO = new SerializedObject(_data.pitchColorsVar);
		SerializedProperty pitchColorsProperty = pitchColorsVarSO.FindProperty("initialValue");
		EditorGUILayout.PropertyField(pitchColorsProperty, true);
		pitchColorsVarSO.ApplyModifiedProperties();

		GUILayout.Label("Light", EditorStyles.boldLabel);
		_data.lightColorSpeedVar.initialValue =			EditorGUILayout.Slider("Light Color Speed",			_data.lightColorSpeedVar.initialValue,			0f,		1f);

		EditorGUILayout.Space();

		EditorGUILayout.EndScrollView();
	}
}