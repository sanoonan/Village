using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Wiggle))]
public class CC_WiggleEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty timer;
	SerializedProperty speed;
	SerializedProperty scale;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		timer = srcObj.FindProperty("timer");
		speed = srcObj.FindProperty("speed");
		scale = srcObj.FindProperty("scale");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.PropertyField(timer);
		EditorGUILayout.PropertyField(speed);
		EditorGUILayout.PropertyField(scale);

		srcObj.ApplyModifiedProperties();
    }
}
