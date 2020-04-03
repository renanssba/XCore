Shader "Fertiliel/Vertical Oscillation"
{
	Properties
	{
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		_LineHeight("Line Height", Range(1,2)) = 1
		_Intensity("Intensity",  Range(0,10)) = 1
		_Speed("Speed", Range(1,50)) = 10
		_Frequency("Frequency", Range(1,50)) = 2
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LineHeight;
			float _Intensity;
			float _Speed;
			float _Frequency;
			fixed4 _Color;

			// note: no SV_POSITION in this struct
			struct v2f {
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(
				float4 vertex : POSITION, // vertex position input
				float2 uv : TEXCOORD0, // texture coordinate input
				float4 color : COLOR,
				out float4 outpos : SV_POSITION // clip space position output
			)
			{
				v2f o;
				o.uv = uv;
				o.color = color;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}
			
			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float sign = (floor(screenPos.x)% _LineHeight*2.0 < _LineHeight) ? 1.0 : -1.0;
				float disp = sign * _Intensity * sin((_Time *_Speed) + _Frequency * i.uv.y) / 10.0;
				float2 displacedCoord = float2(i.uv.x, i.uv.y + disp);
				// sample the texture
				fixed4 col = tex2D(_MainTex, displacedCoord) * i.color;
				return col;
			}
			ENDCG
		}
	}
}
/*
varying vec2 vTextureCoord;
uniform sampler2D uSampler;

uniform vec4 filterArea;
uniform vec4 filterClamp;
uniform vec2 dimensions;

uniform float time;
uniform float speed;
uniform int lineHeight;
uniform float intensity;
uniform float frequency;
vec2 loopCoord(vec2 v) {
	if (v.x > filterClamp.z) {
		v.x -= filterClamp.z;
	}
	else if (v.x < filterClamp.x) {
		v.x += filterClamp.z;
	}

	if (v.y > filterClamp.w) {
		v.y -= filterClamp.w;
	}
	else if (v.y < filterClamp.y) {
		v.y += filterClamp.w;
	}
	return v;
}

void main(void)
{
	vec2 pixelCoord = vTextureCoord.xy * filterArea.xy;
	vec2 coord = pixelCoord / dimensions;
	if (coord.x > 1.0 || coord.y > 1.0) {
		return;
	}

	float lh = float(lineHeight);
	float sign = (mod(floor(pixelCoord.x), lh*2.0) < lh) ? 1.0 : -1.0;
	float disp = sign * intensity * sin((time *speed) + frequency * coord.y) / 10.0;
	vec2 displacedCoord = vec2(coord.x, coord.y + disp);

	gl_FragColor = texture2D(uSampler, loopCoord(displacedCoord));
}
*/

