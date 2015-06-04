Shader "Hidden/CC_Wiggle"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_timer ("Timer", Float) = 0.0
		_scale ("Scale", Float) = 12.0
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

				sampler2D _MainTex;
				half _timer;
				half _scale;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 t = i.uv;
					t.x += sin(_timer + t.x * _scale) * 0.01;
					t.y += cos(_timer + t.y * _scale) * 0.01 - 0.01;
					return tex2D(_MainTex, t);
				}

			ENDCG
		}
	}

	FallBack off
}
