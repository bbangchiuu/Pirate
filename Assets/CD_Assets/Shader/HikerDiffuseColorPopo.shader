// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Hiker/DiffuseColorPopo" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("_Color", Color) = (1,1,1,0)

	_SkinColor("_SkinColor", Color) = (1,1,1,0)
    _SkinFactor("_SkinFactor", Vector) = (5,1,1,0)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
float4 _Color;
float4 _SkinColor;
float4 _SkinFactor;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

	// Initialize the variables
	fixed4 white = fixed4(1, 1, 1, 1);
	fixed4 black = fixed4(0, 0, 0, 1);

	// Average the color out
	float dt = clamp( (abs(c.x- _SkinColor.x) + abs(c.y- _SkinColor.y) + abs(c.z- _SkinColor.z))/3,0.0,1.0)*_SkinFactor.x;
	dt = clamp(dt * dt,0.0,1.0);

	c.rgb = lerp(black, c.rgb, dt);

	o.Albedo = lerp(c.rgb,_Color.rgb, _Color.a);
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
