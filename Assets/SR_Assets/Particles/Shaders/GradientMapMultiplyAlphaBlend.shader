// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Citadel Deep/Gradient Map Multiply Alpha Blend" {
Properties {
    _MainTex ("Base (RGBA)", 2D) = "white" {}
    _Color1 ("Hi Color", color) = (1.0,1.0,1.0,1.0)
    _Step1 ("Color Cutoff", range (0.0,2.0)) = 0.5
    _Color2 ("Lo Color", color) = (1.0,0.95,0.8,1.0)
    //_Step2 ("Color Cutoff 2", range (0.0,2.0)) = 0.5
    //_Color3 ("Shade Color", color) = (0.0,0.0,0.0,1.0)
    
   // _Step2 ("Color Lo Cutoff", range (0.0,1.0)) = 0.25
   // _Color3 ("Shade Color", color) = (0.5, 0.6, 0.8, 1.0)
    
    _AlphaCutoff ("Alpha Cutoff", range (0.0,1.0)) = 0.0
    _AlphaInfluence ("Alpha Influence", range (0.0,50.0)) = 1.0
    
    _Intensity ("Intensity", range (-1,10)) = 1

	_MKGlowPower("Emission Power", range(0,10)) = 0

	_MKGlowColor("Emission Color", color) = (1.0,0.95,0.8,1.0)
    
}

SubShader {
    //Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "RenderType" = "MKGlow" }
    LOD 100
   
		//Lighting On
   	//Cull Back
    ZWrite Off
    //ColorMask 0
    Blend SrcAlpha OneMinusSrcAlpha
		//AlphaToMask On
   
    Pass {  
		Cull Back
        	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				//fixed4 color : COLOR;
            };

            sampler2D _MainTex;
           
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _Step1;
            float _AlphaCutoff;
            float _AlphaInfluence;
            
            float _Intensity;
           
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
				//half2 uv = i.texcoord;
				//uv.x = uv.x<0 ? 0 : uv.x;
				//uv.y = uv.y<0 ? 0 : uv.y;
				//half4 col = tex2D(_MainTex, uv);
                fixed4 col = tex2D(_MainTex, i.texcoord);
                float blendV = col.r;
                float texA = col.a;

                _Step1 = lerp(_AlphaCutoff,2.0, _Step1);
                col = lerp(_Color2,_Color1,smoothstep(_AlphaCutoff,_Step1,blendV));
                //col.xyz *= _Intensity;
				col *= _Intensity;

                if(texA <= _AlphaCutoff) discard;
                texA = lerp(1.0,texA,_AlphaInfluence);
                col.xyz *= texA;
                col.a *= texA;
				col *= i.color;
               	return fixed4(col.r, col.g, col.b, col.a);
            }
        ENDCG
    }


		Pass{
				Cull Front
				CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;

			float4 _MainTex_ST;
			float4 _Color1;
			float4 _Color2;
			float _Step1;
			float _AlphaCutoff;
			float _AlphaInfluence;

			float _Intensity;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//half2 uv = i.texcoord;
				//uv.x = uv.x<0 ? 0 : uv.x;
				//uv.y = uv.y<0 ? 0 : uv.y;
				//half4 col = tex2D(_MainTex, uv);
				fixed4 col = tex2D(_MainTex, i.texcoord);
			float blendV = col.r;
			float texA = col.a;

			_Step1 = lerp(_AlphaCutoff,2.0, _Step1);
			col = lerp(_Color2,_Color1,smoothstep(_AlphaCutoff,_Step1,blendV));
			//col.xyz *= _Intensity;
			col *= _Intensity;

			if (texA <= _AlphaCutoff) discard;
			texA = lerp(1.0,texA,_AlphaInfluence);
			col.xyz *= texA;
			col.a *= texA;
			col *= i.color;
			return fixed4(col.r, col.g, col.b, col.a);
			}
				ENDCG
			}
}

}