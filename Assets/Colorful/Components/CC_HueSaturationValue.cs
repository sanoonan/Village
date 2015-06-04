using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Hue, Saturation, Value")]
public class CC_HueSaturationValue : CC_Base
{
	public float hue = 0.0f;
	public float saturation = 0.0f;
	public float value = 0.0f;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (hue == 0f && saturation == 0f && value == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}

		material.SetFloat("_hue", hue / 360f);
		material.SetFloat("_saturation", saturation * 0.01f);
		material.SetFloat("_value", value * 0.01f);
		Graphics.Blit(source, destination, material);
	}
}
