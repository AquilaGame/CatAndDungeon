Shader "Hidden/EPO/Fill/Basic/Interlaced"
{
    Properties
    {
        _PublicColor            ("Color", Color) = (1, 0, 0, 1)
        _PublicGapColor			("Gap color", Color) = (1, 0, 0, 0.2)
        _PublicSize             ("Size", Float) = 1
        _PublicGapSize          ("Gap size", Range(-1, 1)) = 0.2
        _PublicSoftness         ("Softness", Range(0, 3)) = 0.75
        _PublicSpeed            ("Speed", Float) = 1.0
        _PublicAngle            ("Angle", Range(0, 360)) = 0.0
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
                half4 screenPos : TEXCOORD1;
                half2 direction : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
			
			DEFINE_CUTOUT

            half _PublicAngle;

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
				
                FixDepth

                o.screenPos = ComputeScreenPos(o.vertex);
				TRANSFORM_CUTOUT

                o.direction = half2(sin(_PublicAngle / 57.295779513), cos(_PublicAngle / 57.295779513));

                return o;
            }

            half4 _PublicColor;
            half4 _PublicGapColor;
            half _PublicSize;
            half _PublicGapSize;
            half _PublicSoftness;
            half _PublicSpeed;

            half4 frag (v2f i) : SV_Target
            {
				CHECK_CUTOUT

                float3 screenPos = i.screenPos.xyz / i.screenPos.w;

                half projection = abs(dot(screenPos * _ScreenParams.xy, i.direction));
                
				half factor = saturate(smoothstep(_PublicGapSize, _PublicGapSize + _PublicSoftness, (sin((projection + _Time.w * _PublicSpeed) * 1.57079632679 / _PublicSize) + 1) / 2));

                return lerp(_PublicGapColor, _PublicColor, factor);
            }
            ENDCG
        }
    }
}
