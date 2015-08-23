Shader "Custom/triplanar" {
	Properties {
		_ColorTop ("ColorTop", Color) = (1,1,1,1)
		_ColorBottom ("ColorBottom", Color) = (1,1,1,1)
		_ColorSides ("ColorSides", Color) = (1,1,1,1)
		_MainTexTop ("Top (RGB)", 2D) = "white" {}
		_MainTexSides ("Sides (RGB)", 2D) = "white" {}
		_MainTexBottom ("Bottom (RGB)", 2D) = "white" {}
		_Scale("Scale", Float) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTexTop;
		sampler2D _MainTexBottom;
		sampler2D _MainTexSides;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos; 
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _ColorTop;
		fixed4 _ColorBottom;
		fixed4 _ColorSides;
		float _Scale;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Get input
			float3 inNormal = IN.worldNormal;
			half4 blendedColour=half4(0.0,0.0,0.0,0.0);
			float2 coordSide1=IN.worldPos.zy;
			float2 coordSide2=IN.worldPos.xy;
			float2 coordTopBot=IN.worldPos.zx;
			float scale=_Scale;

			half4 topBottom;
			if (IN.worldNormal.y>0)
				topBottom=tex2D(_MainTexTop,coordTopBot*scale)*_ColorTop;
			else
				topBottom=tex2D(_MainTexBottom,coordTopBot*scale)*_ColorBottom;
			half4 sides1=tex2D(_MainTexSides,coordSide1*scale)*_ColorSides;
			half4 sides2=tex2D(_MainTexSides,coordSide2*scale)*_ColorSides;

			float3 blendWeights=normalize(abs(inNormal.xyz));
			float total = blendWeights.x+blendWeights.y+blendWeights.z;
			blendWeights /= float3(total,total,total);
			blendedColour = sides1*blendWeights.x + sides2*blendWeights.z + topBottom*blendWeights.y;

			// Albedo comes from a texture tinted by color
			fixed4 c = blendedColour;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
