Shader "Fertiliel/Trippy/Horizontal Oscillation"
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

				fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
				{
					float sign = (floor(screenPos.y) % _LineHeight*2.0 < _LineHeight) ? 1.0 : -1.0;
					float disp = sign * _Intensity * sin((_Time *_Speed) + _Frequency * i.uv.y) / 10.0;
					float2 displacedCoord = float2(i.uv.x + disp, i.uv.y);
					// sample the texture
					fixed4 col = tex2D(_MainTex, displacedCoord) * i.color;
					return col;
				}
				ENDCG
			}
		}
}