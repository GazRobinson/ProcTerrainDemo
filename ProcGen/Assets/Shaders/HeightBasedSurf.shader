// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "HeightBasedSurf"
{
	Properties {
      _MainTex ("Main Texture", 2D) = "white" {}
      _SteepTex ("Steep Texture", 2D) = "white" {}
      _HighTex ("Height Texture", 2D) = "white" {}
      _HeightRange ("Height Range", Range(0.0, 300.0)) = 5.0
	  _HeightSpread("Height Spread", Range(0.0, 100.0)) = 25.0
      _Color ("Color", Color) = (1.0,0.0,0.0,1.0)
     
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
          float3 worldPos;
          float3 worldNormal;
      };
      sampler2D _MainTex;
      sampler2D _SteepTex;
      sampler2D _HighTex;
      float _HeightRange;
	  float _HeightSpread;
      float4 _Color;
      void surf (Input IN, inout SurfaceOutput o) {
         float3 up = float3(0.0f, 1.0f, 0.0f);
		 float h = IN.worldPos.y - _HeightRange;
         o.Albedo = lerp(lerp(tex2D (_SteepTex, IN.uv_MainTex).rgb,tex2D (_MainTex, IN.uv_MainTex).rgb,dot(up, pow(IN.worldNormal, 3))), tex2D (_HighTex, IN.uv_MainTex).rgb, clamp(h/_HeightSpread, 0.0f, 1.0f) );
      }
      ENDCG
    } 
    Fallback "Diffuse"
}
