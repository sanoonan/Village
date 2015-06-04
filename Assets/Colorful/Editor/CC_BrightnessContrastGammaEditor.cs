using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_BrightnessContrastGamma))]
public class CC_BrightnessContrastGammaEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty redCoeff;
	SerializedProperty greenCoeff;
	SerializedProperty blueCoeff;

	SerializedProperty brightness;
	SerializedProperty contrast;
	SerializedProperty gamma;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		redCoeff = srcObj.FindProperty("redCoeff");
		greenCoeff = srcObj.FindProperty("greenCoeff");
		blueCoeff = srcObj.FindProperty("blueCoeff");

		brightness = srcObj.FindProperty("brightness");
		contrast = srcObj.FindProperty("contrast");
		gamma = srcObj.FindProperty("gamma");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		GUILayout.Label("Channels", EditorStyles.boldLabel);
		EditorGUILayout.Slider(redCoeff, 0f, 1f, "Red");
		EditorGUILayout.Slider(greenCoeff, 0f, 1f, "Green");
		EditorGUILayout.Slider(blueCoeff, 0f, 1f, "Blue");

		GUILayout.Label("Brightness, Contrast, Gamma", EditorStyles.boldLabel);
		EditorGUILayout.Slider(brightness, -100f, 100f, "Brightness");
		EditorGUILayout.Slider(contrast, -100f, 100f, "Contrast");
		EditorGUILayout.Slider(gamma, 0.1f, 9.9f, "Gamma");

		srcObj.ApplyModifiedProperties();
    }
}
