Shader "Custom/UnlitTransparent"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}		
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 250

		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass
		{
			SetTexture [_MainTex] { combine texture }
		}
	}
}