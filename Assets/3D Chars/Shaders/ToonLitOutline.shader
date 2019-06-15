//Blue - Outline component
//Red - Additive Light
//Green - Threshold


Shader "Seasons of Love/ToonLitOutline"
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

        _Outline("Outline Width", Range(.002,0.3)) = .005
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        [Toggle(CAMERA_SCALE)] _CameraScale("Scale outline with distance?", Float) = 1
	}
	SubShader{
		Tags { "RenderType"="Opaque" }
		UsePass "Seasons of Love/ToonLit/FORWARD"
		UsePass "Seasons of Love/ToonBasicOutline/OUTLINE"
	}
	Fallback "Seasons of Love/ToonLit"
}
