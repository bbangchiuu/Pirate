Shader "Hiker/HikerHologram" {
    Properties {
      //_MainTex ("Texture", 2D) = "white" {}      
	  _BumpMap ("Normalmap", 2D) 	= "bump" {} 
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	  //_ExtraColor ("Extra Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	  //_BluePower ("Green Power", Range(1,0)) = 0
	  //_Brighness ("Brighness", Range(0,10)) = 1.7
    }
    SubShader {
      Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	  Blend SrcAlpha One
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
		  float3 worldPos;
      };
      //sampler2D _MainTex;
      sampler2D _BumpMap;
      float4 _RimColor;
      float _RimPower;
      void surf (Input IN, inout SurfaceOutput o) {
		  //clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5);

          //half3 orgCol = tex2D (_MainTex, IN.uv_MainTex).rgb;		  
		  //orgCol = orgCol*_RimColor + _ExtraColor;

          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = _RimColor.rgb * pow (rim, _RimPower) * ( 1.4f +  _SinTime.w*0.3f );
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }