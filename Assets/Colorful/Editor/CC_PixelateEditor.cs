using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Pixelate))]
public class CC_PixelateEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty scale;
	SerializedProperty ratio;
	SerializedProperty automaticRatio;
	SerializedProperty mode;

	static string[] modes = { "Resolution Independent", "Pixel Perfect" };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		scale = srcObj.FindProperty("scale");
		ratio = srcObj.FindProperty("ratio");
		automaticRatio = srcObj.FindProperty("automaticRatio");
		mode = srcObj.FindProperty("mode");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		mode.intValue = EditorGUILayout.Popup("Mode", mode.intValue, modes);
		EditorGUILayout.Slider(scale, 1, 1024, "Scale");
		automaticRatio.boolValue = EditorGUILayout.Toggle("Automatic Ratio", automaticRatio.boolValue);

		if (!automaticRatio.boolValue)
			EditorGUILayout.PropertyField(ratio);

		srcObj.ApplyModifiedProperties();
    }
}
