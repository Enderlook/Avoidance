// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AvalonStudios/Effects/Toon Shader 03"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		[StyledHeader(Albedo)]_Albedo("Albedo", Float) = 1
		_Color("Color", Color) = (1,1,1,0)
		_MainTexture("Main Texture", 2D) = "white" {}
		[StyledHeader(Normal)]_Normal("Normal", Float) = 1
		[Toggle(_USENORMALMAP_ON)] _UseNormalMap("Use Normal Map?", Float) = 0
		_NormalMap("Normal Map", 2D) = "bump" {}
		[StyledHeader(Emissive)]_Emissive("Emissive", Float) = 1
		_EmissiveMap("Emissive Map", 2D) = "white" {}
		_EmissiveIntensity("Emissive Intensity", Float) = 0
		[StyledHeader(Shading)]_Shading("Shading", Float) = 1
		_ShadingGradient("Shading Gradient", 2D) = "white" {}
		[StyledHeader(Fresnel)]_Fresnel("Fresnel", Float) = 1
		_FresnelPower("Fresnel Power", Range( 0 , 50)) = 10
		_FresnelAngle("Fresnel Angle", Range( -30 , 1)) = -1
		[HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[StyledHeader(Gloss)]_Gloss("Gloss", Float) = 1
		_GlossSize("Gloss Size", Range( 0 , 200)) = 20
		[HDR]_GlossColor("Gloss Color", Color) = (1,1,1,1)
		[StyledHeader(XRay)]_XRay("XRay", Float) = 0
		[HDR]_XRayEdgeColor("XRay Edge Color", Color) = (0,0,0,0)
		_XRayInColor("XRay In Color", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-1" "IsEmissive" = "true"  "XRay"="ColoredOutline" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USENORMALMAP_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _XRay;
		uniform float _Normal;
		uniform float _Shading;
		uniform float _Emissive;
		uniform float4 _XRayInColor;
		uniform float _Gloss;
		uniform half _Albedo;
		uniform float _Fresnel;
		uniform float4 _XRayEdgeColor;
		uniform sampler2D _EmissiveMap;
		uniform float4 _EmissiveMap_ST;
		uniform float _EmissiveIntensity;
		uniform float4 _Color;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _GlossSize;
		uniform float4 _GlossColor;
		uniform float _FresnelPower;
		uniform float _FresnelAngle;
		uniform float4 _FresnelColor;
		uniform sampler2D _ShadingGradient;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_58_0_g4 = ase_lightColor;
			float4 lightColor24_g4 = temp_output_58_0_g4;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			#ifdef _USENORMALMAP_ON
				float3 staticSwitch68 = normalize( (WorldNormalVector( i , UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) )) );
			#else
				float3 staticSwitch68 = ase_normWorldNormal;
			#endif
			float3 useNormalMap70 = staticSwitch68;
			float3 temp_output_61_0_g4 = useNormalMap70;
			float3 normal29_g4 = temp_output_61_0_g4;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 temp_output_57_0_g4 = ase_worldlightDir;
			float3 lightDir4_g4 = temp_output_57_0_g4;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult7_g4 = normalize( ( lightDir4_g4 + ase_worldViewDir ) );
			float dotResult8_g4 = dot( normal29_g4 , normalizeResult7_g4 );
			float fresnelNdotV43_g4 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode43_g4 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV43_g4, _FresnelPower ) );
			float3 normalizeResult37_g4 = normalize( ( ( -1.0 * normal29_g4 ) + ase_worldViewDir ) );
			float dotResult39_g4 = dot( lightDir4_g4 , normalizeResult37_g4 );
			float dotResult15_g4 = dot( temp_output_61_0_g4 , temp_output_57_0_g4 );
			float2 appendResult21_g4 = (float2((0.01 + (dotResult15_g4 - -1.0) * (0.99 - 0.01) / (1.0 - -1.0)) , temp_output_58_0_g4.a));
			float4 shadingTex27_g4 = tex2D( _ShadingGradient, appendResult21_g4 );
			float4 fresnelEffect48_g4 = max( ( step( 0.1 , ( fresnelNode43_g4 * (1.0 + (dotResult39_g4 - -1.0) * (_FresnelAngle - 1.0) / (1.0 - -1.0)) ) ) * _FresnelColor ) , shadingTex27_g4 );
			float4 glossEffect14_g4 = max( ( step( 0.1 , pow( dotResult8_g4 , _GlossSize ) ) * _GlossColor ) , fresnelEffect48_g4 );
			float4 toonEffectFunc91 = ( lightColor24_g4 * glossEffect14_g4 );
			c.rgb = ( unity_AmbientSky + ( ( _Color * tex2D( _MainTexture, uv_MainTexture ) ) * toonEffectFunc91 ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 uv_EmissiveMap = i.uv_texcoord * _EmissiveMap_ST.xy + _EmissiveMap_ST.zw;
			float4 emissiveTex115 = ( tex2D( _EmissiveMap, uv_EmissiveMap ) * _EmissiveIntensity );
			o.Emission = emissiveTex115.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18301
0;0;1920;1059;4417.25;758.4354;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;67;-2352.79,1663.727;Inherit;False;1231.694;439.5334;;5;70;31;68;69;62;Normal;0.9028931,0,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;62;-2302.79,1713.727;Inherit;True;Property;_NormalMap;Normal Map;5;0;Create;True;0;0;False;0;False;-1;None;c20592f66c11ece4b87ecfbbe42b1fb3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;31;-1939.829,1944.847;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;69;-1940.678,1732.766;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;68;-1704.314,1854.695;Inherit;False;Property;_UseNormalMap;Use Normal Map?;4;0;Create;True;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;93;-2419.024,394.5882;Inherit;False;1170.238;1133.233;;10;55;76;6;42;41;54;43;91;117;118;Blend Toon Effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-1372.422,1858.275;Inherit;False;useNormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;43;-2328.711,971.789;Inherit;False;Property;_FresnelColor;Fresnel Color;14;1;[HDR];Create;True;0;0;False;0;False;1,1,1,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;6;-2333.509,444.5882;Inherit;True;Property;_ShadingGradient;Shading Gradient;10;0;Create;True;0;0;False;0;False;2eafec1b7efe2a042894259f1e517b1c;bb24fe1133e36b94289c1c00c76702ee;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;22;-2383.134,-369.8662;Inherit;False;1208.961;650.0413;Comment;6;19;16;17;15;18;92;Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2353.795,908.4387;Inherit;False;Property;_FresnelAngle;Fresnel Angle;13;0;Create;True;0;0;False;0;False;-1;1;-30;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2369.024,1127.788;Inherit;False;Property;_FresnelPower;Fresnel Power;12;0;Create;True;0;0;False;0;False;10;7.1;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-2347.825,841.6826;Inherit;False;Property;_GlossSize;Gloss Size;16;0;Create;True;0;0;False;0;False;20;5.4;0;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;55;-2315.728,683.8469;Inherit;False;Property;_GlossColor;Gloss Color;17;1;[HDR];Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;118;-2284.389,1332.755;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;76;-2323.49,619.2526;Inherit;False;70;useNormalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;106;-2378.646,-938.5864;Inherit;False;1183.674;355.653;;5;115;104;102;103;101;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;117;-2339.767,1197.554;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;101;-2328.646,-888.5864;Inherit;True;Property;_EmissiveMap;Emissive Map;7;0;Create;True;0;0;False;0;False;None;5246018d188625049813d0cb7579d961;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;95;-1885.08,878.5482;Inherit;False;Toon Shading Effect 02;-1;;4;112028a726ff64d4a973cc477a4711cb;0;9;60;SAMPLER2D;0;False;61;FLOAT3;0,0,0;False;63;COLOR;1,1,1,1;False;62;FLOAT;0;False;64;FLOAT;0;False;65;COLOR;1,1,1,1;False;66;FLOAT;0;False;57;FLOAT3;0,0,0;False;58;COLOR;1,1,1,1;False;1;COLOR;56
Node;AmplifyShaderEditor.TexturePropertyNode;16;-2333.134,-142.4766;Inherit;True;Property;_MainTexture;Main Texture;2;0;Create;True;0;0;False;0;False;None;eb56013c82530874c86643222a6472f4;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;-1551.423,876.7906;Inherit;False;toonEffectFunc;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;102;-2071.393,-887.0448;Inherit;True;Property;_TextureSample3;Texture Sample 3;15;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;-2007.394,-689.0448;Inherit;False;Property;_EmissiveIntensity;Emissive Intensity;8;0;Create;True;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-2088.133,-143.4766;Inherit;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-2006.477,-319.8662;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1693.134,-206.4764;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-1690.393,-880.0448;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1692.391,-25.99985;Inherit;False;91;toonEffectFunc;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;114;-4029.249,-650.4246;Inherit;False;1160.128;160.2299;;7;122;111;107;112;109;110;108;Drawers;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;-1419.26,-887.445;Inherit;False;emissiveTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;121;-3856.644,-360.2115;Inherit;False;488.9919;258.7303;;2;119;120;Replace Value for Outline Colored;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1409.173,-50.31173;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;23;-1076.943,-75.29501;Inherit;False;unity_AmbientSky;0;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;119;-3806.644,-310.2115;Inherit;False;Property;_XRayEdgeColor;XRay Edge Color;19;1;[HDR];Create;True;0;0;True;0;False;0,0,0,0;2.670157,0.07842992,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;116;-725.972,-172.797;Inherit;False;115;emissiveTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-3328.831,-599.1947;Inherit;False;Property;_Fresnel;Fresnel;11;0;Create;True;0;0;True;1;StyledHeader(Fresnel);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-3979.249,-600.14;Half;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;True;1;StyledHeader(Albedo);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-3169.177,-600.2765;Inherit;False;Property;_Gloss;Gloss;15;0;Create;True;0;0;True;1;StyledHeader(Gloss);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-728.1154,-73.47245;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-3662.313,-600.4246;Inherit;False;Property;_Emissive;Emissive;6;0;Create;True;0;0;True;1;StyledHeader(Emissive);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-3495.125,-599.998;Inherit;False;Property;_Shading;Shading;9;0;Create;True;0;0;True;1;StyledHeader(Shading);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-3822.777,-600.4246;Inherit;False;Property;_Normal;Normal;3;0;Create;True;0;0;True;1;StyledHeader(Normal);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-3014.751,-601.0777;Inherit;False;Property;_XRay;XRay;18;0;Create;True;0;0;True;1;StyledHeader(XRay);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;120;-3601.652,-308.4812;Inherit;False;Property;_XRayInColor;XRay In Color;20;0;Create;True;0;0;True;0;False;1,1,1,1;1,0.9048886,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-87.36226,-214.0482;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;AvalonStudios/Effects/Toon Shader 03;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;-1;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;1;XRay=ColoredOutline;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;62;0
WireConnection;68;1;69;0
WireConnection;68;0;31;0
WireConnection;70;0;68;0
WireConnection;95;60;6;0
WireConnection;95;61;76;0
WireConnection;95;63;55;0
WireConnection;95;62;54;0
WireConnection;95;64;42;0
WireConnection;95;65;43;0
WireConnection;95;66;41;0
WireConnection;95;57;117;0
WireConnection;95;58;118;0
WireConnection;91;0;95;56
WireConnection;102;0;101;0
WireConnection;17;0;16;0
WireConnection;18;0;15;0
WireConnection;18;1;17;0
WireConnection;104;0;102;0
WireConnection;104;1;103;0
WireConnection;115;0;104;0
WireConnection;19;0;18;0
WireConnection;19;1;92;0
WireConnection;24;0;23;0
WireConnection;24;1;19;0
WireConnection;0;2;116;0
WireConnection;0;13;24;0
ASEEND*/
//CHKSM=91E6369FFF52C99D08FC2D852FB9F8F78573DBBC