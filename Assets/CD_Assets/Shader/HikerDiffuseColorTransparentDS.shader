// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Hiker/DiffuseColor TransparentDoubleSide" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("_Color", Color) = (0,1,1,0)
}
SubShader {
	Cull  Off
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
float4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = lerp(c.rgb,_Color.rgb, _Color.a);
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
