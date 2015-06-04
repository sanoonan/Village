using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_GradientRamp))]
public class CC_GradientRampEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty rampTexture;
	SerializedProperty amount;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		rampTexture = srcObj.FindProperty("rampTexture");
		amount = srcObj.FindProperty("amount");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.PropertyField(rampTexture);
		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		srcObj.ApplyModifiedProperties();
    }
}
