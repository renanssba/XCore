Shader "Fertiliel/Trippy/Oscillation (Particles)"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		[Toggle(VERTICAL)] _Vertical("Vertical?", Float) = 0
		[Toggle(SAME)] _SameDir("Same Direction?", Float) = 0
		_LineHeight("Line Height", Range(1,2)) = 1
		_Intensity("Intensity",  Range(0,10)) = 1
		_Speed("Speed", Range(1,50)) = 10
		_Frequency("Frequency", Range(1,50)) = 2
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane"}
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off Lighting Off ZWrite Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles
				#pragma shader_feature VERTICAL
				#pragma shader_feature SAME
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _LineHeight;
				float _Intensity;
				float _Speed;
				float _Frequency;
				fixed4 _Color;

				struct v2f {
					float3 uv : TEXCOORD0;
					fixed4 color : COLOR;
				};

				v2f vert(
					float4 vertex : POSITION, // vertex position input
					float3 uv : TEXCOORD0, // texture coordinate input, z is a random value from custom particle vertex stream
					fixed4 color : COLOR,
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
					float vertical = 0;

					#if VERTICAL
						vertical = 1;
					#endif

					float sameDir = 1 - vertical;
					#if SAME
						sameDir = vertical;
					#endif

					float sign = (floor(screenPos[-1 * (vertical - 1)]) % _LineHeight * 2.0 < _LineHeight) ? 1.0 : -1.0;
					float disp = sign * _Intensity * sin((_Time *_Speed + i.uv.z) + _Frequency * i.uv[sameDir]) / 10.0;
					float2 displacedCoord = float2(i.uv.x + (disp *(-1 * (vertical - 1))), i.uv.y + (disp * vertical));
					//fixed4 col = tex2D(_MainTex, displacedCoord) * _Color * i.color;
					fixed4 col = tex2D(_MainTex, displacedCoord) * i.color * _Color ;
					return col;
				}
				ENDCG
			}
		}
}