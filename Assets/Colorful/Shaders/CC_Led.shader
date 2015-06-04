Shader "Hidden/CC_Led"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_scale ("Scale", Float) = 80.0
		_ratio ("Width / Height ratio", Float) = 1.0
		_brightness ("Brightness", Float) = 1.0
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#include "UnityCG.cginc"
				#include "Colorful.cginc"

				sampler2D _MainTex;
				half _scale;
				half _ratio;
				half _brightness;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = pixelate(_MainTex, i.uv, _scale, _ratio) * _brightness;
					half2 coord = i.uv * half2(_scale, _scale / _ratio);
					half mvx = abs(sin(coord.x * 3.1415)) * 1.5;
					half mvy = abs(sin(coord.y * 3.1415)) * 1.5;

					half s = mvx * mvy;
					half c = step(s, 1.0);
					color = ((1 - c) * color) + ((color * s) * c);

					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
