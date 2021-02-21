Shader "Hidden/EPO/Fill/Basic/Dots"
{
    Properties
    {
        _PublicColor                ("Color", Color) = (1, 0, 0, 1)
        _PublicHorizontalSize       ("Horizontal size", Float) = 1.0
        _PublicVerticalSize         ("Vertical size", Float) = 1.0
        _PublicHorizontalGapSize    ("Horizontal gap size", Range(-1, 1)) = 0.5
        _PublicVerticalGapSize      ("Vertical gap size", Range(-1, 1)) = 0.5
        _PublicHorizontalSoftness   ("Horizontal softness", Range(0, 1)) = 0.2
        _PublicVerticalSoftness     ("Vertical softness", Range(0, 1)) = 0.2
        _PublicHorizontalSpeed      ("Horizontal speed", Float) = 1
        _PublicVerticalSpeed        ("Vertical speed", Float) = 1
        _PublicAngle                ("Angle", Range(0, 360)) = 0.0
        _PublicSecondaryAlpha       ("Secondary alpha", Range(0, 1)) = 0.2
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
                half2 secondaryDirection : TEXCOORD3;
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
                
                o.secondaryDirection = half2(sin((_PublicAngle + 90.0f) / 57.295779513), cos((_PublicAngle + 90.0f) / 57.295779513));

                return o;
            }

            half4 _PublicColor;
            half _PublicHorizontalSize;
            half _PublicVerticalSize;
            half _PublicHorizontalGapSize;
            half _PublicVerticalGapSize;
            half _PublicHorizontalSoftness;
            half _PublicVerticalSoftness;
            half _PublicSecondaryAlpha;
            half _PublicHorizontalSpeed;
            half _PublicVerticalSpeed;

            inline half calculateAlphaMultiplier(half projection, half gapSize, half softness, half speed, half size)
            {
                return saturate(_PublicSecondaryAlpha + 
                    smoothstep(gapSize, gapSize + softness, (sin((projection + _Time.w * speed) * 1.57079632679 / size) + 1) / 2));
            }

            half4 frag (v2f i) : SV_Target
            {
				CHECK_CUTOUT

                half3 screenPos = i.screenPos.xyz / i.screenPos.w;
                
                half projection = abs(dot(screenPos * _ScreenParams.xy, i.direction));
                half secondaryProjection = abs(dot(screenPos * _ScreenParams.xy, i.secondaryDirection));

                half4 resultingColor = _PublicColor;
                resultingColor.a *= calculateAlphaMultiplier(projection, _PublicHorizontalGapSize, _PublicHorizontalSoftness, _PublicHorizontalSpeed, _PublicHorizontalSize) *
                    calculateAlphaMultiplier(secondaryProjection, _PublicVerticalGapSize, _PublicVerticalSoftness, _PublicVerticalSpeed, _PublicVerticalSize);

                return resultingColor;
            }
            ENDCG
        }
    }
}
