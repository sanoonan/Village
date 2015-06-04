﻿using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Wiggle")]
public class CC_Wiggle : CC_Base
{
	public float timer = 0.0f;
	public float speed = 1.0f;
	public float scale = 12.0f;

	void Update()
	{
		timer += speed * Time.deltaTime;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_timer", timer);
		material.SetFloat("_scale", scale);
		Graphics.Blit(source, destination, material);
	}
}
