Shader "Hidden/CC_Vibrance"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_amount ("Amount", Range(-2.0, 2.0)) = 0
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
				half _amount;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = tex2D(_MainTex, i.uv);

					half cMax = max(max(color.r, color.g), color.b);
					half amount = (cMax - luminance(color.rgb)) * (-3.0 * _amount);
					color.rgb = lerp(color.rgb, half3(cMax, cMax, cMax), amount);

					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
