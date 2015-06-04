using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Grayscale))]
public class CC_GrayscaleEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty redLuminance;
	SerializedProperty greenLuminance;
	SerializedProperty blueLuminance;
	SerializedProperty amount;

	int selectedPreset = 0;
	static string[] presets = { "Default", "Unity Default", "Desaturate" };
	static float [,] presetsData = { { 0.3f, 0.59f, 0.11f }, { 0.222f, 0.707f, 0.071f }, { 0.333f, 0.334f, 0.333f } };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		redLuminance = srcObj.FindProperty("redLuminance");
		greenLuminance = srcObj.FindProperty("greenLuminance");
		blueLuminance = srcObj.FindProperty("blueLuminance");
		amount = srcObj.FindProperty("amount");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		GUILayout.Label("Luminance", EditorStyles.boldLabel);

		EditorGUILayout.Slider(redLuminance, 0.0f, 1.0f, "Red Luminance");
		EditorGUILayout.Slider(greenLuminance, 0.0f, 1.0f, "Green Luminance");
		EditorGUILayout.Slider(blueLuminance, 0.0f, 1.0f, "Blue Luminance");

		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		GUILayout.Label("Presets", EditorStyles.boldLabel);

		GUI.changed = false;
		selectedPreset = EditorGUILayout.Popup("Preset", selectedPreset, presets);

		if (GUI.changed)
		{
			redLuminance.floatValue = presetsData[selectedPreset, 0];
			greenLuminance.floatValue = presetsData[selectedPreset, 1];
			blueLuminance.floatValue = presetsData[selectedPreset, 2];
		}

		srcObj.ApplyModifiedProperties();
    }
}
