Shader "Custom/UnlitDoubleSided" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Lighting Off
		Cull Off
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// #pragma surface surf Standard fullforwardshadows
		#pragma surface surf NoLighting noambient

		#define UNITY_PASS_FORWARDBASE

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c; c.rgb = s.Albedo;
			c.a=1.0f;
			return c;
		}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;}
		ENDCG
	} 
	FallBack "Diffuse"
}
