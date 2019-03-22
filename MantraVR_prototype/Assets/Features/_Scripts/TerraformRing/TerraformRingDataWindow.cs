using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class TerraformRingDataWindow : EditorWindow
{
	private TerraformRingData _data;
	private Vector2 _scrollPosition;

	[MenuItem("MantraVR/Terraform Ring Data")]
	public static void ShowWindow()
	{
		GetWindow(typeof(TerraformRingDataWindow));
	}

	private void OnGUI()
	{
		EditorGUILayout.Space();

		_data = (TerraformRingData)EditorGUILayout.ObjectField("Data", _data, typeof(TerraformRingData), true);

		EditorGUILayout.Space();

		Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(2));
		r.height = 2;
		r.x -= 2;
		r.width += 6;
		EditorGUI.DrawRect(r, Color.gray);

		if (!_data)
			return;

		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

		GUILayout.Label("Terraform Ring Properties", EditorStyles.boldLabel);
		_data.moveSpeedVar.initialValue =				EditorGUILayout.Slider("Move Speed",				_data.moveSpeedVar.initialValue,				0f,		200f);
		_data.startScaleVar.initialValue =				EditorGUILayout.Vector3Field("Start Scale",			_data.startScaleVar.initialValue);
		_data.endScaleVar.initialValue =				EditorGUILayout.Vector3Field("End Scale",			_data.endScaleVar.initialValue);

		SerializedObject growTimeVarSO = new SerializedObject(_data.growTimeVar);
		SerializedProperty growTimeProperty = growTimeVarSO.FindProperty("initialValue");
		EditorGUILayout.PropertyField(growTimeProperty, true);
		growTimeVarSO.ApplyModifiedProperties();

		EditorGUILayout.Space();

		EditorGUILayout.EndScrollView();
	}
}
#endif