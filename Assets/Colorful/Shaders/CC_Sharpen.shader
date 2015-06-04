Shader "Hidden/CC_Sharpen"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_px ("Pixel Width", Float) = 1
		_py ("Pixel Height", Float) = 1 
		_strength ("Strength", Range(0, 5.0)) = 0.60
		_clamp ("Clamp", Range(0, 1.0)) = 0.05
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
				half _px;
				half _py;
				half _strength;
				half _clamp;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half4 color = tex2D(_MainTex, coords);

					half4 blur  = tex2D(_MainTex, coords + half2(0.5 *  _px,       -_py));
						  blur += tex2D(_MainTex, coords + half2(      -_px, 0.5 * -_py));
						  blur += tex2D(_MainTex, coords + half2(       _px, 0.5 *  _py));
						  blur += tex2D(_MainTex, coords + half2(0.5 * -_px,        _py));
					blur /= 4;

					half4 lumaStrength = half4(0.2126, 0.7152, 0.0722, 0) * _strength * 0.666;
					half4 sharp = color - blur;
					color += clamp(dot(sharp, lumaStrength), -_clamp, _clamp);

					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
