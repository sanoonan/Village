using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Vibrance))]
public class CC_VibranceEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty amount;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		amount = srcObj.FindProperty("amount");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(amount, -100, 100, "Vibrance");

		srcObj.ApplyModifiedProperties();
    }
}
