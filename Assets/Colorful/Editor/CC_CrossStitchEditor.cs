using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_CrossStitch))]
public class CC_CrossStitchEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty size;
	SerializedProperty brightness;
	SerializedProperty invert;
	SerializedProperty pixelize;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		size = srcObj.FindProperty("size");
		brightness = srcObj.FindProperty("brightness");
		invert = srcObj.FindProperty("invert");
		pixelize = srcObj.FindProperty("pixelize");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.LabelField("Size works best with power of two values !", EditorStyles.boldLabel);
		EditorGUILayout.IntSlider(size, 1, 128, "Size");
		EditorGUILayout.PropertyField(brightness);
		EditorGUILayout.PropertyField(invert);
		EditorGUILayout.PropertyField(pixelize);

		srcObj.ApplyModifiedProperties();
	}
}
