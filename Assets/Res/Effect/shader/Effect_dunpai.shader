// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Effects/dunpai"
{
	Properties
	{
		[HDR]_Aldobe_color("Aldobe_color", Color) = (0,0,0,0)
		_Aldobe("Aldobe", 2D) = "white" {}
		[HDR]_Depthfabe_color("Depthfabe_color", Color) = (0,0,0,0)
		_Depthfade("Depthfade", Range( 0 , 1)) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.3764713
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
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

		uniform sampler2D _Aldobe;
		uniform float4 _Aldobe_ST;
		uniform float _Opacity;
		uniform float4 _Aldobe_color;
		uniform float4 _Depthfabe_color;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depthfade;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_Aldobe = i.uv_texcoord * _Aldobe_ST.xy + _Aldobe_ST.zw;
			float4 tex2DNode1 = tex2D( _Aldobe, uv_Aldobe );
			float temp_output_13_0 = ( ( 1.0 - _Time.y ) * 0.5 );
			float clampResult38 = clamp( (-1.0 + (tex2DNode1.g - 0.0) * ((0.5 + (sin( ( temp_output_13_0 * 3.0 ) ) - -1.0) * (1.0 - 0.5) / (1.0 - -1.0)) - -1.0) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float clampResult53 = clamp( ( clampResult38 - tex2DNode1.b ) , 0.0 , 1.0 );
			float clampResult124 = clamp( tex2DNode1.b , 0.0 , 0.5 );
			float temp_output_36_0 = (0.0 + (tex2DNode1.r - 0.0) * (0.2 - 0.0) / (1.0 - 0.0));
			float clampResult57 = clamp( ( temp_output_36_0 - tex2DNode1.b ) , 0.0 , 1.0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth111 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth111 = abs( ( screenDepth111 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depthfade ) );
			float clampResult118 = clamp( ( 1.0 - pow( ( distanceDepth111 - 0.7973949 ) , 0.6661849 ) ) , 0.0 , 1.0 );
			float4 lerpResult106 = lerp( _Aldobe_color , _Depthfabe_color , clampResult118);
			c.rgb = lerpResult106.rgb;
			c.a = ( clampResult53 + clampResult124 + clampResult57 + ( tex2DNode1.a - _Opacity ) );
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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
-1126;271;980;526;2044.274;1006.018;5.554666;True;False
Node;AmplifyShaderEditor.TimeNode;12;-2155.86,-329.4557;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-2135.952,-103.0098;Float;False;Constant;_WaceSoeed;WaceSoeed;0;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;86;-1920.981,-305.8489;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1723.786,-207.6122;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1772.638,-1.898604;Float;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;3;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1579.879,-132.3061;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;18;-1430.702,-60.88094;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;862.1647,13.6082;Float;False;Property;_Depthfade;Depthfade;3;0;Create;True;0;0;False;0;1;0.0382353;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;111;1235.522,-5.816422;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-502.0632,680.565;Float;True;Property;_Aldobe;Aldobe;1;0;Create;True;0;0;False;0;None;e6201927fb4577442817ef9cae68b3ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-361.8301,481.0333;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;1165.763,115.0863;Float;False;Constant;_Float17;Float 17;4;0;Create;True;0;0;False;0;0.7973949;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-367.0897,407.249;Float;False;Constant;_Float2;Float 2;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;-1249.277,-6.976534;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-358.7897,323.2489;Float;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-316.0429,926.8679;Float;False;Constant;_Float10;Float 10;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-321.2376,1168.642;Float;False;Constant;_Float8;Float 8;1;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;113;1527.891,98.90237;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-309.2376,1091.642;Float;False;Constant;_Float11;Float 11;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-325.2376,1006.642;Float;False;Constant;_Float9;Float 9;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;2;-164.6428,418.5648;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;1442.253,198.6249;Float;False;Constant;_Float18;Float 18;5;0;Create;True;0;0;False;0;0.6661849;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;115;1793.981,95.17412;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;36;-13.42399,919.7397;Float;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;-1;False;3;FLOAT;1;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;38;237.7541,417.8176;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;52;653.764,413.4983;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;1130.017,900.9295;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;0.3764713;0.291;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;112;1969.828,-37.39993;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;56;628.6008,848.5076;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;82;2055.019,-246.7609;Float;False;Property;_Depthfabe_color;Depthfabe_color;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;766.9961,630.4628,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;121;1402.318,737.2317;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;78;2121.307,-499.1648;Float;False;Property;_Aldobe_color;Aldobe_color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1.059274,0.687696,0.3383021,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;53;1082.9,405.0311;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;124;887.6616,640.282;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;118;2177.899,-33.59474;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;57;1002.374,829.238;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;48;134.5685,-475.2854;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-2730.037,-793.0062;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;455.3287,1138.624;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2394.802,-812.8;Float;False;Constant;_Float4;Float 4;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;2047.208,571.5303;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-168.3215,-394.5769;Float;False;Constant;_Float12;Float 12;1;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2394.802,-652.8;Float;False;Constant;_Float6;Float 6;1;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;50;-64.71593,59.2122;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2394.802,-732.8;Float;False;Constant;_Float5;Float 5;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TauNode;42;-1207.577,-328.4082;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;39;-1866.801,-796.8;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;44;-708.0259,-482.1611;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-2202.802,-796.8;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;2;FLOAT2;-1,0;False;3;FLOAT2;1,0;False;4;FLOAT2;0.5,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2392.54,-557.1412;Float;False;Constant;_Float7;Float 7;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1306.738,-600.4202;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;40;-1625.699,-796.2426;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;45;-438.7832,-483.2784;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;106;2449.324,-145.5791;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1027.651,-483.0608;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;523.8741,51.14719;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2756.601,343.2616;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Effects/dunpai;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Overlay;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;86;0;12;2
WireConnection;13;0;86;0
WireConnection;13;1;14;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;18;0;15;0
WireConnection;111;0;107;0
WireConnection;20;0;18;0
WireConnection;113;0;111;0
WireConnection;113;1;114;0
WireConnection;2;0;1;2
WireConnection;2;1;4;0
WireConnection;2;2;5;0
WireConnection;2;3;3;0
WireConnection;2;4;20;0
WireConnection;115;0;113;0
WireConnection;115;1;116;0
WireConnection;36;0;1;1
WireConnection;36;1;35;0
WireConnection;36;2;34;0
WireConnection;36;3;37;0
WireConnection;36;4;33;0
WireConnection;38;0;2;0
WireConnection;52;0;38;0
WireConnection;52;1;1;3
WireConnection;112;0;115;0
WireConnection;56;0;36;0
WireConnection;56;1;1;3
WireConnection;121;0;1;4
WireConnection;121;1;122;0
WireConnection;53;0;52;0
WireConnection;124;0;1;3
WireConnection;118;0;112;0
WireConnection;57;0;56;0
WireConnection;48;0;45;0
WireConnection;48;1;49;0
WireConnection;55;0;48;0
WireConnection;55;1;36;0
WireConnection;54;0;53;0
WireConnection;54;1;124;0
WireConnection;54;2;57;0
WireConnection;54;3;121;0
WireConnection;50;0;45;0
WireConnection;39;0;24;0
WireConnection;44;0;43;0
WireConnection;24;0;23;0
WireConnection;24;1;29;0
WireConnection;24;2;30;0
WireConnection;24;3;31;0
WireConnection;24;4;32;0
WireConnection;41;0;40;0
WireConnection;41;1;13;0
WireConnection;40;0;39;0
WireConnection;45;0;44;0
WireConnection;106;0;78;0
WireConnection;106;1;82;0
WireConnection;106;2;118;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;51;0;50;0
WireConnection;51;1;38;0
WireConnection;0;9;54;0
WireConnection;0;13;106;0
ASEEND*/
//CHKSM=8ABC2CEF1B322D3F79410C769A8054CDABDFE531