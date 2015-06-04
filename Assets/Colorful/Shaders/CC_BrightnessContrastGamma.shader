Shader "Hidden/CC_BrightnessContrastGamma"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_rCoeff ("Luminance coeff (Red)", Range(0.0, 1.0)) = 0.5
		_gCoeff ("Luminance coeff (Green)", Range(0.0, 1.0)) = 0.5
		_bCoeff ("Luminance coeff (Blue)", Range(0.0, 1.0)) = 0.5
		_brightness ("Brightness", Range(0.0, 2.0)) = 0
		_contrast ("Contrast", Range(0.0, 2.0)) = 1
		_gamma ("Gamma", Range(0.01, 10)) = 1
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
				float _rCoeff;
				float _gCoeff;
				float _bCoeff;
				float _brightness;
				float _contrast;
				float _gamma;

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 color = tex2D(_MainTex, i.uv);
					fixed4 factor = fixed4(_rCoeff, _gCoeff, _bCoeff, color.a);
					
					color *= _brightness;
					color = (color - factor) * _contrast + factor;
					color = clamp(color, 0.0, 1.0);
					color = pow(color, _gamma);

					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
