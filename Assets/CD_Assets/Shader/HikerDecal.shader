// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Hiker/TextDecal" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("_Color", Color) = (1,1,1,0)
}
SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Geometry+1" "ForceNoShadowCasting"="True" }
	LOD 150
	Offset -1, -1

CGPROGRAM
#pragma surface surf Unlit noforwardadd noambient decal:blend

half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) {
half4 c;
c.rgb = s.Albedo*atten;
c.a = s.Alpha;
return c;
}

sampler2D _MainTex;
float4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = _Color;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
