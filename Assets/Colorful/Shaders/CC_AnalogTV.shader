Shader "Hidden/CC_AnalogTV"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_phase ("Phase (time)", Float) = 0.01
		_noiseIntensity ("Static noise intensity", Float) = 0.5
		_scanlinesIntensity ("Scanlines intensity", Float) = 2.0
		_scanlinesCount ("Scanlines count", Float) = 1024

		_distortion ("Distortion", Float) = 0.2
		_cubicDistortion ("Cubic Distortion", Float) = 0.6
		_scale ("Scale (Zoom)", Float) = 0.8
	}

	CGINCLUDE

		#include "UnityCG.cginc"
		#include "Colorful.cginc"

		sampler2D _MainTex;

		half _phase;
		half _grayscale;
		half _noiseIntensity;
		half _scanlinesIntensity;
		half _scanlinesCount;

		half _distortion;
		half _cubicDistortion;
		half _scale;

		half2 barrelDistortion(half2 coord) 
		{
			// Inspired by SynthEyes lens distortion algorithm
			// See http://www.ssontech.com/content/lensalg.htm

			half2 h = coord.xy - half2(0.5, 0.5);
			half r2 = h.x * h.x + h.y * h.y;
			half f = 1.0 + r2 * (_distortion + _cubicDistortion * sqrt(r2));

			return f * _scale * h + 0.5;
		}

		half4 filter(half2 uv)
		{
			half2 coord = barrelDistortion(uv);
			half4 color = tex2D(_MainTex, coord);

			half n = simpleNoise(coord.x, coord.y, 1234.0, _phase);
			half dx = fmod(n, 0.01);

			half3 result = color.rgb + color.rgb * saturate(0.1 + dx * 100.0);
			half2 sc = half2(sin(coord.y * _scanlinesCount), cos(coord.y * _scanlinesCount));
			result += color.rgb * sc.xyx * _scanlinesIntensity;
			result = color.rgb + saturate(_noiseIntensity) * (result - color.rgb);

			return half4(result, color.a);
		}

		fixed4 frag(v2f_img i):COLOR
		{
			return filter(i.uv);
		}

		fixed4 frag_grayscale(v2f_img i):COLOR
		{
			half4 result = filter(i.uv);

			fixed lum = luminance(result.rgb);
			result = half4(lum, lum, lum, result.a);

			return result;
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
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_grayscale
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}
	}

	FallBack off
}
