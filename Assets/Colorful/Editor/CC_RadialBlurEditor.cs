using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_RadialBlur))]
public class CC_RadialBlurEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty amount;
	SerializedProperty center;
	SerializedProperty quality;

	static string[] qualities = { "Low", "Medium", "High" };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		amount = srcObj.FindProperty("amount");
		center = srcObj.FindProperty("center");
		quality = srcObj.FindProperty("quality");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		quality.intValue = EditorGUILayout.Popup("Quality", quality.intValue, qualities);

		if (quality.intValue == 2)
			EditorGUILayout.LabelField("High quality will only be available on SM3.0 compatible GPUs !", EditorStyles.boldLabel);

		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");
		center.vector2Value = EditorGUILayout.Vector2Field("Center Point", center.vector2Value);

		srcObj.ApplyModifiedProperties();
    }
}
