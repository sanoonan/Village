Shader "Hidden/CC_FastVignette"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_sharpness ("Sharpness", Range(-1.0, 1.0)) = 0.1
		_darkness ("Darkness", Range(0, 2.0)) = 0.3
	}

	CGINCLUDE

		#include "UnityCG.cginc"
		#include "Colorful.cginc"

		sampler2D _MainTex;
		half _sharpness;
		half _darkness;

		fixed4 frag(v2f_img i):COLOR
		{
			half4 color = tex2D(_MainTex, i.uv);

			half d = distance(i.uv, half2(0.5, 0.5));
			half multiplier = smoothstep(0.8, _sharpness * 0.799, d * (_darkness + _sharpness));
			color.rgb *= multiplier;

			return color;
		}

		fixed4 frag_desat(v2f_img i):COLOR
		{
			half4 color = tex2D(_MainTex, i.uv);

			half d = distance(i.uv, half2(0.5, 0.5));
			half multiplier = smoothstep(0.8, _sharpness * 0.799, d * (_darkness + _sharpness));
			color.rgb *= multiplier;

			half lum = luminance(color.rgb);
			half4 grayscale = half4(lum, lum, lum, color.a);

			return lerp(grayscale, color, multiplier);
		}

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_desat
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}
	}

	FallBack off
}
