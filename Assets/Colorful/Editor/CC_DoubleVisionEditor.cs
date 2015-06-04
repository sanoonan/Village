using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_DoubleVision))]
public class CC_DoubleVisionEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty displace;
	SerializedProperty amount;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		displace = srcObj.FindProperty("displace");
		amount = srcObj.FindProperty("amount");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		displace.vector2Value = EditorGUILayout.Vector2Field("Displace", displace.vector2Value);
		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		srcObj.ApplyModifiedProperties();
    }
}
