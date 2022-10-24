// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Component02"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.02
		_Outline_Color("Outline_Color", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Shadow_Color("Shadow_Color", Color) = (0,0,0,0)
		_RimSampler("Rim Sampler", 2D) = "white" {}
		_RimColor("RimColor", Color) = (0,0,0,0)
		_RimIntensity("RimIntensity", Range( 0 , 10)) = 5
		_SpeShaTex("SpeShaTex", 2D) = "white" {}
		[HDR]_Emission_Color("Emission_Color", Color) = (0,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Specilar("Specilar", Range( 0 , 1)) = 0
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
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Emission = ( tex2D( _Albedo, uv_Albedo ) * _Outline_Color ).rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
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
		uniform float4 _Emission_Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Shadow_Color;
		uniform sampler2D _RimSampler;
		uniform float4 _RimColor;
		uniform float _RimIntensity;
		uniform float _Specilar;
		uniform float _Smoothness;
		uniform float4 _Outline_Color;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_SpeShaTex = i.uv_texcoord * _SpeShaTex_ST.xy + _SpeShaTex_ST.zw;
			float4 tex2DNode29 = tex2D( _SpeShaTex, uv_SpeShaTex );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = i.worldNormal;
			float dotResult5 = dot( ase_worldlightDir , ase_worldNormal );
			float Light_Dot70 = (dotResult5*0.5 + 0.5);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 shadow74 = ( tex2D( _Albedo, uv_Albedo ) * saturate( ( 1.0 - Light_Dot70 ) ) * _Shadow_Color );
			float dotResult42 = dot( ase_worldNormal , ( ase_worldlightDir + float3(-1,0,0) ) );
			float floatrimlightDot55 = saturate( ( ( dotResult42 + 1.5 ) * 0.5 ) );
			float dotResult36 = dot( ase_worldNormal , i.viewDir );
			float floatnormalDotEye40 = dotResult36;
			float clampResult53 = clamp( ( 1.0 - abs( floatnormalDotEye40 ) ) , 0.02 , 0.98 );
			float floatfalloffU54 = clampResult53;
			float2 temp_cast_1 = (saturate( ( floatfalloffU54 * floatrimlightDot55 ) )).xx;
			float4 rim66 = ( ( floatrimlightDot55 * tex2D( _RimSampler, temp_cast_1 ) ) * _RimColor * _RimIntensity * ase_lightAtten * tex2D( _Albedo, uv_Albedo ) );
			SurfaceOutputStandardSpecular s83 = (SurfaceOutputStandardSpecular ) 0;
			s83.Albedo = float3( 0,0,0 );
			s83.Normal = ase_worldNormal;
			s83.Emission = float3( 0,0,0 );
			float4 tex2DNode84 = tex2D( _SpeShaTex, uv_SpeShaTex );
			float3 temp_cast_2 = (( tex2DNode84.r * _Specilar )).xxx;
			s83.Specular = temp_cast_2;
			s83.Smoothness = ( tex2DNode84.r * _Smoothness );
			s83.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi83 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g83 = UnityGlossyEnvironmentSetup( s83.Smoothness, data.worldViewDir, s83.Normal, float3(0,0,0));
			gi83 = UnityGlobalIllumination( data, s83.Occlusion, s83.Normal, g83 );
			#endif

			float3 surfResult83 = LightingStandardSpecular ( s83, viewDir, gi83 ).rgb;
			surfResult83 += s83.Emission;

			#ifdef UNITY_PASS_FORWARDADD//83
			surfResult83 -= s83.Emission;
			#endif//83
			c.rgb = ( ( tex2D( _Albedo, uv_Albedo ) * Light_Dot70 * ase_lightColor ) + shadow74 + rim66 + float4( surfResult83 , 0.0 ) ).rgb;
			c.a = tex2DNode29.g;
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
			float4 tex2DNode29 = tex2D( _SpeShaTex, uv_SpeShaTex );
			o.Emission = ( tex2DNode29.b * _Emission_Color ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				vertexDataFunc( v, customInputData );
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
-1428;284;1047;542;4391.479;683.5786;2.364388;True;False
Node;AmplifyShaderEditor.CommentaryNode;33;-4261.202,-895.7019;Float;False;2167.316;1782.634;Comment;36;66;65;64;63;62;61;60;59;58;57;56;55;54;53;52;51;50;49;48;47;46;45;44;43;42;41;40;39;38;37;36;35;34;68;69;81;Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;34;-3238.077,-845.7019;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;35;-3237.343,-672.0477;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;37;-4130.053,226.0193;Float;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;-1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;38;-4200.581,-7.762573;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;36;-3005.781,-749.3795;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;41;-4186.879,-177.9538;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-3913.578,96.65543;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-2820.504,-754.1049;Float;False;floatnormalDotEye;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3595.251,62.9183;Float;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;42;-3642.675,-178.9209;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;43;-3959.156,-492.3823;Float;False;40;floatnormalDotEye;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-3358.735,-178.9505;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3341.364,60.67313;Float;False;Constant;_Float4;Float 4;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;45;-3685.51,-485.8306;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-3355.487,-299.4992;Float;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;False;0;0.98;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-3545.899,-485.2988;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;89;-4244.946,958.6675;Float;False;1333.371;1075.394;Comment;13;3;2;5;7;6;70;75;22;23;26;27;25;74;light;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-3357.862,-408.3267;Float;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-3116.606,-179.355;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;53;-3134.764,-483.2392;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;3;-4129.216,1215.668;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-4162.216,1008.667;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;52;-2908.525,-179.0796;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;5;-3872.214,1117.668;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-2848.212,-487.7763;Float;False;floatfalloffU;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-3903.785,1275.302;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-2713.564,-182.0127;Float;False;floatrimlightDot;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-3652.094,432.2327;Float;False;55;floatrimlightDot;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-3544.494,217.3221;Float;False;54;floatfalloffU;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;6;-3722.522,1199.668;Float;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-3317.366,310.2651;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-3433.997,1193.199;Float;False;Light_Dot;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;59;-3090.91,309.2294;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;-4194.946,1668.883;Float;False;70;Light_Dot;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-3193.312,648.7225;Float;False;Property;_RimIntensity;RimIntensity;6;0;Create;True;0;0;False;0;5;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;61;-2905.842,281.1202;Float;True;Property;_RimSampler;Rim Sampler;4;0;Create;True;0;0;False;0;None;152d0c61672f25c4d947e9eb353319e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;22;-3983.635,1674.15;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;64;-3124.189,730.3483;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;69;-2895.398,574.4474;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;27;-3622.711,1827.062;Float;False;Property;_Shadow_Color;Shadow_Color;3;0;Create;True;0;0;False;0;0,0,0,0;0.3962264,0.3962264,0.3962264,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;68;-2925.042,557.5075;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-2597.76,263.1427;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;23;-3791.636,1674.15;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-3705.003,1476.291;Float;True;Global;TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;-2843.629,603.7927;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;63;-3134.164,476.8053;Float;False;Property;_RimColor;RimColor;5;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-3321.258,1649.691;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2523.301,463.0214;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-2028.625,-485.8681;Float;False;Property;_Smoothness;Smoothness;9;0;Create;True;0;0;False;0;0;0.664;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;84;-2041.757,-711.7938;Float;True;Property;_TextureSample2;Texture Sample 2;7;0;Create;True;0;0;False;0;None;d7e270ef15409404ea3892ffbe6868d5;True;0;False;white;Auto;False;Instance;29;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-2039.212,-841.1247;Float;False;Property;_Specilar;Specilar;10;0;Create;True;0;0;False;0;0;0.469;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;80;-1203.906,772.919;Float;False;Property;_Outline_Color;Outline_Color;1;0;Create;True;0;0;False;0;0,0,0,0;0.2735849,0.2735849,0.2735849,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-2336.886,456.5704;Float;False;rim;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;82;-1931.218,210.2491;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;78;-1278.548,560.7899;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;c12eeb09d7f9ae8468f2da3d37293ce2;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;72;-1946.549,136.4872;Float;False;70;Light_Dot;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1713.625,-564.8681;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2015.738,-122.7688;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;None;374069d28f5ec91418b3efe7ed697b27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-3154.575,1642.364;Float;False;shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1712.211,-859.1247;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-1521.753,185.8769;Float;False;74;shadow;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1684.036,118.5424;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-1517.473,260.6654;Float;False;66;rim;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-956.9057,716.919;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;32;-1013.984,-99.2522;Float;False;Property;_Emission_Color;Emission_Color;8;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,2.101961,8,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-1080.851,-285.5757;Float;True;Property;_SpeShaTex;SpeShaTex;7;0;Create;True;0;0;False;0;None;ed6b776890e4a214298ee8b46e71dab3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomStandardSurface;83;-1509.06,-657.8105;Float;False;Specular;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-750.6486,-179.4788;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1247.938,119.7402;Float;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;77;-789.5466,379.2907;Float;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-538.72,-108.6045;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Component02;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.02;0.0754717,0.0754717,0.0754717,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;39;0;38;0
WireConnection;39;1;37;0
WireConnection;40;0;36;0
WireConnection;42;0;41;0
WireConnection;42;1;39;0
WireConnection;47;0;42;0
WireConnection;47;1;44;0
WireConnection;45;0;43;0
WireConnection;51;0;45;0
WireConnection;48;0;47;0
WireConnection;48;1;46;0
WireConnection;53;0;51;0
WireConnection;53;1;50;0
WireConnection;53;2;49;0
WireConnection;52;0;48;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;54;0;53;0
WireConnection;55;0;52;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;6;2;7;0
WireConnection;58;0;57;0
WireConnection;58;1;56;0
WireConnection;70;0;6;0
WireConnection;59;0;58;0
WireConnection;61;1;59;0
WireConnection;22;0;75;0
WireConnection;69;0;64;0
WireConnection;68;0;60;0
WireConnection;81;0;55;0
WireConnection;81;1;61;0
WireConnection;23;0;22;0
WireConnection;25;0;26;0
WireConnection;25;1;23;0
WireConnection;25;2;27;0
WireConnection;65;0;81;0
WireConnection;65;1;63;0
WireConnection;65;2;68;0
WireConnection;65;3;69;0
WireConnection;65;4;62;0
WireConnection;66;0;65;0
WireConnection;85;0;84;1
WireConnection;85;1;86;0
WireConnection;74;0;25;0
WireConnection;87;0;84;1
WireConnection;87;1;88;0
WireConnection;9;0;1;0
WireConnection;9;1;72;0
WireConnection;9;2;82;0
WireConnection;79;0;78;0
WireConnection;79;1;80;0
WireConnection;83;3;87;0
WireConnection;83;4;85;0
WireConnection;30;0;29;3
WireConnection;30;1;32;0
WireConnection;24;0;9;0
WireConnection;24;1;76;0
WireConnection;24;2;67;0
WireConnection;24;3;83;0
WireConnection;77;0;79;0
WireConnection;0;2;30;0
WireConnection;0;9;29;2
WireConnection;0;13;24;0
WireConnection;0;11;77;0
ASEEND*/
//CHKSM=74FA346E7B686DA6C9E8A49184B3C73D9E55DD05