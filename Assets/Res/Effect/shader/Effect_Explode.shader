// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Effect/Explode"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Albedo02("Albedo02", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,0)
		[HDR]_Color1("Color 1", Color) = (1,1,1,0)
		[HDR]_Color2("Color 2", Color) = (1,1,1,0)
		_Float1("Float 1", Range( 0 , 2)) = 0.3781823
		_Aldobe03("Aldobe03", 2D) = "white" {}
		_Float5("Float 5", Range( 0 , 0.1)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex3coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv0_Albedo = v.texcoord.xy * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2Dlod( _Albedo, float4( uv0_Albedo, 0, 0.0) );
			float Float0353 = v.texcoord1.xyz.z;
			float outlineVar = ( _Float5 + ( tex2DNode1 * Float0353 ) ).r;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 color57 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float2 uv0_Aldobe03 = i.uv_texcoord * _Aldobe03_ST.xy + _Aldobe03_ST.zw;
			float Float0250 = i.uv2_tex3coord2.y;
			float clampResult48 = clamp( step( tex2D( _Aldobe03, uv0_Aldobe03 ).r , Float0250 ) , 0.0 , 1.0 );
			float myVarName58 = clampResult48;
			o.Emission = color57.rgb;
			clip( myVarName58 - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float3 uv2_tex3coord2;
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

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Aldobe03;
		uniform float4 _Aldobe03_ST;
		uniform float4 _Color2;
		uniform float4 _Color1;
		uniform float4 _Color0;
		uniform sampler2D _Albedo02;
		uniform float4 _Albedo02_ST;
		uniform float _Float1;
		uniform float _Cutoff = 0.5;
		uniform float _Float5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv0_Albedo = v.texcoord.xy * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2Dlod( _Albedo, float4( uv0_Albedo, 0, 0.0) );
			float3 ase_vertexNormal = v.normal.xyz;
			float Float0353 = v.texcoord1.xyz.z;
			v.vertex.xyz += (( tex2DNode1 * float4( ase_vertexNormal , 0.0 ) * Float0353 ) ).rgb;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv0_Aldobe03 = i.uv_texcoord * _Aldobe03_ST.xy + _Aldobe03_ST.zw;
			float Float0250 = i.uv2_tex3coord2.y;
			float clampResult48 = clamp( step( tex2D( _Aldobe03, uv0_Aldobe03 ).r , Float0250 ) , 0.0 , 1.0 );
			float myVarName58 = clampResult48;
			float4 color0325 = _Color2;
			float4 color0216 = _Color1;
			float4 color0115 = _Color0;
			float Float0139 = i.uv2_tex3coord2.x;
			float4 temp_cast_0 = (Float0139).xxxx;
			float4 temp_cast_1 = (( Float0139 - 0.1 )).xxxx;
			float2 uv0_Albedo02 = i.uv_texcoord * _Albedo02_ST.xy + _Albedo02_ST.zw;
			float4 Var0113 = tex2D( _Albedo02, uv0_Albedo02 );
			float4 smoothstepResult32 = smoothstep( temp_cast_0 , temp_cast_1 , ( Var0113 * _Float1 ));
			float4 clampResult26 = clamp( smoothstepResult32 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 lerpResult7 = lerp( color0216 , color0115 , clampResult26);
			float4 temp_cast_2 = (Float0139).xxxx;
			float4 temp_cast_3 = (( Float0139 - 0.1 )).xxxx;
			float4 smoothstepResult35 = smoothstep( temp_cast_2 , temp_cast_3 , Var0113);
			float4 clampResult34 = clamp( smoothstepResult35 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 lerpResult27 = lerp( color0325 , lerpResult7 , clampResult34);
			c.rgb = lerpResult27.rgb;
			c.a = 1;
			clip( myVarName58 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float3 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyz = customInputData.uv2_tex3coord2;
				o.customPack2.xyz = v.texcoord1;
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
				surfIN.uv2_tex3coord2 = IN.customPack2.xyz;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
171;228;1092;605;3613.162;1241.529;2.622174;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;65;-2020.363,481.2354;Float;False;1;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2224.547,243.1441;Float;False;0;9;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-863.121,206.4226;Float;False;0;43;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-1738.47,594.3892;Float;False;Float02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-1922.336,219.6387;Float;True;Property;_Albedo02;Albedo02;2;0;Create;True;0;0;False;0;None;b44ce1632fbe1144aaf51e423b05f8e8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;51;-366.3661,452.3005;Float;False;50;Float02;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-480.8178,183.4816;Float;True;Property;_Aldobe03;Aldobe03;9;0;Create;True;0;0;False;0;None;91a9366a83dd353408371945d0bce120;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1746.855,479.6866;Float;False;Float01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1580.958,218.55;Float;False;Var01;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;45;-117.964,399.0955;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-904.6183,-227.489;Float;False;39;Float01;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1076.281,-309.9243;Float;False;Property;_Float1;Float 1;8;0;Create;True;0;0;False;0;0.3781823;1.1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-504.0762,865.1661;Float;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-1740.706,693.7927;Float;False;Float03;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-1025.556,-514.4249;Float;True;13;Var01;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-2058.535,-318.6179;Float;False;Property;_Color1;Color 1;4;1;[HDR];Create;True;0;0;False;0;1,1,1,0;2,0.1253506,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;48;78.30633,394.974;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-6.358483,1322.821;Float;False;53;Float03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-721.4908,-727.9218;Float;False;39;Float01;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-217.4913,846.1696;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;b44ce1632fbe1144aaf51e423b05f8e8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-718.1021,-324.6017;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-691.7154,-127.0022;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-2059.937,-504.3224;Float;False;Property;_Color0;Color 0;3;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1.498039,1.270588,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-2012.424,-100.7404;Float;False;Property;_Color2;Color 2;6;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0.4622642,0.4622642,0.4622642,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-1807.063,-323.571;Float;False;color02;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;414.46,871.0093;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-598.7673,-801.9299;Float;False;13;Var01;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;260.5367,717.419;Float;False;Property;_Float5;Float 5;12;0;Create;True;0;0;False;0;0.1;0.05;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;32;-504.1139,-249.5642;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;317.9843,393.7618;Float;True;myVarName;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;36;-484.8002,-650.5853;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-1812.617,-502.4089;Float;False;color01;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;35;-277.8096,-794.4279;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1746.745,-89.711;Float;False;color03;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-257.3641,-269.9934;Float;False;15;color01;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;55;7.257047,1129.521;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;18;-261.7258,-348.3519;Float;False;16;color02;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;667.8119,838.0621;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;26;-253.1801,-164.3087;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;369.0482,638.0421;Float;False;58;myVarName;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;57;533.7001,472.6981;Float;False;Constant;_Color3;Color 3;11;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OutlineNode;56;836.5967,612.528;Float;False;0;True;Masked;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;351.8306,1139.299;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;34;-17.72788,-584.8201;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;108.0713,-733.1473;Float;False;25;color03;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;7;-20.75849,-308.1925;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2362.121,477.4509;Float;False;Property;_Float0;Float 0;7;0;Create;True;0;0;False;0;0.6240879;0.872;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;1100.968,1040.333;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;72;1281.501,-918.3156;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;27;277.6817,-612.644;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;71;995.9811,-755.8311;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-4;False;4;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2355.511,571.5654;Float;False;Property;_Float3;Float 3;10;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;1878.047,-919.0765;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2283.549,655.6545;Float;False;Property;_Float4;Float 4;11;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;74;1522.547,-776.1024;Float;True;Property;_BurnRamp;Burn Ramp;5;0;Create;True;0;0;False;0;None;64e7766099ad46747a07014e44d0aea1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1329.835,209.2174;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Effect/Explode;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;50;0;65;2
WireConnection;9;1;11;0
WireConnection;43;1;46;0
WireConnection;39;0;65;1
WireConnection;13;0;9;0
WireConnection;45;0;43;1
WireConnection;45;1;51;0
WireConnection;53;0;65;3
WireConnection;48;0;45;0
WireConnection;1;1;2;0
WireConnection;22;0;19;0
WireConnection;22;1;23;0
WireConnection;33;0;42;0
WireConnection;16;0;6;0
WireConnection;60;0;1;0
WireConnection;60;1;54;0
WireConnection;32;0;22;0
WireConnection;32;1;42;0
WireConnection;32;2;33;0
WireConnection;58;0;48;0
WireConnection;36;0;41;0
WireConnection;15;0;5;0
WireConnection;35;0;31;0
WireConnection;35;1;41;0
WireConnection;35;2;36;0
WireConnection;25;0;24;0
WireConnection;63;0;61;0
WireConnection;63;1;60;0
WireConnection;26;0;32;0
WireConnection;56;0;57;0
WireConnection;56;2;59;0
WireConnection;56;1;63;0
WireConnection;3;0;1;0
WireConnection;3;1;55;0
WireConnection;3;2;54;0
WireConnection;34;0;35;0
WireConnection;7;0;18;0
WireConnection;7;1;17;0
WireConnection;7;2;26;0
WireConnection;62;0;56;0
WireConnection;62;1;3;0
WireConnection;72;0;71;0
WireConnection;27;0;28;0
WireConnection;27;1;7;0
WireConnection;27;2;34;0
WireConnection;75;0;72;0
WireConnection;75;1;74;0
WireConnection;74;1;72;0
WireConnection;0;10;58;0
WireConnection;0;13;27;0
WireConnection;0;11;62;0
ASEEND*/
//CHKSM=CF0C87AC53E4190079EBD1DC9AA1588043E87068