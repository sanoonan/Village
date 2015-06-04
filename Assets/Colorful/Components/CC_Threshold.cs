using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Threshold")]
public class CC_Threshold : CC_Base
{
	public float threshold = 128f;
	public bool useNoise = false;
	public float noiseRange = 48f;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_threshold", threshold / 255f);
		material.SetFloat("_range", noiseRange / 255f);
		Graphics.Blit(source, destination, material, useNoise ? 1 : 0);
	}
}
