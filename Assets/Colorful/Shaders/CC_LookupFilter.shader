Shader "Hidden/CC_LookupFilter"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LookupTex ("Lookup (RGB)", 2D) = "white" {}
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
				#pragma exclude_renderers flash
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _LookupTex;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = tex2D(_MainTex, i.uv);
					half blue = color.b * 63.0;

					half2 quad1 = half2(0, 0);
					quad1.y = floor(floor(blue) / 8.0);
					quad1.x = floor(blue) - quad1.y * 8.0;

					half2 quad2 = half2(0, 0);
					quad2.y = floor(ceil(blue) / 8.0);
					quad2.x = ceil(blue) - quad2.y * 8.0;

					half c1 = 0.0009765625 + (0.123046875 * color.r);
					half c2 = 0.0009765625 + (0.123046875 * color.g);

					half2 texPos1 = half2(0, 0);
					texPos1.x = quad1.x * 0.125 + c1;
					texPos1.y = -(quad1.y * 0.125 + c2);

					half2 texPos2 = half2(0, 0);
					texPos2.x = quad2.x * 0.125 + c1;
					texPos2.y = -(quad2.y * 0.125 + c2);

					half4 newColor = lerp(tex2D(_LookupTex, texPos1),
										  tex2D(_LookupTex, texPos2),
										  frac(blue));
					newColor.a = color.a;
					return newColor;
				}

			ENDCG
		}
	}

	FallBack off
}
