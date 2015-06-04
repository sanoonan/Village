using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Sharpen")]
public class CC_Sharpen : CC_Base
{
	public float strength = 0.6f;
	public float clamp = 0.05f;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (strength == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}

		material.SetFloat("_px", 1.0f / (float)Screen.width);
		material.SetFloat("_py", 1.0f / (float)Screen.height);
		material.SetFloat("_strength", strength);
		material.SetFloat("_clamp", clamp);
		Graphics.Blit(source, destination, material);
	}
}
