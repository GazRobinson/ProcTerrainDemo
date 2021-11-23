Shader "Unlit/Mandlebrot"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _MaxIterations ("Max Iterations", Float) = 13.0
        _Zoom ("Zoom", Float) = 1.0
        _C("C", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
            float2 _C;
            float _MaxIterations;
            float _Zoom;
			float mandelbrot (float2 c)
            {
                float2 z = 0;
                float2 zNext;
                int i;
                for (i = 0; i < _MaxIterations; i ++)
                {
                    // f(z) = z^2 + c
                    zNext.x = z.x * z.x - z.y * z.y   + c.x;
                    zNext.y = 2 * z.x * z.y           + c.y;
                    z = zNext;
             
                    // Bounded?
                    if ( distance(z,float2(0,0)) > 2)
                        break;
                }
             
                return i / float(_MaxIterations);
            }
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = mandelbrot(i.uv*_Zoom+_C);//tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
