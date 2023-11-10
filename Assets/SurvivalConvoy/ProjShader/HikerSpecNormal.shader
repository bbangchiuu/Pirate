Shader "Hiker/HikerSpecNormal" 
{
	Properties 
	{
	  _MainTex ("Texture", 2D) = "white" {}
	  _SpecTex ("SpecMap", 2D) = "white" {}
	  _SpecStr ("Specular", Float ) = 1
	  _Shininess ("Shininess", Range (0.03, 10)) = 0.078125
	  _BumpMap ("Normalmap", 2D) 	= "bump" {}  
	}

	Category {
		Tags { "RenderType" = "Opaque" }

		SubShader 
		{
			LOD 300 
			
			CGPROGRAM
			#pragma surface surf ColoredSpecular noforwardadd halfasview interpolateview
			#pragma target 3.0
			#pragma glsl 
			#pragma exclude_renderers d3d11_9x
	 
			struct MySurfaceOutput 
			{
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half Specular;
				half Gloss;
				half Alpha;
			};

			half4 LightingColoredSpecular (MySurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
			{
				fixed diff = max (0, dot (s.Normal, lightDir));

				fixed nh = max (0, dot (s.Normal, halfDir));

				fixed spec = pow (nh, s.Specular*128) * s.Gloss;

				fixed4 c;

				//c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2)*100;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
		
				UNITY_OPAQUE_ALPHA(c.a);

				return c;
			}
	 	 
	 
			struct Input 
			{
				float2 uv_MainTex;		
				INTERNAL_DATA
			};
	
			half _Shininess, _SpecStr;
	
			sampler2D _MainTex, _BumpMap, _SpecTex;	
	
			void surf (Input IN, inout MySurfaceOutput o)
			{
				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		
				half spec = tex2D (_SpecTex, IN.uv_MainTex).g;
				
				o.Gloss = spec*_SpecStr;

				o.Specular =  _Shininess;
		
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}
			ENDCG
		}

		SubShader {
			LOD 200

		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input {
			  float2 uv_MainTex;
		  };
		  sampler2D _MainTex;
		  void surf (Input IN, inout SurfaceOutput o) {
			  o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		  }
		  ENDCG
		}
	}
	fallback "Mobile/Diffuse"
}