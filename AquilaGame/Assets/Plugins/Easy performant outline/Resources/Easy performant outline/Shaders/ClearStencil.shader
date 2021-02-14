Shader "Hidden/ClearStencil"
{
    SubShader
    {
        Cull Front ZWrite Off ZTest Always

        Pass
        {   Stencil 
            {
                Ref 0
                Comp Always
                Pass Replace
            }

            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            
            float _EffectSize;

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;

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

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
