// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Hiker/DiffuseColorEmissive" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("_Color", Color) = (1,1,1,0)
	_EmissiveRange("_EmissiveRange", Float) = (0,1,0,0)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
float4 _Color;
float4 _EmissiveRange;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb;
	o.Emission = _Color * clamp( (_SinTime.w + 1)*0.5, _EmissiveRange.x, _EmissiveRange.y);

	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
