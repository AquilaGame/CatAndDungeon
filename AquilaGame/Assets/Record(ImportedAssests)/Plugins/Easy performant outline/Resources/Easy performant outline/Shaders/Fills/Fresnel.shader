Shader "Hidden/EPO/Fill/Basic/Fresnel"
{
    Properties
    {
        _PublicOuterColor           ("Outer color", Color) = (1, 0, 0, 1)
        _PublicInnerColor           ("Inner color", Color) = (0, 1, 0, 1)
        _PublicFresnelPower         ("Fresnel power", Float) = 2
        _PublicFresnelMultiplier    ("Fresnel multipler", Float) = 1
    }

    SubShader
    {
        Cull [_Cull]
        ZWrite Off
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            Stencil
            {
                Ref [_FillRef]
                Comp Equal
                Pass Zero
                Fail Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile __ USE_CUTOUT
			#pragma multi_compile __ TEXARRAY_CUTOUT
			#pragma multi_compile __ EPO_HDRP

            #include "UnityCG.cginc"
            #include "../MiskCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                
                half3 normal : NORMAL;

#if USE_CUTOUT
                float2 uv : TEXCOORD0;
#endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
#if USE_CUTOUT
                float2 uv : TEXCOORD0;
#endif
				half3 normal : TEXCOORD1;

                half3 viewDir : TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
            };
			
			DEFINE_CUTOUT

            half4 _PublicOuterColor;
            half4 _PublicInnerColor;
			half _PublicFresnelPower;
            half _PublicFresnelMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                FixDepth
                
                o.normal = UnityObjectToWorldNormal(v.normal);

                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				TRANSFORM_CUTOUT

                return o;
            }

            half4 _PublicColor;

            half4 frag (v2f i) : SV_Target
            {
				CHECK_CUTOUT

                half fresnel = pow(saturate(1.0f - dot(normalize(i.normal), normalize(i.viewDir))), _PublicFresnelPower);

                return lerp(_PublicOuterColor, _PublicInnerColor, fresnel) * _PublicFresnelMultiplier;
            }
            ENDCG
        }
    }
}
