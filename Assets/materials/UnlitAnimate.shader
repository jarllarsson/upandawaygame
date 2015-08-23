Shader "Custom/UnlitAnimate" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_speedU("Scroll Speed U", Float) = 0.5
		_speedV("Scroll Speed V", Float) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		Lighting Off
		Cull Off
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// #pragma surface surf Standard fullforwardshadows
		#pragma surface surf NoLighting noambient alpha
		#include "UnityCG.cginc"

		#define UNITY_PASS_FORWARDBASE

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c; c.rgb = s.Albedo;
			c.a=s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		float _speedU;
		float _speedV;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			float uSpd=_speedU*_Time.y;
			float vSpd=_speedV*_Time.y;
			float2 uv=float2(IN.uv_MainTex.x+uSpd, IN.uv_MainTex.y+vSpd);
			float4 col=tex2D (_MainTex, uv).rgba;
			o.Albedo = col.rgb;
			o.Alpha = col.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
