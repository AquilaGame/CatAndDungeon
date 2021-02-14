Shader "Hidden/Blur"
{
    SubShader
    {
        Cull Front ZWrite Off ZTest Always

        Pass
        {
            Stencil 
            {
                Ref [_Ref]
                Comp [_Comparison]
                Pass IncrWrap
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile ANISOTROPIC_BLUR BOX_BLUR GAUSSIAN5X5 GAUSSIAN9X9 GAUSSIAN13X13
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
            half2 _Shift;
            
#if USE_INFO_BUFFER
            UNITY_DECLARE_SCREENSPACE_TEXTURE(_InfoBuffer);
            half4 _InfoBuffer_ST;
            half4 _InfoBuffer_TexelSize;
#endif

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                TransformVertex(BLUR)
				TransformNormal(BLUR)

                o.vertex = UnityObjectToClipPos(v.vertex); 

                ComputeScreenShift
					
                o.uv = ComputeScreenPos(o.vertex);

				CheckY

#if UNITY_UV_STARTS_AT_TOP
				ModifyUV
#endif

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
                half2 targetShift = _Shift * _MainTex_TexelSize.xy;

                half2 uv = i.uv.xy/i.uv.w;

#if USE_INFO_BUFFER
                half4 info = FetchTexelAtFrom(_InfoBuffer, uv, _InfoBuffer_ST);
                targetShift *= info.g * 2.0f;
#endif

#if BOX_BLUR || ANISOTROPIC_BLUR
                half4 first = FetchTexelAtWithShift(uv, targetShift);
                half4 second = FetchTexelAtWithShift(uv, -targetShift);
                half4 center = FetchTexelAt(uv);    
                half4 result = (first + second + center) / 3.0f;

                return result;
#endif

#if GAUSSIAN5X5
                half4 result = half4(0, 0, 0, 0);
                half2 off = targetShift *  1.3333333333333333;
                result += FetchTexel(uv) * 0.29411764705882354;
                result += FetchTexelAtWithShift(uv,  off) * 0.35294117647058826;
                result += FetchTexelAtWithShift(uv, -off) * 0.35294117647058826;

                return result;
#endif

#if GAUSSIAN9X9
                half4 result = float4(0, 0, 0, 0);
                half2 off1 = 1.3846153846 * targetShift;
                half2 off2 = 3.2307692308 * targetShift;
                result += FetchTexel(uv) * 0.2270270270;
                result += FetchTexelAtWithShift(uv,  off1) * 0.3162162162;
                result += FetchTexelAtWithShift(uv, -off1) * 0.3162162162;
                result += FetchTexelAtWithShift(uv,  off2) * 0.0702702703;
                result += FetchTexelAtWithShift(uv, -off2) * 0.0702702703;

                return result;
#endif

#if GAUSSIAN13X13
                half4 result = float4(0, 0, 0, 0);
                half2 off1 = 1.411764705882353 * targetShift;
                half2 off2 = 3.2941176470588234 * targetShift;
                half2 off3 = 5.176470588235294 * targetShift;

                result += FetchTexel(uv) * 0.1964825501511404;
                result += FetchTexelAtWithShift(uv,  off1) * 0.2969069646728344;
                result += FetchTexelAtWithShift(uv, -off1) * 0.2969069646728344;
                result += FetchTexelAtWithShift(uv,  off2) * 0.09447039785044732;
                result += FetchTexelAtWithShift(uv, -off2) * 0.09447039785044732;
                result += FetchTexelAtWithShift(uv,  off3) * 0.010381362401148057;
                result += FetchTexelAtWithShift(uv, -off3) * 0.010381362401148057;

                return result;
#endif

                return half4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
