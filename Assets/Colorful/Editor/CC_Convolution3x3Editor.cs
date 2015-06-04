using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Convolution3x3))]
public class CC_Convolution3x3Editor : Editor
{
	SerializedObject srcObj;

	SerializedProperty divisor;
	SerializedProperty amount;
	SerializedProperty kernelTop;
	SerializedProperty kernelMiddle;
	SerializedProperty kernelBottom;

	int selectedPreset = 0;
	static string[] presets = { "Default", "Sharpen", "Emboss", "Gaussian Blur", "Laplacian Edge Detection", "Prewitt Edge Detection", "Frei-Chen Edge Detection" };
	static Vector3[,] presetsData = { { new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f) },
									  { new Vector3(0f, -1f, 0f), new Vector3(-1f, 5f, -1f), new Vector3(0f, -1f, 0f) },
									  { new Vector3(-2f, -1f, 0f), new Vector3(-1f, 1f, 1f), new Vector3(0f, 1f, 2f) },
									  { new Vector3(1f, 2f, 1f), new Vector3(2f, 4f, 2f), new Vector3(1f, 2f, 1f) },
									  { new Vector3(0f, -1f, 0f), new Vector3(-1f, 4f, -1f), new Vector3(0f, -1f, 0f) },
									  { new Vector3(0f, 1f, 1f), new Vector3(-1f, 0f, 1f), new Vector3(-1f, -1f, 0f) },
									  { new Vector3(-1f, -1.4142f, -1f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1.4142f, 1f) } };
	static float[] presetsDiv = { 1.0f, 1.0f, 1.0f, 16.0f, 1.0f, 1.0f, 1.0f };

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		divisor = srcObj.FindProperty("divisor");
		amount = srcObj.FindProperty("amount");
		kernelTop = srcObj.FindProperty("kernelTop");
		kernelMiddle = srcObj.FindProperty("kernelMiddle");
		kernelBottom = srcObj.FindProperty("kernelBottom");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		GUILayout.Label("Matrix", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(divisor);

		Vector3 temp = kernelTop.vector3Value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Kernel");
		temp.x = EditorGUILayout.FloatField(temp.x);
		temp.y = EditorGUILayout.FloatField(temp.y);
		temp.z = EditorGUILayout.FloatField(temp.z);
		EditorGUILayout.EndHorizontal();
		kernelTop.vector3Value = temp;

		temp = kernelMiddle.vector3Value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		temp.x = EditorGUILayout.FloatField(temp.x);
		temp.y = EditorGUILayout.FloatField(temp.y);
		temp.z = EditorGUILayout.FloatField(temp.z);
		EditorGUILayout.EndHorizontal();
		kernelMiddle.vector3Value = temp;

		temp = kernelBottom.vector3Value;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		temp.x = EditorGUILayout.FloatField(temp.x);
		temp.y = EditorGUILayout.FloatField(temp.y);
		temp.z = EditorGUILayout.FloatField(temp.z);
		EditorGUILayout.EndHorizontal();
		kernelBottom.vector3Value = temp;

		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		GUILayout.Label("Presets", EditorStyles.boldLabel);

		GUI.changed = false;
		selectedPreset = EditorGUILayout.Popup("Preset", selectedPreset, presets);

		if (GUI.changed)
		{
			kernelTop.vector3Value = presetsData[selectedPreset, 0];
			kernelMiddle.vector3Value = presetsData[selectedPreset, 1];
			kernelBottom.vector3Value = presetsData[selectedPreset, 2];
			divisor.floatValue = presetsDiv[selectedPreset];
		}

		srcObj.ApplyModifiedProperties();
    }
}
