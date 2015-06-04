using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Sharpen))]
public class CC_SharpenEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty strength;
	SerializedProperty clamp;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		strength = srcObj.FindProperty("strength");
		clamp = srcObj.FindProperty("clamp");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(strength, 0f, 5f, "Strength");
		EditorGUILayout.Slider(clamp, 0f, 1f, "Clamp");

		srcObj.ApplyModifiedProperties();
    }
}
