using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Posterize")]
public class CC_Posterize : CC_Base
{
	public int levels = 4;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_levels", (float)levels);
		Graphics.Blit(source, destination, material);
	}
}
