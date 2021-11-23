Shader "Unlit/InstancedShader"
{
	Properties
    {
        _Color("Color", Color) = "white" {}
        _TextureIdx("Texture Idx", float) = 0.0
        _Textures("Textures", 2DArray) = "" {}
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        CGPROGRAM
 
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.5
        #include "UnityCG.cginc"
 
        UNITY_DECLARE_TEX2DARRAY(_Textures);
 
        struct Input
        {
            fixed2 uv_Textures;
        };
 
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(float, _TextureIdx)
        UNITY_INSTANCING_BUFFER_END(Props)
 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = UNITY_SAMPLE_TEX2DARRAY(
                _Textures,
                float3(IN.uv_Textures, UNITY_ACCESS_INSTANCED_PROP(Props, _TextureIdx))
            );
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
 
        ENDCG
    }
    FallBack "Diffuse"
}
