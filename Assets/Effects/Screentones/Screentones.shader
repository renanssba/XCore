Shader "Fertiliel/Screentones"
{
	Properties
	{
		[PerRendererData]_MainTex (" Alpha Mask", 2D) = "black" {}
		_GridSize("Grid Size", Range(1,500)) = 30
		_MaskScale("Mask Scale", Range(-0.5,2)) = 0
		_PatternColor("Pattern Color", Color) = (1,1,1,1)
		_BackgroundColor("Background Color", Color) = (1,1,1,0)
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _GridSize;
			float _MaskScale;
			fixed4 _PatternColor;
			fixed4 _BackgroundColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//o.uv = TRANSFORM_TEX(lerp(v.uv - _MaskScale, v.uv + _MaskScale, v.uv), _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				float clampedMaskScale = 2 - (clamp(_MaskScale,0,1.999));
				float2 cellSize = float2(1.0,1.0) / _GridSize / clampedMaskScale;
				float2 scaledUV = lerp(i.uv - _MaskScale, i.uv + _MaskScale, i.uv);
				fixed4 tex = tex2D(_MainTex, scaledUV);
				float2 pos = fmod(i.uv, cellSize) - cellSize/2.0;
				float dist_squared = dot(pos, pos);
				fixed4 col = lerp(_PatternColor, _BackgroundColor, smoothstep(0.00008 * tex.a ,0.0001 * (tex.a) , dist_squared));
				return col;
			}
			ENDCG
		}
	}
}
