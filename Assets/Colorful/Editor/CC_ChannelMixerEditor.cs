using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_ChannelMixer))]
public class CC_ChannelMixerEditor : CC_BaseEditor
{
	SerializedObject srcObj;

	SerializedProperty redR;
	SerializedProperty redG;
	SerializedProperty redB;
	SerializedProperty greenR;
	SerializedProperty greenG;
	SerializedProperty greenB;
	SerializedProperty blueR;
	SerializedProperty blueG;
	SerializedProperty blueB;
	SerializedProperty constantR;
	SerializedProperty constantG;
	SerializedProperty constantB;

	int currentTab = 0;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		redR = srcObj.FindProperty("redR");
		redG = srcObj.FindProperty("redG");
		redB = srcObj.FindProperty("redB");
		greenR = srcObj.FindProperty("greenR");
		greenG = srcObj.FindProperty("greenG");
		greenB = srcObj.FindProperty("greenB");
		blueR = srcObj.FindProperty("blueR");
		blueG = srcObj.FindProperty("blueG");
		blueB = srcObj.FindProperty("blueB");
		constantR = srcObj.FindProperty("constantR");
		constantG = srcObj.FindProperty("constantG");
		constantB = srcObj.FindProperty("constantB");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		srcObj.Update();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Red", (currentTab == 0) ? tabLeftOn : tabLeft)) currentTab = 0;
		if (GUILayout.Button("Green", (currentTab == 1) ? tabMiddleOn : tabMiddle)) currentTab = 1;
		if (GUILayout.Button("Blue", (currentTab == 2) ? tabRightOn : tabRight)) currentTab = 2;
		
		GUILayout.EndHorizontal();

		if (currentTab == 0) ChannelUI(redR, redG, redB, constantR);
		if (currentTab == 1) ChannelUI(greenR, greenG, greenB, constantG);
		if (currentTab == 2) ChannelUI(blueR, blueG, blueB, constantB);

		srcObj.ApplyModifiedProperties();
	}

	void ChannelUI(SerializedProperty red, SerializedProperty green, SerializedProperty blue, SerializedProperty constant)
	{
		EditorGUILayout.Slider(red, -200, 200, "Red");
		EditorGUILayout.Slider(green, -200, 200, "Green");
		EditorGUILayout.Slider(blue, -200, 200, "Blue");
		GUILayout.Space(12);
		EditorGUILayout.Slider(constant, -200, 200, "Constant");
	}
}
