using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Blend))]
public class CC_BlendEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty amount;
	SerializedProperty texture;
	SerializedProperty mode;

	static string[] modes = { "Darken", "Multiply", "Color Burn", "Linear Burn", "Darker Color", "",
							  "Lighten", "Screen", "Color Dodge", "Linear Dodge (Add)", "Lighter Color", "",
							  "Overlay", "Soft Light", "Hard Light", "Vivid Light", "Linear Light", "Pin Light", "Hard Mix", "",
							  "Difference", "Exclusion", "Subtract", "Divide" };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		amount = srcObj.FindProperty("amount");
		texture = srcObj.FindProperty("texture");
		mode = srcObj.FindProperty("mode");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		mode.intValue = EditorGUILayout.Popup("Mode", mode.intValue, modes);
		EditorGUILayout.PropertyField(texture);
		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		srcObj.ApplyModifiedProperties();
    }
}
