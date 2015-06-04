Shader "Hidden/CC_RadialBlur"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_amount ("Amount", Range(0.0, 1.0)) = 0.1
		_center ("Center Point", Vector) = (0.5, 0.5, 0.0, 0.0)
	}

	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half amount;
		half2 center;

		fixed4 frag_low(v2f_img i):COLOR
		{
			half2 coord = i.uv - center;
			half4 color = half4(0.0, 0.0, 0.0, 0.0);
			half scale;

			for (int i = 0; i < 4; i++)
			{
				scale = 1.0 + amount * (i / 3.0);
				color += tex2D(_MainTex, coord * scale + center);
			}

			color *= 0.25;
			return color;
		}

		fixed4 frag_med(v2f_img i):COLOR
		{
			half2 coord = i.uv - center;
			half4 color = half4(0.0, 0.0, 0.0, 0.0);
			half scale;

			for (int i = 0; i < 8; i++)
			{
				scale = 1.0 + amount * (i / 7.0);
				color += tex2D(_MainTex, coord * scale + center);
			}

			color *= 0.125;
			return color;
		}

		fixed4 frag_high(v2f_img i):COLOR
		{
			half2 coord = i.uv - center;
			half4 color = half4(0.0, 0.0, 0.0, 0.0);
			half scale;

			for (int i = 0; i < 14; i++)
			{
				scale = 1.0 + amount * (i / 13.0);
				color += tex2D(_MainTex, coord * scale + center);
			}

			color /= 14;
			return color;
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
				#pragma fragment frag_low
				#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_med
				#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_high
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma target 3.0

			ENDCG
		}
	}

	FallBack off
}
