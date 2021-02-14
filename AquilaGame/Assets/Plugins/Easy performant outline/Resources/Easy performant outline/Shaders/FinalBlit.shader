Shader "Hidden/FinalBlit"
{
    SubShader
    {
        Cull Front ZWrite Off ZTest Always
        Blend One OneMinusSrcAlpha

        Pass
        {
            Stencil 
            {
                Ref [_OutlineRef]
                Comp [_Comparison]
                Pass [_Operation]
                ZFail Keep
                Fail Keep
                ReadMask [_ReadMask]
                WriteMask 255
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile __ USE_INFO_BUFFER
            
            #include "UnityCG.cginc"
            #include "MiskCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half3 normal : NORMAL;
				DefineTransform

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            half _EffectSize;

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_InitialTex);
            half4 _InitialTex_ST;
            half4 _InitialTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
                TransformVertex(ANY)
				TransformNormal(ANY)

                o.vertex = UnityObjectToClipPos(v.vertex);
				
                ComputeScreenShift

                o.uv = ComputeScreenPos(o.vertex);

#if UNITY_UV_STARTS_AT_TOP
				ModifyUV
#endif

                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
                half2 uv = i.uv.xy / i.uv.w;
                half4 texel = FetchTexel(uv);

                return texel;// + float4(0.3, 0, 0.3, 0.8);
            }
            ENDCG
        }
    }
}
