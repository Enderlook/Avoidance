// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AvalonStudios/SkyBox/Procedural Sky"
{
	Properties
	{
		[StyledHeader(Sky)]_Sky("Sky", Float) = 0
		_Dark("Dark", Color) = (0,0,0,0)
		_Light("Light", Color) = (0,0,0,0)
		_LightEdge("Light Edge", Color) = (0,0,0,0)
		_DarkEdge("Dark Edge", Color) = (0,0,0,0)
		_DarkColorStrength("Dark Color Strength", Float) = 1
		_LightColorStrength("Light Color Strength", Float) = 1
		[StyledHeader(Stars)]_Stars("Stars", Float) = 0
		[Toggle(_HASSTARS_ON)] _HasStars("Has Stars?", Float) = 0
		_StarsScale("Stars Scale", Vector) = (8,2,0,0)
		_StarDensity("Star Density", Float) = 10
		[StyledHeader(Fog)]_Fog("Fog", Float) = 0
		[Toggle(_HASFOG_ON)] _HasFog("Has Fog?", Float) = 0
		_FogHeight("Fog Height", Range( 0 , 100)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Background"  "Queue" = "Background+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _HASFOG_ON
		#pragma shader_feature_local _HASSTARS_ON
		#pragma surface surf Unlit keepalpha noshadow nofog 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _Sky;
		uniform float _Stars;
		uniform float _Fog;
		uniform float4 _Dark;
		uniform float4 _Light;
		uniform float4 _LightEdge;
		uniform float _DarkColorStrength;
		uniform float4 _DarkEdge;
		uniform float _LightColorStrength;
		uniform float2 _StarsScale;
		uniform float _StarDensity;
		uniform float _FogHeight;


		inline float2 UnityVoronoiRandomVector( float2 UV, float offset )
		{
			float2x2 m = float2x2( 15.27, 47.63, 99.41, 89.98 );
			UV = frac( sin(mul(UV, m) ) * 46839.32 );
			return float2( sin(UV.y* +offset ) * 0.5 + 0.5, cos( UV.x* offset ) * 0.5 + 0.5 );
		}
		
		//x - Out y - Cells
		float3 UnityVoronoi( float2 UV, float AngleOffset, float CellDensity, inout float2 mr )
		{
			float2 g = floor( UV * CellDensity );
			float2 f = frac( UV * CellDensity );
			float t = 8.0;
			float3 res = float3( 8.0, 0.0, 0.0 );
		
			for( int y = -1; y <= 1; y++ )
			{
				for( int x = -1; x <= 1; x++ )
				{
					float2 lattice = float2( x, y );
					float2 offset = UnityVoronoiRandomVector( lattice + g, AngleOffset );
					float d = distance( lattice + offset, f );
		
					if( d < res.x )
					{
						mr = f - lattice - offset;
						res = float3( d, offset.x, offset.y );
					}
				}
			}
			return res;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult2_g20 = normalize( ase_worldPos );
			float3 break4_g20 = normalizeResult2_g20;
			float2 appendResult13_g20 = (float2(( atan2( break4_g20.x , break4_g20.z ) / 6.28318548202515 ) , ( asin( break4_g20.y ) / ( UNITY_PI / 2.0 ) )));
			float vTextCoord8 = (0.0 + (appendResult13_g20.y - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float4 lerpResult15 = lerp( _Dark , _Light , vTextCoord8);
			float4 lerpResult19 = lerp( lerpResult15 , _LightEdge , pow( vTextCoord8 , _DarkColorStrength ));
			float4 lerpResult21 = lerp( lerpResult19 , _DarkEdge , pow( ( 1.0 - vTextCoord8 ) , _LightColorStrength ));
			float4 lerpColors22 = lerpResult21;
			float3 normalizeResult2_g21 = normalize( ase_worldPos );
			float3 break4_g21 = normalizeResult2_g21;
			float2 appendResult13_g21 = (float2(( atan2( break4_g21.x , break4_g21.z ) / 6.28318548202515 ) , ( asin( break4_g21.y ) / ( UNITY_PI / 2.0 ) )));
			float2 appendResult39 = (float2(_Time.y , 0.0));
			float2 uv30 = 0;
			float3 unityVoronoy30 = UnityVoronoi((appendResult13_g21*_StarsScale + ( 0.007 * appendResult39 )),20.0,_StarDensity,uv30);
			float stars34 = pow( ( 1.0 - saturate( unityVoronoy30.x ) ) , 100.0 );
			#ifdef _HASSTARS_ON
				float4 staticSwitch35 = ( lerpColors22 + stars34 );
			#else
				float4 staticSwitch35 = lerpColors22;
			#endif
			float3 normalizeResult2_g22 = normalize( ase_worldPos );
			float3 break4_g22 = normalizeResult2_g22;
			float2 appendResult13_g22 = (float2(( atan2( break4_g22.x , break4_g22.z ) / 6.28318548202515 ) , ( asin( break4_g22.y ) / ( UNITY_PI / 2.0 ) )));
			float fog49 = pow( saturate( ( 1.0 - appendResult13_g22.y ) ) , _FogHeight );
			float4 lerpResult50 = lerp( staticSwitch35 , unity_FogColor , fog49);
			#ifdef _HASFOG_ON
				float4 staticSwitch53 = lerpResult50;
			#else
				float4 staticSwitch53 = staticSwitch35;
			#endif
			o.Emission = staticSwitch53.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18301
0;0;1920;1059;938.3105;330.5502;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;24;-2703.33,1792.123;Inherit;False;2287.65;734.448;Comment;14;34;33;32;31;30;28;27;29;41;26;25;39;40;38;Stars;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1;-2681.063,-418.9482;Inherit;False;1156.111;543.2762;Comment;6;72;4;3;8;7;6;UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;72;-2657.079,-84.76007;Inherit;True;SkyBoxUV;-1;;20;3d6fdb21e5ebe8b41bbb9cdaa4011692;0;0;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;38;-2647.921,2302.035;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2354.842,2146.241;Inherit;False;Constant;_Float5;Float 5;10;0;Create;True;0;0;False;0;False;0.007;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;6;-2322.719,-85.95911;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;39;-2382.477,2300.871;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;25;-2117.168,1846.435;Inherit;False;SkyBoxUV;-1;;21;3d6fdb21e5ebe8b41bbb9cdaa4011692;0;0;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;26;-2108.596,1935.975;Inherit;False;Property;_StarsScale;Stars Scale;10;0;Create;True;0;0;False;0;False;8,2;20,3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2136.598,2225.714;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;7;-1970.704,-97.65891;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2;-2728.407,305.985;Inherit;False;1648.693;1300.792;Comment;14;22;21;18;19;20;11;15;17;16;13;14;12;9;10;Lerping Colors;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;29;-1765.596,1844.975;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1704.309,2075.635;Inherit;False;Property;_StarDensity;Star Density;11;0;Create;True;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1746.669,-117.3141;Inherit;True;vTextCoord;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;30;-1482.563,1843.663;Inherit;True;0;0;1;0;1;False;1;True;False;4;0;FLOAT2;0,0;False;1;FLOAT;20;False;2;FLOAT;5;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;14;-2656.236,1041.011;Inherit;False;Property;_DarkColorStrength;Dark Color Strength;6;0;Create;True;0;0;False;0;False;1;2.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-2678.496,342.4221;Inherit;False;Property;_Dark;Dark;2;0;Create;True;0;0;False;0;False;0,0,0,0;0.1629237,0.0895781,0.3113208,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-2662.247,536.1707;Inherit;False;Property;_Light;Light;3;0;Create;True;0;0;False;0;False;0,0,0,0;0,0.3301886,0.1400175,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;42;-2687.387,2757.158;Inherit;False;1538.758;342.5483;Comment;7;49;48;47;46;45;44;73;Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;9;-2639.644,830.2819;Inherit;False;8;vTextCoord;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;16;-2190.455,1292.217;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;17;-2279.779,1013.834;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-2280.249,791.5082;Inherit;False;Property;_LightEdge;Light Edge;4;0;Create;True;0;0;False;0;False;0,0,0,0;0.02892488,0.2452829,0.06395526,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;73;-2636.012,2809.06;Inherit;False;SkyBoxUV;-1;;22;3d6fdb21e5ebe8b41bbb9cdaa4011692;0;0;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;15;-2269.891,458.1084;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;31;-1293.655,1842.793;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2215.052,1517.716;Inherit;False;Property;_LightColorStrength;Light Color Strength;7;0;Create;True;0;0;False;0;False;1;0.49;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;18;-1884.452,1373.902;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;44;-2403.288,2808.43;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.OneMinusNode;32;-1101.656,1840.793;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-1890.625,778.675;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;-1864.77,1119.559;Inherit;False;Property;_DarkEdge;Dark Edge;5;0;Create;True;0;0;False;0;False;0,0,0,0;0.2164627,0,0.3396226,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;33;-911.6551,1839.793;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-1581.489,1105.961;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;45;-2089.035,2831.332;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;47;-1839.666,2832.606;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-1313.668,1102.104;Inherit;True;lerpColors;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1934.124,2987.707;Inherit;False;Property;_FogHeight;Fog Height;14;0;Create;True;0;0;False;0;False;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-643.7349,1836.123;Inherit;False;stars;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;54;-179.6225,557.5363;Inherit;False;1849.264;482;Comment;9;37;23;36;51;52;35;50;53;0;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-120.4546,855.6948;Inherit;False;34;stars;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-129.6225,643.0709;Inherit;False;22;lerpColors;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;48;-1624.124,2899.707;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-1391.629,2895.246;Inherit;False;fog;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;252.793,760.3124;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;552.3031,855.8394;Inherit;False;49;fog;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;51;492.9288,761.7971;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;35;498.0987,645.2817;Inherit;False;Property;_HasStars;Has Stars?;9;0;Create;True;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;50;867.7485,738.3485;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;77;-189.9674,60.44983;Inherit;False;555.6569;166.0281;;3;74;75;76;Drawers;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;27;-2097.495,2072.953;Inherit;False;Constant;_Vector1;Vector 1;4;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;4;-1857.61,-378.237;Inherit;True;UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;53;1093.267,648.8009;Inherit;False;Property;_HasFog;Has Fog?;13;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;3;-2301.51,-365.6691;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;74;-139.9674,111.4779;Inherit;False;Property;_Sky;Sky;1;0;Create;True;0;0;True;1;StyledHeader(Sky);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;24.68951,110.4498;Inherit;False;Property;_Stars;Stars;8;0;Create;True;0;0;True;1;StyledHeader(Stars);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;189.6895,111.4498;Inherit;False;Property;_Fog;Fog;12;0;Create;True;0;0;True;1;StyledHeader(Fog);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1406.642,607.5363;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;AvalonStudios/SkyBox/Procedural Sky;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Background;;Background;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;72;0
WireConnection;39;0;38;0
WireConnection;41;0;40;0
WireConnection;41;1;39;0
WireConnection;7;0;6;1
WireConnection;29;0;25;0
WireConnection;29;1;26;0
WireConnection;29;2;41;0
WireConnection;8;0;7;0
WireConnection;30;0;29;0
WireConnection;30;2;28;0
WireConnection;16;0;9;0
WireConnection;17;0;9;0
WireConnection;17;1;14;0
WireConnection;15;0;12;0
WireConnection;15;1;10;0
WireConnection;15;2;9;0
WireConnection;31;0;30;0
WireConnection;18;0;16;0
WireConnection;18;1;11;0
WireConnection;44;0;73;0
WireConnection;32;0;31;0
WireConnection;19;0;15;0
WireConnection;19;1;13;0
WireConnection;19;2;17;0
WireConnection;33;0;32;0
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;21;2;18;0
WireConnection;45;0;44;1
WireConnection;47;0;45;0
WireConnection;22;0;21;0
WireConnection;34;0;33;0
WireConnection;48;0;47;0
WireConnection;48;1;46;0
WireConnection;49;0;48;0
WireConnection;36;0;23;0
WireConnection;36;1;37;0
WireConnection;35;1;23;0
WireConnection;35;0;36;0
WireConnection;50;0;35;0
WireConnection;50;1;51;0
WireConnection;50;2;52;0
WireConnection;4;0;3;0
WireConnection;53;1;35;0
WireConnection;53;0;50;0
WireConnection;0;2;53;0
ASEEND*/
//CHKSM=AE5FB513EF7C3A22175FFA5CF4DFABDE274FBB21