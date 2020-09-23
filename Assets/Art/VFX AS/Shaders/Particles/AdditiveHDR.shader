// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AvalonStudios/Particles/Additive HDR"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One , One One
		
		CGPROGRAM
		#pragma target 3.0
		#pragma exclude_renderers xbox360 xboxone ps4 psp2 n3ds wiiu 
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodirlightmap nofog nometa 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 MainColorWithAlpha44 = _Color;
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 UVs14 = uv0_MainTex;
			float4 tex2DNode2 = tex2D( _MainTex, UVs14 );
			float MainTexture16 = tex2DNode2.a;
			float4 appendResult41 = (float4(i.vertexColor.r , i.vertexColor.g , i.vertexColor.b , 0.0));
			float MainColorAlpha46 = _Color.a;
			float4 temp_output_6_0 = ( ( MainColorWithAlpha44 * MainTexture16 ) * appendResult41 * i.vertexColor.a * MainColorAlpha46 );
			float4 Color22 = temp_output_6_0;
			o.Emission = Color22.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "AvalonStudios/Mobile/Particles/Additive"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
0;0;1918;1059;4519.498;1008.939;4.213496;True;True
Node;AmplifyShaderEditor.CommentaryNode;17;-2499.98,-320.7628;Inherit;False;1527.754;519.7872;Comment;6;31;16;2;14;1;36;Main Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-2449.98,-267.9283;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;False;0;8734a3a6cfedbad4a9b061abbcf7830e;8734a3a6cfedbad4a9b061abbcf7830e;False;white;LockedToTexture2D;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-2161.542,-77.87399;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;50;-2489.451,-1141.247;Inherit;False;840.6031;654.6279;Comment;5;44;45;49;46;3;Main Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1909.078,-78.32771;Inherit;True;UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-1635.49,-267.9731;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-2439.451,-1091.247;Inherit;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1.720795,1.720795,1.720795,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-1307.138,-262.9664;Inherit;True;MainTexture;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1931.523,-1089.504;Inherit;True;MainColorWithAlpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;25;-2500.17,338.0151;Inherit;False;1777.466;915.1002;Comment;10;10;41;6;22;5;4;18;42;51;54;Color & Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-2450.17,595.6033;Inherit;True;16;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-2147.031,-716.6188;Inherit;True;MainColorAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-2453.524,389.6299;Inherit;True;44;MainColorWithAlpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;5;-2182.048,892.3087;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;41;-1906.644,761.1493;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-1874.306,1047.865;Inherit;True;46;MainColorAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2148.451,528.4748;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1605.568,732.5752;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;30;-522.4251,333.5844;Inherit;False;716.2175;724.6855;Comment;2;23;12;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-1249.503,497.4861;Inherit;True;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-981.1848,760.9336;Inherit;True;AlphaSlpit;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;10;-1247.365,763.8203;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;23;-450.3506,451.8175;Inherit;True;22;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1309.309,-8.220936;Inherit;True;MainTextureColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;49;-2044.108,-881.754;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1891.847,-887.0106;Inherit;True;MainColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;12;-69.20758,469.9201;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;AvalonStudios/Particles/Additive HDR;False;False;False;False;True;True;True;False;True;True;True;False;False;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;8;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;AvalonStudios/Mobile/Particles/Additive;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;2;1;0
WireConnection;14;0;36;0
WireConnection;2;0;1;0
WireConnection;2;1;14;0
WireConnection;16;0;2;4
WireConnection;44;0;3;0
WireConnection;46;0;3;4
WireConnection;41;0;5;1
WireConnection;41;1;5;2
WireConnection;41;2;5;3
WireConnection;4;0;51;0
WireConnection;4;1;18;0
WireConnection;6;0;4;0
WireConnection;6;1;41;0
WireConnection;6;2;5;4
WireConnection;6;3;54;0
WireConnection;22;0;6;0
WireConnection;42;0;10;3
WireConnection;10;0;6;0
WireConnection;31;0;2;0
WireConnection;49;0;3;1
WireConnection;49;1;3;2
WireConnection;49;2;3;3
WireConnection;45;0;49;0
WireConnection;12;2;23;0
ASEEND*/
//CHKSM=EF79B9B3FE689CBF04B17DBF3A9A97CB969A67CE