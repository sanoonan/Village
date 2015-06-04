using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Levels))]
public class CC_LevelsEditor : CC_BaseEditor
{
	Texture2D histogramTexture;
	bool logarithmic = false;
	int currentTab = 0;

	GUIContent histogramContent;
	GUIContent rampContent;
	static Color32 fillColor;

	int selectedPreset = 0;
	static string[] presets = { "Default", "Darker", "Increase Contrast 1", "Increase Contrast 2", "Increase Contrast 3",
								"Lighten Shadows", "Lighter", "Midtones Brighter", "Midtones Darker" };

	static float [,] presetsData = { { 0, 1, 255, 0, 255 }, { 15, 1, 255, 0, 255 }, { 10, 1, 245, 0, 255 },
									 { 20, 1, 235, 0, 255 }, { 30, 1, 225, 0, 255 }, { 0, 1.6f, 255, 0, 255 },
									 { 0, 1, 230, 0, 255 }, { 0, 1.25f, 255, 0, 255 }, { 0, 0.75f, 255, 0, 255 } };

	SerializedObject srcObj;

	SerializedProperty mode;
	SerializedProperty inputMinL;
	SerializedProperty inputMaxL;
	SerializedProperty inputGammaL;
	SerializedProperty inputMinR;
	SerializedProperty inputMaxR;
	SerializedProperty inputGammaR;
	SerializedProperty inputMinG;
	SerializedProperty inputMaxG;
	SerializedProperty inputGammaG;
	SerializedProperty inputMinB;
	SerializedProperty inputMaxB;
	SerializedProperty inputGammaB;

	SerializedProperty outputMinL;
	SerializedProperty outputMaxL;
	SerializedProperty outputMinR;
	SerializedProperty outputMaxR;
	SerializedProperty outputMinG;
	SerializedProperty outputMaxG;
	SerializedProperty outputMinB;
	SerializedProperty outputMaxB;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		mode = srcObj.FindProperty("mode");
		inputMinL = srcObj.FindProperty("inputMinL");
		inputMaxL = srcObj.FindProperty("inputMaxL");
		inputGammaL = srcObj.FindProperty("inputGammaL");
		inputMinR = srcObj.FindProperty("inputMinR");
		inputMaxR = srcObj.FindProperty("inputMaxR");
		inputGammaR = srcObj.FindProperty("inputGammaR");
		inputMinG = srcObj.FindProperty("inputMinG");
		inputMaxG = srcObj.FindProperty("inputMaxG");
		inputGammaG = srcObj.FindProperty("inputGammaG");
		inputMinB = srcObj.FindProperty("inputMinB");
		inputMaxB = srcObj.FindProperty("inputMaxB");
		inputGammaB = srcObj.FindProperty("inputGammaB");

		outputMinL = srcObj.FindProperty("outputMinL");
		outputMaxL = srcObj.FindProperty("outputMaxL");
		outputMinR = srcObj.FindProperty("outputMinR");
		outputMaxR = srcObj.FindProperty("outputMaxR");
		outputMinG = srcObj.FindProperty("outputMinG");
		outputMaxG = srcObj.FindProperty("outputMaxG");
		outputMinB = srcObj.FindProperty("outputMinB");
		outputMaxB = srcObj.FindProperty("outputMaxB");

		histogramTexture = new Texture2D(256, 128, TextureFormat.ARGB32, false);
		histogramTexture.filterMode = FilterMode.Point;

		histogramContent = new GUIContent(histogramTexture);
		rampContent = new GUIContent((Texture2D)Resources.Load("GrayscaleRamp"));

		fillColor = EditorGUIUtility.isProSkin ? new Color32(242, 242, 242, 255) : new Color32(18, 18, 18, 255);

		ComputeHistogram();
	}
	
	void OnDisable()
	{
		if (histogramTexture)
			DestroyImmediate(histogramTexture);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		srcObj.Update();

		// Mode tabs
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Global", (mode.intValue == 0) ? tabLeftOn : tabLeft)) { mode.intValue = 0; currentTab = 0; ComputeHistogram(); }
		if (GUILayout.Button("Color Channels", (mode.intValue == 1) ? tabRightOn : tabRight)) { mode.intValue = 1; currentTab = 1; ComputeHistogram(); }
		
		GUILayout.EndHorizontal();

		// Color tabs
		if (mode.intValue == 1)
		{
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Red", (currentTab == 1) ? tabLeftOn : tabLeft, GUILayout.Width(85))) { currentTab = 1; ComputeHistogram(); }
				if (GUILayout.Button("Green", (currentTab == 2) ? tabMiddleOn : tabMiddle, GUILayout.Width(85))) { currentTab = 2; ComputeHistogram(); }
				if (GUILayout.Button("Blue", (currentTab == 3) ? tabRightOn : tabRight, GUILayout.Width(85))) { currentTab = 3; ComputeHistogram(); }
				
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		// Histogram
		GUILayout.Space(12);
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField(histogramContent, GUILayout.Width(256), GUILayout.Height(128));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		// Channel UI
		if (currentTab == 0)      ColorChannelUI(inputMinL, inputGammaL, inputMaxL, outputMinL, outputMaxL);
		else if (currentTab == 1) ColorChannelUI(inputMinR, inputGammaR, inputMaxR, outputMinR, outputMaxR);
		else if (currentTab == 2) ColorChannelUI(inputMinG, inputGammaG, inputMaxG, outputMinG, outputMaxG);
		else if (currentTab == 3) ColorChannelUI(inputMinB, inputGammaB, inputMaxB, outputMinB, outputMaxB);

		GUILayout.Space(12);

		// Histogram control
		if (GUILayout.Button("Refresh Histogram"))
			ComputeHistogram();

		GUI.changed = false;
		logarithmic = EditorGUILayout.Toggle("Logarithmic Scale", logarithmic, GUILayout.Width(256));
		if (GUI.changed) ComputeHistogram();

		// Presets
		GUI.changed = false;
		selectedPreset = EditorGUILayout.Popup("Preset", selectedPreset, presets);

		if (GUI.changed)
		{
			mode.intValue = 0; currentTab = 0;
			inputMinL.floatValue = presetsData[selectedPreset, 0];
			inputGammaL.floatValue = presetsData[selectedPreset, 1];
			inputMaxL.floatValue = presetsData[selectedPreset, 2];
			outputMinL.floatValue = presetsData[selectedPreset, 3];
			outputMaxL.floatValue = presetsData[selectedPreset, 4];
		}

		srcObj.ApplyModifiedProperties();
    }

    void ColorChannelUI(SerializedProperty inputMinP, SerializedProperty inputGammaP, SerializedProperty inputMaxP, SerializedProperty outputMinP, SerializedProperty outputMaxP)
    {
		float inputMin = inputMinP.floatValue;
		float inputGamma = inputGammaP.floatValue;
		float inputMax = inputMaxP.floatValue;
		float outputMin = outputMinP.floatValue;
		float outputMax = outputMaxP.floatValue;

		// Input
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.MinMaxSlider(ref inputMin, ref inputMax, 0, 255, GUILayout.Width(256));
			inputMinP.floatValue = inputMin;
			inputMaxP.floatValue = inputMax;
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			inputMin = EditorGUILayout.FloatField((int)inputMin, GUILayout.Width(50));
			GUILayout.Space(50);
			inputGamma = EditorGUILayout.FloatField(inputGamma, GUILayout.Width(50));
			GUILayout.Space(50);
			inputMax = EditorGUILayout.FloatField((int)inputMax, GUILayout.Width(50));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(12);

		// Ramp
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField(rampContent, GUILayout.Width(256), GUILayout.Height(20));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		// Output
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.MinMaxSlider(ref outputMin, ref outputMax, 0, 255, GUILayout.Width(256));
			outputMinP.floatValue = outputMin;
			outputMaxP.floatValue = outputMax;
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			outputMin = EditorGUILayout.FloatField((int)outputMin, GUILayout.Width(50));
			GUILayout.Space(150);
			outputMax = EditorGUILayout.FloatField((int)outputMax, GUILayout.Width(50));
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if (inputGamma < 0.1f) inputGamma = 0.1f;
		else if (inputGamma > 9.99f) inputGamma = 9.99f;

		inputMinP.floatValue = inputMin;
		inputGammaP.floatValue = inputGamma;
		inputMaxP.floatValue = inputMax;
		outputMinP.floatValue = outputMin;
		outputMaxP.floatValue = outputMax;
	}

	void ComputeHistogram()
	{
		// Current camera
		MonoBehaviour target = (MonoBehaviour)this.target;
		CC_Levels comp = (CC_Levels)target.GetComponent<CC_Levels>();
		Camera camera = target.GetComponent<Camera>();

		// Grab the screen pixels
		comp.enabled = false;
		RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		camera.targetTexture = renderTexture;
		RenderTexture.active = renderTexture;
		camera.Render();

		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenTexture.filterMode = FilterMode.Point;
		screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
		Color32[] pixels = screenTexture.GetPixels32();
		comp.enabled = true;

		// Compute the histogram
		int[] histogram = new int[256];

		int l = pixels.Length;

		if (currentTab == 0) // Lum
			for (int i = 0; i < l; i++)
				histogram[(int)(pixels[i].r * 0.3f + pixels[i].g * 0.59f + pixels[i].b * 0.11f)]++;

		else if (currentTab == 1) // Red
			for (int i = 0; i < l; i++)
				histogram[pixels[i].r]++;

		else if (currentTab == 2) // Green
			for (int i = 0; i < l; i++)
				histogram[pixels[i].g]++;

		else if (currentTab == 3) // Blue
			for (int i = 0; i < l; i++)
				histogram[pixels[i].b]++;

		// Find max histogram value
		float max = 0;
		for (int i = 0; i < 256; i++)
			max = (max < histogram[i]) ? histogram[i] : max;

		float maxLog = Mathf.Log10(max);
		float factor = 128 / ((logarithmic) ? maxLog : max);

		// Blit the histogram texture
		pixels = histogramTexture.GetPixels32();
		int index = 0;

		if (logarithmic)
			for (int y = 0; y < 128; y++)
			for (int x = 0; x < 256; x++)
			{
				pixels[index] = (((histogram[x] == 0) ? 0 : (int)(Mathf.Log10(histogram[x]) * factor)) >= y) ? fillColor : new Color32(0, 0, 0, 0);
				index++;
			}
		else
			for (int y = 0; y < 128; y++)
			for (int x = 0; x < 256; x++)
			{
				pixels[index] = ((histogram[x] * factor) >= y) ? fillColor : new Color32(0, 0, 0, 0);
				index++;
			}

		histogramTexture.SetPixels32(pixels);
		histogramTexture.Apply();

		// Cleanup time
		RenderTexture.active = null;
		camera.targetTexture = null;

		DestroyImmediate(screenTexture);
		DestroyImmediate(renderTexture);
	}
}
