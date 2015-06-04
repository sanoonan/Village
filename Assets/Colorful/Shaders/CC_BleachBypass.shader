Shader "Hidden/CC_BleachBypass"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
				#include "Colorful.cginc"

				sampler2D _MainTex;
				fixed _amount;

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 color = tex2D(_MainTex, i.uv);

					fixed lum = luminance(color.rgb);
					fixed3 blend = fixed3(lum, lum, lum);
					fixed L = min(1.0, max(0.0, 10.0 * (lum - 0.45)));
					fixed3 nc = lerp(2.0 * color.rgb * blend,
									 1.0 - 2.0 * (1.0 - blend) * (1.0 - color.rgb),
									 L);

					return lerp(color, fixed4(nc, color.a), _amount);
				}

			ENDCG
		}
	}

	FallBack off
}
