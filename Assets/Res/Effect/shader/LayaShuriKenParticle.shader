Shader "LayaAir3D/ShurikenParticle" {
	Properties {
		_MainTex("Particle Texture", 2D) = "white" {}
		_Color("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		
		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.01
		[HideInInspector] _Cull ("__cull", Float) = 0.0
		[HideInInspector] _Mode ("__mode", Float) = 1.0
		[HideInInspector] _SrcBlend ("__src", Float) = 5.0
		[HideInInspector] _DstBlend ("__dst", Float) = 10.0
		[HideInInspector] _ZWrite("__zw", Float) = 0.0
		[HideInInspector] _ZTest("__zt", Float) = 2.0
		[HideInInspector] _RenderQueue("__rq", Float) = 0.0
	}
	SubShader {		
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Pass { 
			Tags { "LightMode"="ForwardBase" }
		
			Blend [_SrcBlend] [_DstBlend]
			ColorMask RGB
			Cull Off Lighting On ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color = 2.0 * i.color * tex2D(_MainTex, i.uv) * _Color;
				UNITY_APPLY_FOG_COLOR(i.fogCoord, color, fixed4(0,0,0,0));
				color = clamp(color, 0, 1);
				return color;
			}
			ENDCG
		}
	} 
	CustomEditor "LayaParticleGUI"
}
