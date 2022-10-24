// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Effects/Trails"
{
	Properties
	{
		_Texture0("Texture 0", 2D) = "white" {}
		_Float1("Float 1", Range( 0 , 1)) = 1
		[HDR]_Color0("Color 0", Color) = (0,1,0.8665144,0)
		[HDR]_Color1("Color 1", Color) = (1,0,0.8518004,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color1;
		uniform float4 _Color0;
		uniform sampler2D _Texture0;
		uniform float _Float1;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float u35 = i.uv_texcoord.x;
			float clampResult78 = clamp( ( u35 * 2.45 ) , 0.0 , 1.0 );
			float4 lerpResult74 = lerp( _Color1 , _Color0 , clampResult78);
			float time21 = _Time.y;
			float2 uv23 = i.uv_texcoord;
			float2 panner43 = ( time21 * float2( -2,0 ) + uv23);
			float clampResult53 = clamp( ( ( tex2D( _Texture0, panner43 ).g + ( ( ( 1.0 - u35 ) + -0.24 ) * 0.91 ) ) - ( ( ( u35 * 1.09 ) + -0.13 ) * 1.18 ) ) , 0.0 , 1.0 );
			float Opacity56 = clampResult53;
			float2 panner12 = ( time21 * float2( -0.5,0 ) + uv23);
			float2 appendResult25 = (float2(tex2D( _Texture0, panner12 ).rg));
			float2 panner37 = ( time21 * float2( -0.5,0 ) + ( uv23 + ( ( ( appendResult25 + -0.49 ) * 2.71 ) * _Float1 * u35 ) ));
			float2 Trails0239 = panner37;
			float V87 = i.uv_texcoord.y;
			float clampResult94 = clamp( ( V87 * ( 1.0 - V87 ) * ( 1.0 - u35 ) * 5.0 ) , 0.0 , 1.0 );
			float Opacity0296 = clampResult94;
			float temp_output_58_0 = ( Opacity56 * tex2D( _Texture0, Trails0239 ).b * i.vertexColor.a * Opacity0296 );
			o.Emission = ( ( lerpResult74 * i.vertexColor ) * temp_output_58_0 ).rgb;
			o.Alpha = temp_output_58_0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

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
				half4 color : COLOR0;
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
				o.color = v.color;
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
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
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
171;228;1092;605;40.59695;1369.892;2.412902;True;False
Node;AmplifyShaderEditor.RangedFloatNode;17;-1898.314,-43.45438;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;14;-1719.729,-40.30719;Float;False;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-1776.002,-179.9162;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1781.367,-379.4483;Float;True;Property;_Texture0;Texture 0;0;0;Create;True;0;0;False;0;None;9755606df148ca44e867f1c5451ae9ab;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-1533.15,-44.78928;Float;False;time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1545.409,-276.6175;Float;False;uv;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1548.989,-380.5981;Float;False;Trails;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;16;-3417.39,411.4306;Float;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;-0.5,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;22;-3413.712,536.092;Float;False;21;time;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-3421.208,334.6809;Float;False;23;uv;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-3197.222,313.3766;Float;False;19;Trails;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;12;-3193.453,392.8677;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;-3000.229,312.4258;Float;True;Property;_Aldobe;Aldobe;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;-1539.748,-194.6734;Float;False;u;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-3463.37,1221.729;Float;False;21;time;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-2699.694,317.1731;Float;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-3357.887,1328.053;Float;True;35;u;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-3460.37,1014.729;Float;False;23;uv;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-2704.054,813.8368;Float;False;35;u;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-3078.404,1329.842;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-3241.282,951.2979;Float;False;19;Trails;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;-1538.555,-118.6;Float;False;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-2480.86,617.8393;Float;False;35;u;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2576.571,530.621;Float;False;Property;_Float1;Float 1;1;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;43;-3272.671,1105.3;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;28;-2539.168,316.855;Float;True;ConstantBiasScale;-1;;1;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT2;0,0;False;1;FLOAT;-0.49;False;2;FLOAT;2.71;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;46;-2792.999,1309.445;Float;True;ConstantBiasScale;-1;;2;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.24;False;2;FLOAT;0.91;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-3174.521,-334.1945;Float;True;87;V;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-3149.784,-104.5092;Float;False;35;u;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;-3025.904,1022.086;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2458.729,820.7346;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1.09;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-2272.38,255.6277;Float;False;23;uv;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-2265.02,460.6315;Float;True;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;89;-2946.784,-259.3092;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-2039.26,341.2619;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-3009.765,40.69811;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;84;-2180.209,817.9523;Float;True;ConstantBiasScale;-1;;3;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.13;False;2;FLOAT;1.18;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;92;-2961.765,-63.30201;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1896.929,569.5721;Float;False;21;time;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-2502.838,1147.713;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;50;-2172.024,1068.015;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;37;-1709.887,443.5055;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.5,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-2705.784,-307.3092;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;544.1127,-855.7996;Float;True;35;u;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;794.7852,-851.8778;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;2.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;94;-2470.565,-306.502;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;53;-1926.698,1065.555;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-1427.276,436.5194;Float;False;Trails02;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1655.9,1062.651;Float;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;700.5271,-239.2334;Float;False;39;Trails02;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;73;843.4643,-1249.347;Float;False;Property;_Color1;Color 1;3;1;[HDR];Create;True;0;0;False;0;1,0,0.8518004,0;5.992157,2.47847,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;27;696.231,-341.6731;Float;False;19;Trails;1;0;OBJECT;0;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;72;838.0916,-1047.474;Float;False;Property;_Color0;Color 0;2;1;[HDR];Create;True;0;0;False;0;0,1,0.8665144,0;9.51876,3.854704,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;78;1015.271,-863.9578;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-2292.129,-316.9521;Float;False;Opacity02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;939.3923,-321.9746;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;97;1252.445,-200.4296;Float;False;96;Opacity02;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;1255.836,-1063.943;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;1026.405,-487.9089;Float;False;56;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;80;1236.574,-673.3165;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;1475.563,-432.579;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;1564.368,-921.8641;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;86;-2974.404,-563.2593;Float;True;False;True;True;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-3192.607,-555.9618;Float;True;23;uv;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;1838.124,-608.261;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;3405.208,1085.575;Float;False;-1;;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2411.253,-499.8024;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Effects/Trails;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;17;0
WireConnection;21;0;14;0
WireConnection;23;0;13;0
WireConnection;19;0;11;0
WireConnection;12;0;24;0
WireConnection;12;2;16;0
WireConnection;12;1;22;0
WireConnection;10;0;20;0
WireConnection;10;1;12;0
WireConnection;35;0;13;1
WireConnection;25;0;10;0
WireConnection;48;0;47;0
WireConnection;87;0;13;2
WireConnection;43;0;44;0
WireConnection;43;1;45;0
WireConnection;28;3;25;0
WireConnection;46;3;48;0
WireConnection;41;0;42;0
WireConnection;41;1;43;0
WireConnection;52;0;51;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;29;2;36;0
WireConnection;89;0;88;0
WireConnection;31;0;34;0
WireConnection;31;1;29;0
WireConnection;84;3;52;0
WireConnection;92;0;91;0
WireConnection;49;0;41;2
WireConnection;49;1;46;0
WireConnection;50;0;49;0
WireConnection;50;1;84;0
WireConnection;37;0;31;0
WireConnection;37;1;38;0
WireConnection;90;0;88;0
WireConnection;90;1;89;0
WireConnection;90;2;92;0
WireConnection;90;3;93;0
WireConnection;76;0;75;0
WireConnection;94;0;90;0
WireConnection;53;0;50;0
WireConnection;39;0;37;0
WireConnection;56;0;53;0
WireConnection;78;0;76;0
WireConnection;96;0;94;0
WireConnection;26;0;27;0
WireConnection;26;1;40;0
WireConnection;74;0;73;0
WireConnection;74;1;72;0
WireConnection;74;2;78;0
WireConnection;58;0;57;0
WireConnection;58;1;26;3
WireConnection;58;2;80;4
WireConnection;58;3;97;0
WireConnection;79;0;74;0
WireConnection;79;1;80;0
WireConnection;86;0;85;0
WireConnection;81;0;79;0
WireConnection;81;1;58;0
WireConnection;0;2;81;0
WireConnection;0;9;58;0
ASEEND*/
//CHKSM=6891A66CA68CF7A3DB8189488A804861D78D5BEE