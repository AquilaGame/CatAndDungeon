// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SyntyStudios/CustomCharacter"
{
	Properties
	{
		_Color_Primary("Color_Primary", Color) = (0.2431373,0.4196079,0.6196079,0)
		_Color_Secondary("Color_Secondary", Color) = (0.8196079,0.6431373,0.2980392,0)
		_Color_Leather_Primary("Color_Leather_Primary", Color) = (0.282353,0.2078432,0.1647059,0)
		_Color_Metal_Primary("Color_Metal_Primary", Color) = (0.5960785,0.6117647,0.627451,0)
		_Color_Leather_Secondary("Color_Leather_Secondary", Color) = (0.372549,0.3294118,0.2784314,0)
		_Color_Metal_Dark("Color_Metal_Dark", Color) = (0.1764706,0.1960784,0.2156863,0)
		_Color_Metal_Secondary("Color_Metal_Secondary", Color) = (0.345098,0.3764706,0.3960785,0)
		_Color_Hair("Color_Hair", Color) = (0.2627451,0.2117647,0.1333333,0)
		_Color_Skin("Color_Skin", Color) = (1,0.8000001,0.682353,1)
		_Color_Stubble("Color_Stubble", Color) = (0.8039216,0.7019608,0.6313726,1)
		_Color_Scar("Color_Scar", Color) = (0.9294118,0.6862745,0.5921569,1)
		_Color_BodyArt("Color_BodyArt", Color) = (0.2283196,0.5822246,0.7573529,1)
		_Color_Eyes("Color_Eyes", Color) = (0.2283196,0.5822246,0.7573529,1)
		_Texture("Texture", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Emission("Emission", Range( 0 , 1)) = 0
		_BodyArt_Amount("BodyArt_Amount", Range( 0 , 1)) = 0
		[HideInInspector]_Mask_02("Mask_02", 2D) = "white" {}
		[HideInInspector]_Mask_05("Mask_05", 2D) = "white" {}
		[HideInInspector]_Mask_03("Mask_03", 2D) = "white" {}
		[HideInInspector]_Mask_04("Mask_04", 2D) = "white" {}
		[HideInInspector]_Mask_01("Mask_01", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color_BodyArt;
		uniform float4 _Color_Eyes;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float4 _Color_Primary;
		uniform sampler2D _Mask_01;
		uniform float4 _Mask_01_ST;
		uniform float4 _Color_Secondary;
		uniform float4 _Color_Leather_Primary;
		uniform sampler2D _Mask_04;
		uniform float4 _Mask_04_ST;
		uniform float4 _Color_Leather_Secondary;
		uniform float4 _Color_Metal_Primary;
		uniform sampler2D _Mask_02;
		uniform float4 _Mask_02_ST;
		uniform float4 _Color_Metal_Secondary;
		uniform float4 _Color_Metal_Dark;
		uniform float4 _Color_Hair;
		uniform float4 _Color_Skin;
		uniform sampler2D _Mask_03;
		uniform float4 _Mask_03_ST;
		uniform float4 _Color_Stubble;
		uniform float4 _Color_Scar;
		uniform sampler2D _Mask_05;
		uniform float4 _Mask_05_ST;
		uniform float _BodyArt_Amount;
		uniform float _Emission;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float2 uv_Mask_01 = i.uv_texcoord * _Mask_01_ST.xy + _Mask_01_ST.zw;
			float4 tex2DNode156 = tex2D( _Mask_01, uv_Mask_01, float2( 0,0 ), float2( 0,0 ) );
			float temp_output_25_0_g6 = 0.5;
			float temp_output_22_0_g6 = step( tex2DNode156.r , temp_output_25_0_g6 );
			float4 lerpResult35 = lerp( tex2D( _Texture, uv_Texture, float2( 0,0 ), float2( 0,0 ) ) , _Color_Primary , temp_output_22_0_g6);
			float temp_output_25_0_g3 = 0.5;
			float temp_output_22_0_g3 = step( tex2DNode156.g , temp_output_25_0_g3 );
			float4 lerpResult41 = lerp( lerpResult35 , _Color_Secondary , temp_output_22_0_g3);
			float2 uv_Mask_04 = i.uv_texcoord * _Mask_04_ST.xy + _Mask_04_ST.zw;
			float4 tex2DNode162 = tex2D( _Mask_04, uv_Mask_04, float2( 0,0 ), float2( 0,0 ) );
			float temp_output_25_0_g7 = 0.5;
			float temp_output_22_0_g7 = step( tex2DNode162.r , temp_output_25_0_g7 );
			float4 lerpResult45 = lerp( lerpResult41 , _Color_Leather_Primary , temp_output_22_0_g7);
			float temp_output_25_0_g9 = 0.5;
			float temp_output_22_0_g9 = step( tex2DNode162.g , temp_output_25_0_g9 );
			float4 lerpResult65 = lerp( lerpResult45 , _Color_Leather_Secondary , temp_output_22_0_g9);
			float2 uv_Mask_02 = i.uv_texcoord * _Mask_02_ST.xy + _Mask_02_ST.zw;
			float4 tex2DNode158 = tex2D( _Mask_02, uv_Mask_02, float2( 0,0 ), float2( 0,0 ) );
			float temp_output_25_0_g10 = 0.5;
			float temp_output_22_0_g10 = step( tex2DNode158.r , temp_output_25_0_g10 );
			float4 lerpResult124 = lerp( lerpResult65 , _Color_Metal_Primary , temp_output_22_0_g10);
			float temp_output_25_0_g11 = 0.5;
			float temp_output_22_0_g11 = step( tex2DNode158.g , temp_output_25_0_g11 );
			float4 lerpResult132 = lerp( lerpResult124 , _Color_Metal_Secondary , temp_output_22_0_g11);
			float temp_output_25_0_g12 = 0.5;
			float temp_output_22_0_g12 = step( tex2DNode158.b , temp_output_25_0_g12 );
			float4 lerpResult140 = lerp( lerpResult132 , _Color_Metal_Dark , temp_output_22_0_g12);
			float temp_output_25_0_g14 = 0.5;
			float temp_output_22_0_g14 = step( tex2DNode162.b , temp_output_25_0_g14 );
			float4 lerpResult49 = lerp( lerpResult140 , _Color_Hair , temp_output_22_0_g14);
			float2 uv_Mask_03 = i.uv_texcoord * _Mask_03_ST.xy + _Mask_03_ST.zw;
			float4 tex2DNode160 = tex2D( _Mask_03, uv_Mask_03, float2( 0,0 ), float2( 0,0 ) );
			float temp_output_25_0_g15 = 0.5;
			float temp_output_22_0_g15 = step( tex2DNode160.r , temp_output_25_0_g15 );
			float4 lerpResult53 = lerp( lerpResult49 , _Color_Skin , temp_output_22_0_g15);
			float temp_output_25_0_g16 = 0.5;
			float temp_output_22_0_g16 = step( tex2DNode160.b , temp_output_25_0_g16 );
			float4 lerpResult57 = lerp( lerpResult53 , _Color_Stubble , temp_output_22_0_g16);
			float temp_output_25_0_g18 = 0.5;
			float temp_output_22_0_g18 = step( tex2DNode160.g , temp_output_25_0_g18 );
			float4 lerpResult61 = lerp( lerpResult57 , _Color_Scar , temp_output_22_0_g18);
			float2 uv_Mask_05 = i.uv_texcoord * _Mask_05_ST.xy + _Mask_05_ST.zw;
			float4 tex2DNode179 = tex2D( _Mask_05, uv_Mask_05, float2( 0,0 ), float2( 0,0 ) );
			float4 lerpResult181 = lerp( _Color_Eyes , lerpResult61 , tex2DNode179.r);
			float4 temp_cast_0 = (tex2DNode156.b).xxxx;
			float4 color151 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float4 lerpResult152 = lerp( temp_cast_0 , color151 , ( 1.0 - _BodyArt_Amount ));
			float4 lerpResult69 = lerp( _Color_BodyArt , lerpResult181 , lerpResult152);
			o.Albedo = lerpResult69.rgb;
			float3 temp_cast_2 = (( ( 1.0 - tex2DNode179.r ) * _Emission )).xxx;
			o.Emission = temp_cast_2;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Universal Render Pipeline/Lit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
2580;41;2497;1374;-3261.371;778.5169;1.215;True;False
Node;AmplifyShaderEditor.SamplerNode;156;-583.4142,26.32014;Float;True;Property;_Mask_01;Mask_01;22;1;[HideInInspector];Create;True;0;0;True;0;4d2aa66f9d16bf644aeced5c66a39109;4d2aa66f9d16bf644aeced5c66a39109;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;162;730.0652,32.30579;Float;True;Property;_Mask_04;Mask_04;21;1;[HideInInspector];Create;True;0;0;True;0;49123ea06040fdb4e86c6fb0b4288c09;49123ea06040fdb4e86c6fb0b4288c09;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;39;186.6764,-155.1506;Float;False;MaskingFunction;-1;;3;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.FunctionNode;34;-279.8109,-167.2174;Float;False;MaskingFunction;-1;;6;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;37;-620.0018,-426.2611;Float;True;Property;_Texture;Texture;13;0;Create;True;0;0;False;0;7dd0476daa9e82447856b542d1d238eb;7dd0476daa9e82447856b542d1d238eb;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;90;518.2157,-189.2679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-281.0458,-343.9193;Float;False;Property;_Color_Primary;Color_Primary;0;0;Create;True;0;0;False;0;0.2431373,0.4196079,0.6196079,0;0.2352941,0.2352941,0.2352941,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;43;630.107,-150.5898;Float;False;MaskingFunction;-1;;7;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;158;78.78361,43.08949;Float;True;Property;_Mask_02;Mask_02;18;1;[HideInInspector];Create;True;0;0;True;0;f8e518b469cb2bd4c92100e04b7d4ab3;f8e518b469cb2bd4c92100e04b7d4ab3;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;95;958.2158,-182.2679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;89;427.2157,-246.2679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;64;1047.021,-152.0576;Float;False;MaskingFunction;-1;;9;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;35;30.50533,-421.9289;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;40;182.6165,-321.0611;Float;False;Property;_Color_Secondary;Color_Secondary;1;0;Create;True;0;0;False;0;0.8196079,0.6431373,0.2980392,0;0.7019608,0.6235294,0.4666667,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;96;865.2158,-224.2679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;44;626.047,-320.1301;Float;False;Property;_Color_Leather_Primary;Color_Leather_Primary;2;0;Create;True;0;0;False;0;0.282353,0.2078432,0.1647059,0;0.3088235,0.2139057,0.1589533,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;41;454.2986,-380.0913;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;98;1381.216,-179.2679;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;126;1482.28,-144.2758;Float;False;MaskingFunction;-1;;10;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.WireNode;97;1306.427,-223.7358;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;133;1914.29,-142.873;Float;False;MaskingFunction;-1;;11;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.WireNode;130;1810.771,-175.4864;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;63;1042.961,-321.5981;Float;False;Property;_Color_Leather_Secondary;Color_Leather_Secondary;4;0;Create;True;0;0;False;0;0.372549,0.3294118,0.2784314,0;0.372549,0.3294118,0.2784314,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;45;897.7284,-379.1605;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;125;1478.22,-313.8161;Float;False;Property;_Color_Metal_Primary;Color_Metal_Primary;3;0;Create;True;0;0;False;0;0.5960785,0.6117647,0.627451,0;0.6980392,0.6509804,0.6196079,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;131;1731.771,-218.4863;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;137;2242.782,-174.0837;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;65;1314.643,-380.6285;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;141;2343.135,-117.6013;Float;False;MaskingFunction;-1;;12;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.FunctionNode;47;2831.833,-127.9842;Float;False;MaskingFunction;-1;;14;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.WireNode;138;2163.782,-217.0837;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;124;1749.902,-372.8465;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;139;1910.23,-312.4134;Float;False;Property;_Color_Metal_Secondary;Color_Metal_Secondary;6;0;Create;True;0;0;False;0;0.345098,0.3764706,0.3960785,0;0.3921569,0.4039216,0.4117647,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;145;2678.965,-165.4909;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;102;3157.826,-162.8692;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;147;2346.414,-303.821;Float;False;Property;_Color_Metal_Dark;Color_Metal_Dark;5;0;Create;True;0;0;False;0;0.1764706,0.1960784,0.2156863,0;0.1660359,0.2190472,0.2720588,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;146;2599.965,-208.4908;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;132;2181.913,-371.4437;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;48;2827.773,-297.5243;Float;False;Property;_Color_Hair;Color_Hair;7;0;Create;True;0;0;False;0;0.2627451,0.2117647,0.1333333,0;0.2431373,0.2039216,0.145098,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;101;3074.826,-222.8692;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;160;2998.024,137.1288;Float;True;Property;_Mask_03;Mask_03;20;1;[HideInInspector];Create;True;0;0;True;0;83c8563e287501f4997504c772dd2387;83c8563e287501f4997504c772dd2387;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;140;2618.095,-362.8513;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;52;3303.302,-293.8943;Float;False;Property;_Color_Skin;Color_Skin;8;0;Create;True;0;0;False;0;1,0.8000001,0.682353,1;0.5647059,0.4078432,0.3137255,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;51;3307.362,-124.3543;Float;False;MaskingFunction;-1;;15;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;49;3099.455,-356.5544;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;53;3573.773,-352.9244;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;55;3755.367,-124.3551;Float;False;MaskingFunction;-1;;16;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;56;3751.307,-293.8953;Float;False;Property;_Color_Stubble;Color_Stubble;9;0;Create;True;0;0;False;0;0.8039216,0.7019608,0.6313726,1;0,1,0.793103,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;60;4194.171,-286.6357;Float;False;Property;_Color_Scar;Color_Scar;10;0;Create;True;0;0;False;0;0.9294118,0.6862745,0.5921569,1;0.6137929,0,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;148;4860.227,333.2149;Float;False;Property;_BodyArt_Amount;BodyArt_Amount;17;0;Create;True;0;0;False;0;0;0.3189544;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;59;4196.932,-109.2952;Float;False;MaskingFunction;-1;;18;571aab6f8c08f1c4d9bd4012d2958d88;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0.5;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;57;4021.78,-352.9254;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;176;5234.584,335.8315;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;151;5085.321,86.22977;Float;False;Constant;_Color0;Color 0;26;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;179;4625.897,-126.1729;Float;True;Property;_Mask_05;Mask_05;19;1;[HideInInspector];Create;True;0;0;True;0;24b7fe3ac6774f445a118f057e3eaaae;24b7fe3ac6774f445a118f057e3eaaae;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;180;4672.681,-301.6739;Float;False;Property;_Color_Eyes;Color_Eyes;12;0;Create;True;0;0;False;0;0.2283196,0.5822246,0.7573529,1;0,0,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;61;4464.645,-365.3574;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;152;5354.376,4.491408;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;185;5537.207,292.4267;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;5116,627.8245;Float;False;Property;_Emission;Emission;16;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;68;5032.728,-621.3527;Float;False;Property;_Color_BodyArt;Color_BodyArt;11;0;Create;True;0;0;False;0;0.2283196,0.5822246,0.7573529,1;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;181;5028.881,-379.6749;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;5559.291,408.1251;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;5115.983,434.9715;Float;False;Property;_Metallic;Metallic;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;69;5492.444,-373.4669;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;154;5117.737,514.2782;Float;False;Property;_Smoothness;Smoothness;15;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5767.304,-306.0844;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SyntyStudios/CustomCharacter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;39;21;156;2
WireConnection;34;21;156;1
WireConnection;90;0;39;32
WireConnection;43;21;162;1
WireConnection;95;0;43;32
WireConnection;89;0;90;0
WireConnection;64;21;162;2
WireConnection;35;0;37;0
WireConnection;35;1;33;0
WireConnection;35;2;34;32
WireConnection;96;0;95;0
WireConnection;41;0;35;0
WireConnection;41;1;40;0
WireConnection;41;2;89;0
WireConnection;98;0;64;32
WireConnection;126;21;158;1
WireConnection;97;0;98;0
WireConnection;133;21;158;2
WireConnection;130;0;126;32
WireConnection;45;0;41;0
WireConnection;45;1;44;0
WireConnection;45;2;96;0
WireConnection;131;0;130;0
WireConnection;137;0;133;32
WireConnection;65;0;45;0
WireConnection;65;1;63;0
WireConnection;65;2;97;0
WireConnection;141;21;158;3
WireConnection;47;21;162;3
WireConnection;138;0;137;0
WireConnection;124;0;65;0
WireConnection;124;1;125;0
WireConnection;124;2;131;0
WireConnection;145;0;141;32
WireConnection;102;0;47;32
WireConnection;146;0;145;0
WireConnection;132;0;124;0
WireConnection;132;1;139;0
WireConnection;132;2;138;0
WireConnection;101;0;102;0
WireConnection;140;0;132;0
WireConnection;140;1;147;0
WireConnection;140;2;146;0
WireConnection;51;21;160;1
WireConnection;49;0;140;0
WireConnection;49;1;48;0
WireConnection;49;2;101;0
WireConnection;53;0;49;0
WireConnection;53;1;52;0
WireConnection;53;2;51;32
WireConnection;55;21;160;3
WireConnection;59;21;160;2
WireConnection;57;0;53;0
WireConnection;57;1;56;0
WireConnection;57;2;55;32
WireConnection;176;0;148;0
WireConnection;61;0;57;0
WireConnection;61;1;60;0
WireConnection;61;2;59;32
WireConnection;152;0;156;3
WireConnection;152;1;151;0
WireConnection;152;2;176;0
WireConnection;185;0;179;1
WireConnection;181;0;180;0
WireConnection;181;1;61;0
WireConnection;181;2;179;1
WireConnection;183;0;185;0
WireConnection;183;1;184;0
WireConnection;69;0;68;0
WireConnection;69;1;181;0
WireConnection;69;2;152;0
WireConnection;0;0;69;0
WireConnection;0;2;183;0
WireConnection;0;3;165;0
WireConnection;0;4;154;0
ASEEND*/
//CHKSM=B841E171BA9C13399059DE606CF2609D9912FC24