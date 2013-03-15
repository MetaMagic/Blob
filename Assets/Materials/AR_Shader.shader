Shader "Custom/AR_Shader" {
	Properties {
		_ColorTex("ColorTexture", 2D) = "" 
		_DepthTex("DepthTexture", 2D) = ""
	}
	SubShader {
		pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _ColorTex;
		sampler2D _DepthTex;

		struct v2f{
			float4 pos: SV_POSITION;
			float4 pixel_color : COLOR;
			float4 screen_pos : TEXCOORD0;
		};
		
		v2f vert(appdata_full v){
			v2f output;
			output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			output.screen_pos = ComputeScreenPos(output.pos);
			output.pixel_color = v.color;
			return output;
		}
		
		float4 frag(v2f o) : COLOR{
			float2 screenUV = o.screen_pos.xy / o.screen_pos.w;
			float2 uv;
			uv.x = 640 - screenUV.x;
			uv.y = 480 - screenUV.y;
			float4 textureColor = tex2D(_ColorTex, uv);
			float d = tex2D(_DepthTex, uv).r;
			float objectDepth = o.screen_pos.z;	
			if(objectDepth < d){
				return o.pixel_color;
			}
			else{
				return textureColor;
			}
		}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
