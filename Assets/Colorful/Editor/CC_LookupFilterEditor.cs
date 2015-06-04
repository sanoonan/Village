using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_LookupFilter))]
public class CC_LookupFilterEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty lookupTexture;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		lookupTexture = srcObj.FindProperty("lookupTexture");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.PropertyField(lookupTexture);
		EditorGUILayout.LabelField("Read the documentation for more information about this effect.", EditorStyles.boldLabel);

		srcObj.ApplyModifiedProperties();
    }
}
