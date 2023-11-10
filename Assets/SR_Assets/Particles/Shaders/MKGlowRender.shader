// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MKGlowRender"
{
	SubShader 
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" "RenderType" = "MKGlow" }
		Pass
	{
		ZWrite On
		ColorMask 0
	}

		Tags { "RenderType"="MKGlow" "Queue"="Transparent" }		
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{
			ZTest lequal
			Fog { Mode Off }
			//ColorMask RGB
			Cull Off
			Lighting Off
			ZWrite Off


			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			//uniform sampler2D _MKGlowTex : register (s0);
			//uniform sampler2D _MainTex : register (s0);
			//uniform float4 _MKGlowTex_ST;

			sampler2D _MainTex : register (s0);

			float4 _MainTex_ST;
			uniform fixed4 _MKGlowColor;
			uniform half _MKGlowPower;
			//uniform half _MKGlowTexPower;
			//uniform float _MKGlowOffSet;
			
			struct Input
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};
			
			struct Output 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};
			
			Output vert (Input i)
			{
				Output o;
				//i.vertex.xyz += i.normal * _MKGlowOffSet;
				o.pos = UnityObjectToClipPos (i.vertex);
				o.uv = i.texcoord;
				o.color = i.color;
				return o;
			}

			fixed4 frag (Output i) : SV_TARGET
			{
				//fixed4 glow = tex2D(_MKGlowTex, i.uv.xy * _MKGlowTex_ST.xy + _MKGlowTex_ST.zw);
				//glow *= (_MKGlowColor * _MKGlowPower);
				//glow.a = _MKGlowColor.a;
				fixed4 glow = tex2D(_MainTex, i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				
				//_MKGlowPower *= i.color; //Add this to have Glow Power scale with vertex color (for particles)--turned off for now because I like the look better
				glow *= _MKGlowColor * _MKGlowPower; //MY CODE CHANGE THIS IF PROBLEMS ARISE
				//glow.a = _MKGlowColor.a;
				return glow;
			}
			ENDCG
		}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Pass 
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			struct Input
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : POSITION;
			};
			
			struct Output 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			Output vert (Input i)
			{
				Output o;
				o.pos = UnityObjectToClipPos (i.vertex);
				o.uv = i.texcoord;
				return o;
			}

			fixed4 frag (Output i) : SV_TARGET
			{
				return fixed4(0,0,0,0);
			}
			
			ENDCG
		}
	}
	SubShader 
	{
		Tags { "RenderType"="Transparent" }
		Pass 
		{
			Fog { Mode Off }
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile MKTRANSPARENT_ON MKTRANSPARENT_OFF
			
			struct Input
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : POSITION;
			};
			
			struct Output 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			Output vert (Input i)
			{
				Output o;
				o.pos = UnityObjectToClipPos (i.vertex);
				o.uv = i.texcoord;
				return o;
			}

			fixed4 frag (Output i) : SV_TARGET
			{
				#if MKTRANSPARENT_OFF
				return fixed4(0,0,0,1);
				#elif MKTRANSPARENT_ON
				return fixed4(0,0,0,0);
				#endif
			}
			
			ENDCG
		}
	} 
	
	SubShader 
	{
		Tags { "RenderType"="TransparentCutout" }
		Pass 
		{
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile MKCUTOUT_ON MKCUTOUT_OFF
			
			struct Input
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : POSITION;
			};
			
			struct Output 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			Output vert (Input i)
			{
				Output o;
				o.pos = UnityObjectToClipPos (i.vertex);
				o.uv = i.texcoord;
				return o;
			}

			fixed4 frag (Output i) : SV_TARGET
			{
				#if MKCUTOUT_OFF
				return fixed4(0,0,0,1);
				#elif MKCUTOUT_ON
				return fixed4(0,0,0,0);
				#endif
			}
			
			ENDCG
		}
	} 
} 
