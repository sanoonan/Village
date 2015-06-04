using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_HueSaturationValue))]
public class CC_HueSaturationValueEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty hue;
	SerializedProperty saturation;
	SerializedProperty value;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		hue = srcObj.FindProperty("hue");
		saturation = srcObj.FindProperty("saturation");
		value = srcObj.FindProperty("value");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(hue, -180f, 180f, "Hue");
		EditorGUILayout.Slider(saturation, -100.0f, 100.0f, "Saturation");
		EditorGUILayout.Slider(value, -100.0f, 100.0f, "Value");

		srcObj.ApplyModifiedProperties();
    }
}
