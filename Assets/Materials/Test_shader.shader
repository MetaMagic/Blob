Shader "Custom/Test_shader" {
	Properties {
		_DepthTex ("Base (RGB)", 2D) = "" 
		_ColorTex("Base (RGB)", 2D) = "" 
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _DepthTex;
		sampler2D _ColorTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_ColorTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			float2 uv;
			uv.x = 640 - screenUV.x;
			uv.y = 480 - screenUV.y;
			float depthValue = tex2D(_DepthTex, uv);
			clip( ( IN.screenPos.z <= ( depthValue  ) ) ? 1 : -1 );
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
