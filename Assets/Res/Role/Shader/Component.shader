// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Component"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.02
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		_SpeShaTex("SpeShaTex", 2D) = "white" {}
		_Shadowcolor("Shadowcolor", Color) = (0.9245283,0.1351905,0.1351905,0)
		_ShadowRange("ShadowRange", Range( 0 , 1)) = 0.5
		_ShadowIntensity("ShadowIntensity", Range( 0 , 1)) = 0.7422851
		_HiglightRange("HiglightRange", Range( 0 , 1)) = 0.5
		_HiglightIntensity("HiglightIntensity", Range( 0 , 1)) = 0.7422851
		_Higlightcolor("Higlightcolor", Color) = (0.9245283,0.1351905,0.1351905,0)
		_Higlight02Range("Higlight02Range", Range( 0 , 1)) = 0.5
		_Higlight02Intensity("Higlight02Intensity", Range( 0 , 1)) = 0
		_Highlight03Range("Highlight03Range", Range( 0 , 1)) = 0.5
		_Highlight03Intensity("Highlight03Intensity", Range( 0 , 1)) = 0
		[HDR]_Emissioncolor("Emissioncolor", Color) = (0.9245283,0.1351905,0.1351905,0)
		[HDR]_Aldobe_color("Aldobe_color", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			float3 viewDir;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _SpeShaTex;
		uniform float4 _SpeShaTex_ST;
		uniform float4 _Emissioncolor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _ShadowRange;
		uniform float _ShadowIntensity;
		uniform float4 _Shadowcolor;
		uniform float _Higlight02Range;
		uniform float _Higlight02Intensity;
		uniform float _HiglightRange;
		uniform float _HiglightIntensity;
		uniform float4 _Higlightcolor;
		uniform float _Highlight03Range;
		uniform float _Highlight03Intensity;
		uniform float4 _Aldobe_color;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_SpeShaTex = i.uv_texcoord * _SpeShaTex_ST.xy + _SpeShaTex_ST.zw;
			float4 temp_cast_1 = (tex2D( _SpeShaTex, uv_SpeShaTex ).g).xxxx;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			float4 blendOpSrc16 = temp_cast_1;
			float4 blendOpDest16 = tex2DNode1;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = i.worldNormal;
			float dotResult38 = dot( ase_worldlightDir , ase_worldNormal );
			float ifLocalVar43 = 0;
			if( dotResult38 > _ShadowRange )
				ifLocalVar43 = 1.0;
			else if( dotResult38 < _ShadowRange )
				ifLocalVar43 = _ShadowIntensity;
			float ifLocalVar143 = 0;
			if( dotResult38 >= _ShadowRange )
				ifLocalVar143 = 1.0;
			else
				ifLocalVar143 = 0.0;
			float4 tex2DNode64 = tex2D( _SpeShaTex, uv_SpeShaTex );
			float dotResult49 = dot( i.viewDir , ase_worldNormal );
			float ifLocalVar52 = 0;
			if( dotResult49 >= _Higlight02Range )
				ifLocalVar52 = _Higlight02Intensity;
			else
				ifLocalVar52 = 0.0;
			float ifLocalVar67 = 0;
			if( dotResult38 >= _HiglightRange )
				ifLocalVar67 = _HiglightIntensity;
			else
				ifLocalVar67 = 0.0;
			float3 temp_cast_2 = (i.viewDir.z).xxx;
			float dotResult94 = dot( temp_cast_2 , ase_worldNormal );
			float dotResult133 = dot( i.viewDir , ase_worldNormal );
			float ifLocalVar102 = 0;
			if( ( ( 1.0 - dotResult94 ) * pow( ( 1.0 - saturate( ( dotResult133 + 0.5 ) ) ) , 1.762025 ) ) <= _Highlight03Range )
				ifLocalVar102 = 0.0;
			else
				ifLocalVar102 = _Highlight03Intensity;
			c.rgb = ( ( ( ( ( saturate( (( blendOpDest16 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest16 ) * ( 1.0 - blendOpSrc16 ) ) : ( 2.0 * blendOpDest16 * blendOpSrc16 ) ) )) * ifLocalVar43 ) + ( ( 1.0 - ifLocalVar43 ) * _Shadowcolor * tex2DNode1 ) ) + ( ifLocalVar143 * ( tex2DNode64.r * ifLocalVar52 ) * tex2DNode1 ) + ( ifLocalVar143 * ( ( ifLocalVar67 * tex2DNode64.r ) * _Higlightcolor ) ) + ( tex2D( _SpeShaTex, uv_SpeShaTex ).r * ifLocalVar102 ) ) * _Aldobe_color ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float2 uv_SpeShaTex = i.uv_texcoord * _SpeShaTex_ST.xy + _SpeShaTex_ST.zw;
			o.Emission = ( tex2D( _SpeShaTex, uv_SpeShaTex ).b * _Emissioncolor ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16800
216;237;1283;687;1615.426;753.9897;6.239003;True;False
Node;AmplifyShaderEditor.WorldNormalVector;96;-614.6974,1688.135;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;95;-600.6807,1488.462;Float;True;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;133;-249.0551,1924.914;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-161.3109,2206.384;Float;False;Constant;_RimOffset;Rim Offset;4;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;138;46.68903,2094.384;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;137;206.689,2094.384;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;35;-763.3257,526.8215;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;37;-737.1863,718.3255;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;70;302.2264,714.4833;Float;False;Property;_HiglightIntensity;HiglightIntensity;6;0;Create;True;0;0;False;0;0.7422851;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-169.1472,326.6142;Float;False;Property;_ShadowIntensity;ShadowIntensity;4;0;Create;True;0;0;False;0;0.7422851;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;388.3627,797.0745;Float;False;Constant;_Float7;Float 7;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-125.8476,230.0401;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;340.9315,535.7409;Float;False;Property;_HiglightRange;HiglightRange;5;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;94;-174.9043,1571.656;Float;True;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;184.7337,2372.991;Float;False;Constant;_RimPower;Rim Power;3;0;Create;True;0;0;False;0;1.762025;1.762025;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;38;-468.4101,571.1894;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-181.5394,160.614;Float;False;Property;_ShadowRange;ShadowRange;3;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;46;-607.3698,1307.644;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;135;380.6056,2069.383;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;53;-628.3527,1089.218;Float;True;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;51;57.39393,1189.172;Float;False;Property;_Higlight02Range;Higlight02Range;8;0;Create;True;0;0;False;0;0.5;0.73;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;43;185.6221,175.8771;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-763.9962,-56.08398;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;5482ccfd8bb925b489eb7daefc9c2fce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;134;580.2761,2066.636;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-500.9089,-280.9226;Float;True;Property;_SpeShaTex;SpeShaTex;1;0;Create;True;0;0;False;0;None;6f335da7948fd8443bd874ef24b30e21;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;67;720.9083,651.6256;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;130.2202,1352.383;Float;False;Constant;_Float5;Float 5;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;64;431.3862,948.0719;Float;True;Global;TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;4;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;71.33848,1265.739;Float;False;Property;_Higlight02Intensity;Higlight02Intensity;9;0;Create;True;0;0;False;0;0;0.36;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;141;225.8903,1573.087;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;49;-346.0792,1171.846;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;82;692.8463,169.1213;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;1137.572,684.3603;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;86;1306.914,867.5241;Float;False;Property;_Higlightcolor;Higlightcolor;7;0;Create;True;0;0;False;0;0.9245283,0.1351905,0.1351905,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;16;-137.3354,-95.80046;Float;False;Overlay;True;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;99;1101.063,1635.025;Float;False;Property;_Highlight03Range;Highlight03Range;10;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;1028.126,1761.1;Float;False;Property;_Highlight03Intensity;Highlight03Intensity;11;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;80;730.1162,400.9259;Float;False;Property;_Shadowcolor;Shadowcolor;2;0;Create;True;0;0;False;0;0.9245283,0.1351905,0.1351905,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;785.2468,1568.192;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-116.2228,602.9317;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;52;538.2012,1184.142;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;1161.904,1902.345;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;143;163.95,455.1675;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;102;1569.001,1572.46;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;477.9971,-94.98383;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;993.6078,167.5416;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;1582.834,689.7803;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;149;1370.756,1249.315;Float;True;Global;TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;4;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;826.9821,976.6838;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;1944.742,1390.42;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;1187.592,369.262;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;1758.303,487.57;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;1230.961,-93.73037;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;2014.292,297.34;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;159;2034.327,479.6254;Float;False;Property;_Aldobe_color;Aldobe_color;13;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;93;1665.008,-379.2084;Float;True;Global;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;4;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;92;1688.631,-120.5354;Float;False;Property;_Emissioncolor;Emissioncolor;12;1;[HDR];Create;True;0;0;False;0;0.9245283,0.1351905,0.1351905,0;11.98431,5.019608,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;2270.327,400.6253;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;154;280.6084,-259.6376;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;2149.898,-189.8726;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1462.247,1182.973;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Component;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.02;0,0,0,0;VertexOffset;True;False;Spherical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;133;0;95;0
WireConnection;133;1;96;0
WireConnection;138;0;133;0
WireConnection;138;1;139;0
WireConnection;137;0;138;0
WireConnection;94;0;95;3
WireConnection;94;1;96;0
WireConnection;38;0;35;0
WireConnection;38;1;37;0
WireConnection;135;0;137;0
WireConnection;43;0;38;0
WireConnection;43;1;42;0
WireConnection;43;2;44;0
WireConnection;43;4;45;0
WireConnection;134;0;135;0
WireConnection;134;1;136;0
WireConnection;67;0;38;0
WireConnection;67;1;68;0
WireConnection;67;2;70;0
WireConnection;67;3;70;0
WireConnection;67;4;69;0
WireConnection;141;0;94;0
WireConnection;49;0;53;0
WireConnection;49;1;46;0
WireConnection;82;0;43;0
WireConnection;152;0;67;0
WireConnection;152;1;64;1
WireConnection;16;0;4;2
WireConnection;16;1;1;0
WireConnection;140;0;141;0
WireConnection;140;1;134;0
WireConnection;52;0;49;0
WireConnection;52;1;51;0
WireConnection;52;2;48;0
WireConnection;52;3;48;0
WireConnection;52;4;50;0
WireConnection;143;0;38;0
WireConnection;143;1;42;0
WireConnection;143;2;44;0
WireConnection;143;3;44;0
WireConnection;143;4;146;0
WireConnection;102;0;140;0
WireConnection;102;1;99;0
WireConnection;102;2;101;0
WireConnection;102;3;100;0
WireConnection;102;4;100;0
WireConnection;39;0;16;0
WireConnection;39;1;43;0
WireConnection;81;0;82;0
WireConnection;81;1;80;0
WireConnection;81;2;1;0
WireConnection;88;0;152;0
WireConnection;88;1;86;0
WireConnection;65;0;64;1
WireConnection;65;1;52;0
WireConnection;150;0;149;1
WireConnection;150;1;102;0
WireConnection;144;0;143;0
WireConnection;144;1;65;0
WireConnection;144;2;1;0
WireConnection;148;0;143;0
WireConnection;148;1;88;0
WireConnection;85;0;39;0
WireConnection;85;1;81;0
WireConnection;54;0;85;0
WireConnection;54;1;144;0
WireConnection;54;2;148;0
WireConnection;54;3;150;0
WireConnection;160;0;54;0
WireConnection;160;1;159;0
WireConnection;91;0;93;3
WireConnection;91;1;92;0
WireConnection;0;2;91;0
WireConnection;0;13;160;0
ASEEND*/
//CHKSM=5F3494847806BD1E1E2A8AC6220FBAA9D9D5C110