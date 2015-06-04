using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_FastVignette))]
public class CC_FastVignetteEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty sharpness;
	SerializedProperty darkness;
	SerializedProperty desaturate;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		sharpness = srcObj.FindProperty("sharpness");
		darkness = srcObj.FindProperty("darkness");
		desaturate = srcObj.FindProperty("desaturate");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(sharpness, -100.0f, 100.0f, "Sharpness");
		EditorGUILayout.Slider(darkness, 0.0f, 100.0f, "Darkness");
		desaturate.boolValue = EditorGUILayout.Toggle("Desaturate", desaturate.boolValue);

		srcObj.ApplyModifiedProperties();
    }
}
