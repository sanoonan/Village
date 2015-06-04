using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Led))]
public class CC_LedEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty scale;
	SerializedProperty ratio;
	SerializedProperty automaticRatio;
	SerializedProperty brightness;
	SerializedProperty mode;

	static string[] modes = { "Resolution Independent", "Pixel Perfect" };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		scale = srcObj.FindProperty("scale");
		ratio = srcObj.FindProperty("ratio");
		automaticRatio = srcObj.FindProperty("automaticRatio");
		brightness = srcObj.FindProperty("brightness");
		mode = srcObj.FindProperty("mode");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		mode.intValue = EditorGUILayout.Popup("Mode", mode.intValue, modes);
		EditorGUILayout.Slider(scale, 1, 255, "Scale");
		automaticRatio.boolValue = EditorGUILayout.Toggle("Automatic Ratio", automaticRatio.boolValue);

		if (!automaticRatio.boolValue)
			EditorGUILayout.PropertyField(ratio);

		EditorGUILayout.Slider(brightness, 0.0f, 10.0f, "Brightness");

		srcObj.ApplyModifiedProperties();
    }
}
