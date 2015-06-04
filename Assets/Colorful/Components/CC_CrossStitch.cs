using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Cross Stitch")]
public class CC_CrossStitch : CC_Base
{
	public int size = 8;
	public float brightness = 1.5f;
	public bool invert = false;
	public bool pixelize = true;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_stitchSize", size);
		material.SetFloat("_brightness", brightness);

		int pass = invert ? 1 : 0;

		if (pixelize)
		{
			pass += 2;
			material.SetFloat("_scale", GetComponent<Camera>().pixelWidth / size);
			material.SetFloat("_ratio", GetComponent<Camera>().pixelWidth / GetComponent<Camera>().pixelHeight);
		}

		Graphics.Blit(source, destination, material, pass);
	}
}
