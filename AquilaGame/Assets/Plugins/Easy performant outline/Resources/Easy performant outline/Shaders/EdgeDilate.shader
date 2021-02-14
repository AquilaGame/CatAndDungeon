Shader "Hidden/Edge dilate"
{
    SubShader
    {
        Cull [_Cull]
        ZWrite Off
        ZTest [_ZTest]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "MiskCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;

				DefineEdgeDilateParameters

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			half _DilateShift;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

				ComputeSmoothScreenShift;

                FixDepth

                return o;
            }

            half4 _Color;

            half4 frag (v2f i) : SV_Target
            {
                half4 resultingColor = _Color;
                resultingColor.rgb *= resultingColor.a;

                return resultingColor;
            }
            ENDCG
        }
    }
}
