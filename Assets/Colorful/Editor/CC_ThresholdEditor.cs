using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Threshold))]
public class CC_ThresholdEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty threshold;
	SerializedProperty useNoise;
	SerializedProperty noiseRange;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		threshold = srcObj.FindProperty("threshold");
		useNoise = srcObj.FindProperty("useNoise");
		noiseRange = srcObj.FindProperty("noiseRange");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(threshold, 1, 255, "Threshold");
		useNoise.boolValue = EditorGUILayout.Toggle("Noise", useNoise.boolValue);

		if (useNoise.boolValue)
			EditorGUILayout.Slider(noiseRange, 0, 128, "Range");

		srcObj.ApplyModifiedProperties();
    }
}
