Shader "Hidden/CC_DoubleVision"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_displace ("Displace", Vector) = (0.7, 0.0, 0.0, 0.0)
		_amount ("Amount", Range(0.0, 1.0)) = 1.0
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
				fixed2 _displace;
				half _amount;

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 c = tex2D(_MainTex, i.uv);
					fixed4 n = c.rgba;

					n += tex2D(_MainTex, i.uv + half2(_displace.x * 8, _displace.y * 8)) * 0.5;
					n += tex2D(_MainTex, i.uv + half2(_displace.x * 16, _displace.y * 16)) * 0.3;
					n += tex2D(_MainTex, i.uv + half2(_displace.x * 24, _displace.y * 24)) * 0.2;

					n *= 0.5;

					return lerp(c, n, _amount);
				}

			ENDCG
		}
	}

	FallBack off
}
