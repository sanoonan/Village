using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Halftone))]
public class CC_HalftoneEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty density;
	SerializedProperty antialiasing;
	SerializedProperty showOriginal;
	SerializedProperty mode;

	static string[] modes = { "Black and White", "CMYK" };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		density = srcObj.FindProperty("density");
		antialiasing = srcObj.FindProperty("antialiasing");
		showOriginal = srcObj.FindProperty("showOriginal");
		mode = srcObj.FindProperty("mode");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		mode.intValue = EditorGUILayout.Popup("Mode", mode.intValue, modes);

		if (mode.intValue == 0)
			EditorGUILayout.PropertyField(showOriginal);

		EditorGUILayout.Slider(density, 0.0f, 512.0f, "Density");
		EditorGUILayout.PropertyField(antialiasing);

		srcObj.ApplyModifiedProperties();
	}
}
