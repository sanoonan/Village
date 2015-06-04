using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_AnalogTV))]
public class CC_AnalogTVEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty phase;
	SerializedProperty grayscale;
	SerializedProperty noiseIntensity;
	SerializedProperty scanlinesIntensity;
	SerializedProperty scanlinesCount;
	SerializedProperty distortion;
	SerializedProperty cubicDistortion;
	SerializedProperty scale;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		phase = srcObj.FindProperty("phase");
		grayscale = srcObj.FindProperty("grayscale");
		noiseIntensity = srcObj.FindProperty("noiseIntensity");
		scanlinesIntensity = srcObj.FindProperty("scanlinesIntensity");
		scanlinesCount = srcObj.FindProperty("scanlinesCount");
		distortion = srcObj.FindProperty("distortion");
		cubicDistortion = srcObj.FindProperty("cubicDistortion");
		scale = srcObj.FindProperty("scale");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		phase.floatValue = EditorGUILayout.FloatField("Phase (time)", phase.floatValue);
		grayscale.boolValue = EditorGUILayout.Toggle("Convert to Grayscale", grayscale.boolValue);

		GUILayout.Label("Analog Effect", EditorStyles.boldLabel);
		EditorGUILayout.Slider(noiseIntensity, 0.0f, 1.0f, "Noise Intensity");
		EditorGUILayout.Slider(scanlinesIntensity, 0.0f, 10.0f, "Scanlines Intensity");
		EditorGUILayout.Slider(scanlinesCount, 0.0f, 4096.0f, "Scanlines Count");

		GUILayout.Label("Barrel Distortion", EditorStyles.boldLabel);
		EditorGUILayout.Slider(distortion, -2.0f, 2.0f, "Distortion");
		EditorGUILayout.Slider(cubicDistortion, -2.0f, 2.0f, "Cubic Distortion");
		EditorGUILayout.Slider(scale, 0.01f, 2.0f, "Scale (Zoom)");

		srcObj.ApplyModifiedProperties();
    }
}
