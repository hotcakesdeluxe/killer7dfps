﻿Shader "Custom/Dithered/Distance Standard Cutout"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		//_FadeStart("Fade Start", Float) = 50
		//_FadeEnd("Fade End", Float) = 100
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#include "Includes/Dither_Distance.cginc"
		#pragma surface surf Standard fullforwardshadows addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			PerformDither(IN);
			clip(o.Alpha - 0.5);
		}
		ENDCG
	}


	FallBack "Diffuse"
}
