Shader "Hidden/CC_CrossStitch"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_stitchSize ("Stitch Size (Int)", Float) = 8
		_brightness ("Brightness (Float)", Float) = 1.5
		_scale ("Scale (Float)", Float) = 1.0
		_ratio ("Ratio (Float)", Float) = 1.0
	}

	CGINCLUDE
	
		#include "UnityCG.cginc"
		#include "Colorful.cginc"

		sampler2D _MainTex;
		half _stitchSize;
		half _brightness;
		half _scale;
		half _ratio;

		half2 stitch(half2 uv)
		{
			half2 pixelUV = uv * _ScreenParams.xy;
			half2 offset = floor(pixelUV);
			offset.x = offset.x - offset.y;
			offset.y = offset.x + offset.y * 2;
			return fmod(offset, half2(_stitchSize, _stitchSize));
		}

		fixed4 frag(v2f_img i):COLOR
		{
			half2 reminder = stitch(i.uv);
			half4 color;
			if (reminder.x == 0 || reminder.y == 0) color = tex2D(_MainTex, i.uv) * _brightness;
			else									color = half4(0.0, 0.0, 0.0, 1.0);
			return color;
		}

		fixed4 frag_invert(v2f_img i):COLOR
		{
			half2 reminder = stitch(i.uv);
			half4 color;
			if (reminder.x == 0 || reminder.y == 0) color = half4(0.0, 0.0, 0.0, 1.0);
			else									color = tex2D(_MainTex, i.uv) * _brightness;
			return color;
		}

		fixed4 frag_px(v2f_img i):COLOR
		{
			half2 reminder = stitch(i.uv);
			half4 color;
			if (reminder.x == 0 || reminder.y == 0) color = pixelate(_MainTex, i.uv, _scale, _ratio) * _brightness;
			else									color = half4(0.0, 0.0, 0.0, 1.0);
			return color;
		}

		fixed4 frag_invert_px(v2f_img i):COLOR
		{
			half2 reminder = stitch(i.uv);
			half4 color;
			if (reminder.x == 0 || reminder.y == 0) color = half4(0.0, 0.0, 0.0, 1.0);
			else									color = pixelate(_MainTex, i.uv, _scale, _ratio) * _brightness;
			return color;
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
				#pragma fragment frag_invert
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_px
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}

		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_invert_px
				#pragma fragmentoption ARB_precision_hint_fastest 

			ENDCG
		}
	}

	FallBack off
}
