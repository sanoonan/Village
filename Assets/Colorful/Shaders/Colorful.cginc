/* Utils */
half luminance(half3 color)
{
	return dot(color, half3(0.30, 0.59, 0.11));
}

half rot(half value, half low, half hi)
{
	if (value < low)		value += hi;
	else if (value > hi)	value -= hi;
	return value;
}

half rot10(half value)
{
	return rot(value, 0.0, 1.0);
}

half4 pixelate(sampler2D tex, half2 uv, half scale, half ratio)
{
	half ds = 1.0 / scale;
	half2 coord = half2(ds * ceil(uv.x / ds), (ds * ratio) * ceil(uv.y / ds / ratio));
	return half4(tex2D(tex, coord).xyzw);
}

half simpleNoise(half x, half y, half seed, half phase)
{
	half n = x * y * phase * seed;
	return fmod(n, 13) * fmod(n, 123);
}

/* Color conversion */
half3 HSVtoRGB(half3 hsv)
{
	half H = hsv.x * 6.0;
	half R = abs(H - 3.0) - 1.0;
	half G = 2 - abs(H - 2.0);
	half B = 2 - abs(H - 4.0);
	half3 hue = saturate(half3(R, G, B));
	return ((hue - 1) * hsv.y + 1) * hsv.z;
}

half3 RGBtoHSV(half3 rgb)
{
	half3 hsv = half3(0.0, 0.0, 0.0);

	hsv.z = max(rgb.r, max(rgb.g, rgb.b));
	half cMin = min(rgb.r, min(rgb.g, rgb.b));
	half C = hsv.z - cMin;

	if (C != 0)
	{
		hsv.y = C / hsv.z;
		half3 delta = (hsv.z - rgb) / C;
		delta.rgb -= delta.brg;
		delta.rg += half2(2.0, 4.0);

		if (rgb.r >= hsv.z)			hsv.x = delta.b;
		else if (rgb.g >= hsv.z)	hsv.x = delta.r;
		else						hsv.x = delta.g;

		hsv.x = frac(hsv.x / 6);
	}

	return hsv;
}
