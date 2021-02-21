#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
#define FetchTexel(uv) UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex,uv)
#define FetchTexelAt(uv) FetchTexelAtFrom(_MainTex,uv,_MainTex_ST)
#define FetchTexelAtWithShift(uv,shift) FetchTexelAtFrom(_MainTex,(uv)+(shift),_MainTex_ST)
#define FetchTexelAtFrom(tex,uv,texST) UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex,uv)
#else
#define FetchTexel(uv) tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(uv,_MainTex_ST))
#define FetchTexelAt(uv) FetchTexelAtFrom(_MainTex,uv,_MainTex_ST)
#define FetchTexelAtWithShift(uv,shift) tex2D(_MainTex,UnityStereoScreenSpaceUVAdjust((uv),(_MainTex_ST))+(shift))
#define FetchTexelAtFrom(tex,uv,texST) tex2D(tex,UnityStereoScreenSpaceUVAdjust((uv),(texST)))
#endif

#define ANY 0
#define DILATE 1
#define BLUR 2
#define NOT_DILATE -1
#define NOT_BLUR -2

#define DefineEdgeDilateParameters float3 normal : TEXCOORD6;

#define ComputeScreenShift float2 clipNormal = (mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal))).xy; o.vertex.xy += (clipNormal / abs(clipNormal)) * _MainTex_TexelSize.xy * 2.0f * (_EffectSize + v.additionalScale.x) * o.vertex.w;

#define ComputeSmoothScreenShift float2 clipNormal = (mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal))).xy; o.vertex.xy += (normalize(clipNormal) / _ScreenParams.xy) * 2.0f * _DilateShift * o.vertex.w;

#define DefineTransform float4 first : TEXCOORD0; float4 second : TEXCOORD1; float4 third : TEXCOORD2; float4 fourth : TEXCOORD3; float3 center : TEXCOORD4; float3 size : TEXCOORD5; float3 stageInfo : TEXCOORD6; float2 additionalScale : TEXCOORD7;

#define GetStageModifier(index) (index >= 0 ? v.stageInfo[abs(index)] > 0.9f : v.stageInfo[abs(index)] < 0.9f)

#define TransformVertex(stage) v.vertex = mul(GetStageModifier(stage) * v.vertex * float4(v.size.xyz, 1) + float4(v.center, 0), float4x4(v.first, v.second, v.third, v.fourth));

#define TransformNormal(stage) v.normal = GetStageModifier(stage) * mul(transpose(float4x4(v.first, v.second, v.third, float4(0, 0, 0, 0))), float4(v.normal.xyz, 0));

#if UNITY_UV_STARTS_AT_TOP
#define CheckY o.vertex.y *= -_ProjectionParams.x;
#else
#define CheckY;
#endif

#if defined(UNITY_REVERSED_Z) 
#define ChangeDepth o.vertex.z += 0.0001f;
#else
#define ChangeDepth o.vertex.z -= 0.0001f;
#endif

#if EPO_HDRP
#define FixDepth ChangeDepth
#else
#define FixDepth
#endif

#define ModifyUV //o.uv.y = 1.0f - o.uv.y;

#if USE_CUTOUT
	#if TEXARRAY_CUTOUT
	#define DEFINE_CUTOUT UNITY_DECLARE_TEX2DARRAY(_CutoutTexture); half4 _CutoutTexture_ST; half _CutoutThreshold; float _TextureIndex;
	#else
	#define DEFINE_CUTOUT sampler2D _CutoutTexture; half4 _CutoutTexture_ST; half _CutoutThreshold;
	#endif
#else
#define DEFINE_CUTOUT
#endif

#if USE_CUTOUT
	#if TEXARRAY_CUTOUT
	#define CHECK_CUTOUT clip(UNITY_SAMPLE_TEX2DARRAY(_CutoutTexture, float3(i.uv, _TextureIndex)).a - _CutoutThreshold);
	#else
	#define CHECK_CUTOUT clip(tex2D(_CutoutTexture, i.uv).a - _CutoutThreshold);
	#endif
#else
#define CHECK_CUTOUT
#endif

#if USE_CUTOUT
	#define TRANSFORM_CUTOUT o.uv = TRANSFORM_TEX(v.uv, _CutoutTexture);
#else
#define TRANSFORM_CUTOUT
#endif