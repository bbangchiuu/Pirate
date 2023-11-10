// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Hiker/DiffuseColorUVScroll" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("_Color", Color) = (0,1,1,0)
	_Direction(" xy: huong' va` toc do", Vector ) = (1,1,0,0)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
float4 _Color;
float4 _Direction;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	float2 scroll_texcoord = ( IN.uv_MainTex + _Direction.xy*_Time.y );
	fixed4 c = tex2D(_MainTex, scroll_texcoord);
	o.Albedo = lerp(c.rgb,_Color.rgb, _Color.a);
	//o.Albedo.xy = scroll_texcoord;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
