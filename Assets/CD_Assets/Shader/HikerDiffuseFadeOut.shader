Shader "Hiker/DiffuseFadeOut"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("_Color", Color) = (1,1,1,0)
		_EmissiveRange("_EmissiveRange", Float) = (0,1,0,1)
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 Normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 local_vertex : TEXCOORD1;
				float3 Normal : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _EmissiveRange;
			fixed4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.local_vertex = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);		
				// Calculates the vertex's surface normal in object space.
				o.Normal = normalize(mul(float4(v.Normal, 0.0), unity_WorldToObject).xyz);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float specularIntensity = 0.0;

				// Calculates the light's direction in world space.
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				float3 normal = normalize(i.Normal);

				// Calculates the light's intensity on the vertex.
				float intensity = max(dot(normal, lightDirection), 0.0);
				
				// sample the texture
				fixed4 diff = tex2D(_MainTex, i.uv) + _Color;
				fixed4 col = diff;
				float pos_a = saturate((i.local_vertex.y - _EmissiveRange.x) / (_EmissiveRange.y - _EmissiveRange.x));

				col.a = clamp(pos_a, _EmissiveRange.z, _EmissiveRange.w);
				return col;
			}
		ENDCG
	}
	}
}