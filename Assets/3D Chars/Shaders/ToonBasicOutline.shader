Shader "Seasons of Love/ToonBasicOutline"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Shininess ("Shininess(0.0:)", Float) = 0.5
		
		_ShadowThreshold ("Shadow Threshold(0.0:1.0)", Float) = 0.5
		_ShadowColor ("Shadow Color(RGBA)", Color) = (0,0,0,0.5)
		_ShadowSharpness ("Shadow Sharpness(0.0:)", Float) = 100
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.9

        _Outline("Outline Width", Range(.002,0.03)) = .005
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        [Toggle(CAMERA_SCALE)] _CameraScale("Scale outline with distance?", Float) = 1

	}


	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma shader_feature CAMERA_SCALE
	//#pragma target 5.0
	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal: NORMAL;
		fixed4 color: COLOR;
	};

	struct v2f
	{
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        UNITY_FOG_COORDS(0)
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;
	
	v2f vert (appdata v)
	{
		//Remember that the blue component of the vertex color controls outline thickness. Blue = 1 is max width.
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
        float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);

		#ifdef CAMERA_SCALE
				o.vertex.xy += offset * _Outline * v.color.b;
		#else
			#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
				o.vertex.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vertex.z) * _Outline * v.color.b;
			#else
				o.vertex.xy += offset * _Outline * o.vertex.z * v.color.b;
			#endif
		#endif



		o.color = _OutlineColor;

		UNITY_TRANSFER_FOG(o,o.vertex);

        return o;
	}
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		//UsePass "Toon/Basic/BASE"
		Pass
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite On
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			fixed4 frag (v2f i) : SV_Target
			{
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, i.color);
				return i.color;
			}
			ENDCG
		}
	}
	Fallback "Toon/Basic"
}
