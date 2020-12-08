Shader "Custom/HalfLambert"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _RampTex ("Ramp (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _ShadowTint ("Shadow Color", Color) = (0, 0, 0, 1)
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
       
        #pragma surface surf BasicDiffuse fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _RampTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        fixed4 _Color;
        float3 _ShadowTint;
        float4 _RimColor;
        float _RimPower;

        inline float4 LightingBasicDiffuse(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            float difLight = dot(s.Normal, lightDir);
            float hLambert = difLight * 0.5 + 0.5;
            float3 ramp = tex2D(_RampTex, float2(hLambert,hLambert)).rgb;
            float3 shadowColor = s.Albedo * _ShadowTint;
            float4 col;
            col.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten);
            col.rgb += shadowColor * (1.0 - atten);
            col.a=s.Alpha;
            return col;
        }

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower);
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
