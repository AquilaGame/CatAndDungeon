Shader "Hidden/EPO/Fill/Utils/Empty fill"
{
    SubShader
    {
        ColorMask 0

        Cull [_Cull]
        ZWrite Off
        ZTest [_ZTest]

        Pass
        {
            Stencil
            {
                Ref [_OutlineRef]
                Comp Always
                Pass Zero
                Fail Keep
                ZFail Keep
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
                UNITY_VERTEX_OUTPUT_STEREO
            };
			
			DEFINE_CUTOUT

            v2f vert (appdata v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
				
                FixDepth
				TRANSFORM_CUTOUT

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				CHECK_CUTOUT

                return half4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
