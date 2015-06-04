Shader "Hidden/CC_Levels"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_inputMin ("Input Black", Vector) = (0, 0, 0, 1)
		_inputMax ("Input White", Vector) = (1, 1, 1, 1)
		_inputGamma ("Input Gamma", Vector) = (1, 1, 1, 1)
		_outputMin ("Output Black", Vector) = (0, 0, 0, 1)
		_outputMax ("Output White", Vector) = (1, 1, 1, 1)
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
				fixed4 _inputMin;
				fixed4 _inputMax;
				fixed4 _inputGamma;
				fixed4 _outputMin;
				fixed4 _outputMax;

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 color = tex2D(_MainTex, i.uv);
					color = lerp(_outputMin, _outputMax, pow(min(max(color - _inputMin, fixed4(0.0, 0.0, 0.0, 0.0)) / (_inputMax - _inputMin), fixed4(1.0, 1.0, 1.0, 1.0)), 1.0 / _inputGamma));
					return color;
				}

			ENDCG
		}
	}

	FallBack off
}
