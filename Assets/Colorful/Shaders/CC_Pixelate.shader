Shader "Hidden/CC_Pixelate"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_scale ("Scale", Float) = 80.0
		_ratio ("Width / Height ratio", Float) = 1.0
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

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 color = pixelate(_MainTex, i.uv, _scale, _ratio);
					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
