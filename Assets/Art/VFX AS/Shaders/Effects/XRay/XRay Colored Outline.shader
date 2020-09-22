Shader "AvalonStudios/Effects/XRay/XRay Colored Outline"
{
    Properties
    {
        _XRayInColor("Main Color", Color) = (1,1,1,1)
        [HDR]_XRayEdgeColor("XRay Edge Color", Color) = (1, 1, 1, 1)
        _FresnelBias ("Fresnel Bias", Float) = 0
		_FresnelScale ("Fresnel Scale", Float) = 1
		_FresnelPower ("Fresnel Power", Float) = 1
    }
    SubShader
    {
        Stencil
        {
            Ref 0
            Comp NotEqual
        }

        Tags 
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "XRay" = "ColoredOutline"
        }

        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        //Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            float4 _XRayEdgeColor;
            float4 _XRayInColor;
            uniform float _GlobalXRayVisibility;

            fixed4 frag (v2f i) : SV_Target
            {
                float edge = 1 - dot(i.normal, i.viewDir) * 1.5;
                return (_XRayEdgeColor) * (edge + _XRayInColor) * _GlobalXRayVisibility;
            }
            ENDCG
        }
    }
}
