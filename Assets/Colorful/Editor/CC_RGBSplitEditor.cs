using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_RGBSplit))]
public class CC_RGBSplitEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty amount;
	SerializedProperty angle;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		amount = srcObj.FindProperty("amount");
		angle = srcObj.FindProperty("angle");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.PropertyField(amount);
		EditorGUILayout.PropertyField(angle);

		srcObj.ApplyModifiedProperties();
    }
}
