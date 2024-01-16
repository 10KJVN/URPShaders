Shader "Unlit/CustomBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Destination ("Texture", 2D) = "white" {}
		_Opacity ("Opacity", Vector) = (0, 0, 0, 0)
		_Scale ("Scale", Vector) = (0, 0, 0, 0)
		_Position ("Position", Vector) = (0, 0, 0, 0)
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
			sampler2D _Destination;
			float _Opacity;
			float _Scale;
			float2 _Position;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                
				float s = 1 / _Scale;
				float2 uv = i.uv * s + _Position;
				
				float4 col = tex2D(_MainTex, uv);
				float4 dest = tex2D(_Destination, i.uv);
				
				col = col * _Opacity;
				col += dest;
								
                return col;
            }
            ENDCG
        }
    }
}
