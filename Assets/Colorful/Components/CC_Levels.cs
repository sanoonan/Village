using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Levels")]
public class CC_Levels : CC_Base
{
	public int mode = 0;

	public float inputMinL = 0;
	public float inputMaxL = 255;
	public float inputGammaL = 1;
	public float inputMinR = 0;
	public float inputMaxR = 255;
	public float inputGammaR = 1;
	public float inputMinG = 0;
	public float inputMaxG = 255;
	public float inputGammaG = 1;
	public float inputMinB = 0;
	public float inputMaxB = 255;
	public float inputGammaB = 1;

	public float outputMinL = 0;
	public float outputMaxL = 255;
	public float outputMinR = 0;
	public float outputMaxR = 255;
	public float outputMinG = 0;
	public float outputMaxG = 255;
	public float outputMinB = 0;
	public float outputMaxB = 255;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (mode == 0)
		{
			material.SetVector("_inputMin", new Vector4(inputMinL / 255f, inputMinL / 255f, inputMinL / 255f, 1.0f));
			material.SetVector("_inputMax", new Vector4(inputMaxL / 255f, inputMaxL / 255f, inputMaxL / 255f, 1.0f));
			material.SetVector("_inputGamma", new Vector4(inputGammaL, inputGammaL, inputGammaL, 1.0f));
			material.SetVector("_outputMin", new Vector4(outputMinL / 255f, outputMinL / 255f, outputMinL / 255f, 1.0f));
			material.SetVector("_outputMax", new Vector4(outputMaxL / 255f, outputMaxL / 255f, outputMaxL / 255f, 1.0f));
		}
		else
		{
			material.SetVector("_inputMin", new Vector4(inputMinR / 255f, inputMinG / 255f, inputMinB / 255f, 1.0f));
			material.SetVector("_inputMax", new Vector4(inputMaxR / 255f, inputMaxG / 255f, inputMaxB / 255f, 1.0f));
			material.SetVector("_inputGamma", new Vector4(inputGammaR, inputGammaG, inputGammaB, 1.0f));
			material.SetVector("_outputMin", new Vector4(outputMinR / 255f, outputMinG / 255f, outputMinB / 255f, 1.0f));
			material.SetVector("_outputMax", new Vector4(outputMaxR / 255f, outputMaxG / 255f, outputMaxB / 255f, 1.0f));
		}

		Graphics.Blit(source, destination, material);
	}
}
