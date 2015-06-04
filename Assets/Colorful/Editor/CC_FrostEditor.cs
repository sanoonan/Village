using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Frost))]
public class CC_FrostEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty scale;
	SerializedProperty sharpness;
	SerializedProperty darkness;
	SerializedProperty enableVignette;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		scale = srcObj.FindProperty("scale");
		sharpness = srcObj.FindProperty("sharpness");
		darkness = srcObj.FindProperty("darkness");
		enableVignette = srcObj.FindProperty("enableVignette");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(scale, 0.0f, 16.0f, "Scale");

		enableVignette.boolValue = EditorGUILayout.Toggle("Vignette", enableVignette.boolValue);

		if (enableVignette.boolValue)
		{
			EditorGUILayout.Slider(sharpness, 0.0f, 100.0f, "Sharpness");
			EditorGUILayout.Slider(darkness, 0.0f, 100.0f, "Darkness");
		}

		srcObj.ApplyModifiedProperties();
    }
}
