Shader "Hidden/CC_ChannelMixer"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_red ("Red Channel", Vector) = (1, 0, 0, 1)
		_green ("Green Channel", Vector) = (0, 1, 0, 1)
		_blue ("Blue Channel", Vector) = (0, 0, 1, 1)
		_constant ("Constant", Vector) = (0, 0, 0, 1)
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
				half4 _red;
				half4 _green;
				half4 _blue;
				half4 _constant;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = tex2D(_MainTex, i.uv);

					half3 result = (color.rrr * _red)
								 + (color.ggg * _green)
								 + (color.bbb * _blue)
								 + _constant;
					
					return fixed4(result, color.a);
				}

			ENDCG
		}
	}

	FallBack off
}
