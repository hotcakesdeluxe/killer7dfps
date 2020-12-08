Shader "Custom/ScreenGradient"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_TopColor("Top Color", Color) = (1,1,1,1)
		_Offset("Offset", float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
            "LightMode" = "ForwardBase"
	        "PassFlags" = "OnlyDirectional"
		}

		Cull Off
		Lighting Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
        #include "Lighting.cginc"

		struct appdata
		{
			float4 vertex	: POSITION;
			float2 uv		: TEXCOORD0;
            float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 vertex	: SV_POSITION;
			float2 uv		: TEXCOORD0;
			half4 color		: COLOR;
		};

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float4 _MainTex_ST;
		half4 _BottomColor;
		half4 _TopColor;
		float _Offset;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			float factor = mad(o.vertex.y, -0.5, 0.5);
			factor *= 1 + _Offset*2;
			factor -= _Offset;
			factor = clamp(factor, 0, 1);
			o.color = lerp(_BottomColor, _TopColor, factor) *  dot( v.normal, _WorldSpaceLightPos0.xyz );


			return o;
		}

		float3 getNoise(float2 uv)
		{
			float3 noise = tex2D(_NoiseTex, uv * 100 + _Time * 50);
			noise = mad(noise, 2.0, -0.5);

			return noise/255;
		}

		half4 frag(v2f i) : SV_Target
		{
			half4 texCol = tex2D(_MainTex, i.uv);
			
			half4 c;
			c.rgb = i.color.rgb + getNoise(i.uv);
			c.rgb *= texCol.a;
			c.a = texCol.a;

			return c;
		}

		ENDCG
		}
        // shadow caster rendering pass, implemented manually
        // using macros from UnityCG.cginc
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
	}
}