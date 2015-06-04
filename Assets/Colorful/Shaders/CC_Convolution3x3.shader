Shader "Hidden/CC_Convolution3x3"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_px ("Pixel Width", Float) = 1
		_py ("Pixel Height", Float) = 1
		_amount ("Amount", Range(0.0, 1.0)) = 1.0
		_kernelT ("Kernel Top Row", Vector) = (0, 0, 0, 0)
		_kernelM ("Kernel Middle Row", Vector) = (0, 0, 0, 0)
		_kernelB ("Kernel Bottom Row", Vector) = (0, 0, 0, 0)
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
				half _px;
				half _py;
				half _amount;
				half4 _kernelT;
				half4 _kernelM;
				half4 _kernelB;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 origin;
					half4 color = half4(0, 0, 0, 0);
					half4 temp;

					// Top
					temp = tex2D(_MainTex, i.uv + half2(-_px, -_py));
					color += temp * _kernelT.x;

					temp = tex2D(_MainTex, i.uv + half2(   0, -_py));
					color += temp * _kernelT.y;

					temp = tex2D(_MainTex, i.uv + half2( _px, -_py));
					color += temp * _kernelT.z;

					// Middle
					temp = tex2D(_MainTex, i.uv + half2(-_px,   0));
					color += temp * _kernelM.x;

					origin = tex2D(_MainTex, i.uv);
					color += origin * _kernelM.y;

					temp = tex2D(_MainTex, i.uv + half2( _px,   0));
					color += temp * _kernelM.z;

					// Bottom
					temp = tex2D(_MainTex, i.uv + half2(-_px,  _py));
					color += temp * _kernelB.x;

					temp = tex2D(_MainTex, i.uv + half2(   0,  _py));
					color += temp * _kernelB.y;

					temp = tex2D(_MainTex, i.uv + half2( _px,  _py));
					color += temp * _kernelB.z;

					return lerp(origin, color, _amount);
				}

			ENDCG
		}
	}

	FallBack off
}
