Shader "AvalonStudios/Effects/XRay/Surface XRay Replaceable"
{
	Properties
	{
		[StyledHeader(Albedo)]_Albedo("Albedo", Float) = 1
		_MainColor("Main Color", Color) = (1,1,1,1)
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		[StyledHeader(Metallic)]_Metallic("Metallic", Float) = 1
		_MetallicMap("Metallic Map", 2D) = "white" {}
		_MetallicIntensity("Metallic Intensity", Range( 0 , 1)) = 1
		[StyledHeader(Roughness)]_Roughness("Roughness", Float) = 1
		_RoughnessMap("Roughness Map", 2D) = "white" {}
		_RoughnessIntensity("Roughness Intensity", Range( -2 , 2)) = 1
		[StyledHeader(Normal)]_Normal("Normal", Float) = 1
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[StyledHeader(Emission)]_Emission("Emission", Float) = 1
		_EmissiveMap("Emissive Map", 2D) = "white" {}
		[HDR]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
		[StyledHeader(AO)]_AO("AO", Float) = 1
		_AOMap("AO Map", 2D) = "white" {}
		_Occlusion("Occlusion", Range(0, 1)) = 0
		[StyledHeader(X Ray Effect)]_XRayEffect("X Ray Effect", Float) = 1
		[HDR]_XRayEdgeColor("XRay Edge Color", Color) = (1,1,1,1)
		_XRayInColor("XRay In Color", Color) = (1, 1, 1, 1)
		// _FresnelBias ("Fresnel Bias", Float) = 0
		// _FresnelScale ("Fresnel Scale", Float) = 1
		// _FresnelPower ("Fresnel Power", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-1" "IsEmissive" = "true"  "XRay"="ColoredOutline" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		// Samplers
		sampler2D _AlbedoMap;
		sampler2D _NormalMap;
		sampler2D _MetallicMap;
		sampler2D _RoughnessMap;
		sampler2D _EmissiveMap;
		sampler2D _AOMap;

		float4 _MainColor;
		float4 _EmissiveColor;
		float _NormalIntensity;
		float _MetallicIntensity;
		float _RoughnessIntensity;
		float _Occlusion;

		struct Input {
			float2 uv_AlbedoMap;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float3 normal = UnpackScaleNormal(tex2D (_NormalMap, IN.uv_AlbedoMap), _NormalIntensity);
			o.Normal = normal.rgb;

			float4 albedo = tex2D(_AlbedoMap, IN.uv_AlbedoMap) * _MainColor;
			float metallic = (tex2D (_MetallicMap, IN.uv_AlbedoMap).r * _MetallicIntensity);
			float roughness = ( ( 1.0 - tex2D( _RoughnessMap, IN.uv_AlbedoMap).r ) * _RoughnessIntensity );
			float4 emissive = ( tex2D( _EmissiveMap, IN.uv_AlbedoMap) * _EmissiveColor );
			float ao = (tex2D (_AOMap, IN.uv_AlbedoMap).r * _Occlusion);
			o.Albedo = albedo.rgb;
			o.Metallic = metallic;
			o.Smoothness = roughness;
			o.Emission = emissive.rgb;
			o.Occlusion = ao;
			o.Alpha = 1;
		}
		ENDCG
	}
	
	Fallback "Legacy Shaders/VertexLit"
}
