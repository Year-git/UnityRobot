// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SampleShaders/CustomLightingToon"
{
	Properties
	{
		_AlbedoMap01("AlbedoMap01", 2D) = "white" {}
		_AlbedoMap03("AlbedoMap03", 2D) = "white" {}
		_AlbedoMap02("AlbedoMap02", 2D) = "white" {}
		_Control("Control", 2D) = "white" {}
		_Normal01("Normal01", 2D) = "bump" {}
		_Normal02("Normal02", 2D) = "bump" {}
		_Normal03("Normal03", 2D) = "bump" {}
		_Smooth03("Smooth03", Range( 0 , 2)) = 1
		_Smooth02("Smooth02", Range( 0 , 2)) = 1
		_Float0("Float 0", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
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

		uniform sampler2D _AlbedoMap01;
		uniform float4 _AlbedoMap01_ST;
		uniform sampler2D _Normal01;
		uniform float4 _Normal01_ST;
		uniform float _Float0;
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _AlbedoMap02;
		uniform float4 _AlbedoMap02_ST;
		uniform sampler2D _Normal02;
		uniform float4 _Normal02_ST;
		uniform float _Smooth02;
		uniform sampler2D _AlbedoMap03;
		uniform float4 _AlbedoMap03_ST;
		uniform sampler2D _Normal03;
		uniform float4 _Normal03_ST;
		uniform float _Smooth03;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			SurfaceOutputStandard s202 = (SurfaceOutputStandard ) 0;
			float2 uv_AlbedoMap01 = i.uv_texcoord * _AlbedoMap01_ST.xy + _AlbedoMap01_ST.zw;
			s202.Albedo = tex2D( _AlbedoMap01, uv_AlbedoMap01 ).rgb;
			float2 uv_Normal01 = i.uv_texcoord * _Normal01_ST.xy + _Normal01_ST.zw;
			s202.Normal = WorldNormalVector( i , UnpackNormal( tex2D( _Normal01, uv_Normal01 ) ) );
			s202.Emission = float3( 0,0,0 );
			s202.Metallic = 0.0;
			s202.Smoothness = _Float0;
			s202.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi202 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g202 = UnityGlossyEnvironmentSetup( s202.Smoothness, data.worldViewDir, s202.Normal, float3(0,0,0));
			gi202 = UnityGlobalIllumination( data, s202.Occlusion, s202.Normal, g202 );
			#endif

			float3 surfResult202 = LightingStandard ( s202, viewDir, gi202 ).rgb;
			surfResult202 += s202.Emission;

			#ifdef UNITY_PASS_FORWARDADD//202
			surfResult202 -= s202.Emission;
			#endif//202
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode186 = tex2D( _Control, uv_Control );
			SurfaceOutputStandard s225 = (SurfaceOutputStandard ) 0;
			float2 uv_AlbedoMap02 = i.uv_texcoord * _AlbedoMap02_ST.xy + _AlbedoMap02_ST.zw;
			s225.Albedo = tex2D( _AlbedoMap02, uv_AlbedoMap02 ).rgb;
			float2 uv_Normal02 = i.uv_texcoord * _Normal02_ST.xy + _Normal02_ST.zw;
			s225.Normal = WorldNormalVector( i , UnpackNormal( tex2D( _Normal02, uv_Normal02 ) ) );
			s225.Emission = float3( 0,0,0 );
			s225.Metallic = 0.0;
			s225.Smoothness = _Smooth02;
			s225.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi225 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g225 = UnityGlossyEnvironmentSetup( s225.Smoothness, data.worldViewDir, s225.Normal, float3(0,0,0));
			gi225 = UnityGlobalIllumination( data, s225.Occlusion, s225.Normal, g225 );
			#endif

			float3 surfResult225 = LightingStandard ( s225, viewDir, gi225 ).rgb;
			surfResult225 += s225.Emission;

			#ifdef UNITY_PASS_FORWARDADD//225
			surfResult225 -= s225.Emission;
			#endif//225
			SurfaceOutputStandard s228 = (SurfaceOutputStandard ) 0;
			float2 uv_AlbedoMap03 = i.uv_texcoord * _AlbedoMap03_ST.xy + _AlbedoMap03_ST.zw;
			s228.Albedo = tex2D( _AlbedoMap03, uv_AlbedoMap03 ).rgb;
			float2 uv_Normal03 = i.uv_texcoord * _Normal03_ST.xy + _Normal03_ST.zw;
			s228.Normal = WorldNormalVector( i , UnpackNormal( tex2D( _Normal03, uv_Normal03 ) ) );
			s228.Emission = float3( 0,0,0 );
			s228.Metallic = 0.0;
			s228.Smoothness = _Smooth03;
			s228.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi228 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g228 = UnityGlossyEnvironmentSetup( s228.Smoothness, data.worldViewDir, s228.Normal, float3(0,0,0));
			gi228 = UnityGlobalIllumination( data, s228.Occlusion, s228.Normal, g228 );
			#endif

			float3 surfResult228 = LightingStandard ( s228, viewDir, gi228 ).rgb;
			surfResult228 += s228.Emission;

			#ifdef UNITY_PASS_FORWARDADD//228
			surfResult228 -= s228.Emission;
			#endif//228
			c.rgb = ( ( surfResult202 * tex2DNode186.r ) + ( tex2DNode186.g * surfResult225 ) + ( surfResult228 * tex2DNode186.b ) );
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
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
-729;303;652;463;-1390.397;639.581;1;True;False
Node;AmplifyShaderEditor.SamplerNode;172;1387.848,-880.1281;Float;True;Property;_AlbedoMap01;AlbedoMap01;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;232;1873.602,1645.689;Float;False;Property;_Smooth03;Smooth03;7;0;Create;True;0;0;False;0;1;1.186;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1515.77,745.5599;Float;False;Property;_Smooth02;Smooth02;8;0;Create;True;0;0;False;0;1;1.186;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;200;1303.744,-546.2466;Float;True;Property;_Normal01;Normal01;4;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;234;1574.396,-389.581;Float;False;Property;_Float0;Float 0;9;0;Create;True;0;0;False;0;1;1.186;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;229;1513.625,1123.31;Float;True;Property;_AlbedoMap03;AlbedoMap03;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;226;1196.732,323.9067;Float;True;Property;_AlbedoMap02;AlbedoMap02;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;227;1221.797,607.1973;Float;True;Property;_Normal02;Normal02;5;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;230;1429.521,1457.192;Float;True;Property;_Normal03;Normal03;6;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;186;978.7888,-53.87498;Float;True;Property;_Control;Control;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomStandardSurface;228;1952.594,1360.708;Float;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomStandardSurface;202;1826.817,-642.73;Float;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomStandardSurface;225;1744.87,510.7139;Float;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;2406.291,-478.8757;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;2396.611,547.2515;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;2394.579,173.318;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;219;2955.874,26.48246;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;104;3263.772,-203.5246;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;SampleShaders/CustomLightingToon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;228;0;229;0
WireConnection;228;1;230;0
WireConnection;228;4;232;0
WireConnection;202;0;172;0
WireConnection;202;1;200;0
WireConnection;202;4;234;0
WireConnection;225;0;226;0
WireConnection;225;1;227;0
WireConnection;225;4;233;0
WireConnection;208;0;202;0
WireConnection;208;1;186;1
WireConnection;221;0;228;0
WireConnection;221;1;186;3
WireConnection;210;0;186;2
WireConnection;210;1;225;0
WireConnection;219;0;208;0
WireConnection;219;1;210;0
WireConnection;219;2;221;0
WireConnection;104;13;219;0
ASEEND*/
//CHKSM=616391F74010500F3560B572E51FAE8148CC9CA5