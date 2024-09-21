Shader "Custom/CubemapArrayShaderExample"
{
    Properties 
    {
        _MainTex ("CubemapArray", CubeArray) = "" {}
        _Mip ("Mip", Float) = 0.0
        _Intensity ("Intensity", Float) = 1.0
        _SliceIndex ("Slice", Int) = 0
        _Exposure ("Exposure", Float) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparant" "IgnoreProjector"="True" "RenderType"="Transparent" "ForceSupported" = "True"}
        Pass 
        {
            Cull Off

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma require sampleLOD
        #pragma require cubearray
        #include "UnityCG.cginc"
    
        struct appdata {
            float4 pos : POSITION;
            float3 nor : NORMAL;
        };
    
        struct v2f {
            float3 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
        };
            
        uniform int _SliceIndex;
        float _Mip;
        half _Alpha;
        half _Intensity;
        float _Exposure;
    
        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.pos);
            float3 viewDir = -normalize(ObjSpaceViewDir(v.pos));
            o.uv = reflect(viewDir, v.nor);
            return o;
        }

        // Raymarching loop that calculates the distance to terrain
        float RaymarchTerrain(float3 ro, float3 rd) {
        float dist = 0.0;
        for (int i = 0; i < 100; i++) { // MAX_STEPS set to 100 for example
        float3 p = ro + rd * dist;  // Current point along the ray
        float d = TerrainSDF(p);    // Get the distance to the nearest terrain surface
        if (d < 0.001) break;       // Surface hit threshold
        dist += d;                  // Move along the ray by the distance
        if (dist > 100.0) break;    // Exit if raymarching exceeds a certain distance
        }
            return dist;
}
        
        half4 _MainTex_HDR;
        UNITY_DECLARE_TEXCUBEARRAY(_MainTex);
        fixed4 frag (v2f i) : COLOR0
        {
            fixed4 c = UNITY_SAMPLE_TEXCUBEARRAY(_MainTex, float4(i.uv, _SliceIndex));
            fixed4 cmip = UNITY_SAMPLE_TEXCUBEARRAY_LOD(_MainTex, float4(i.uv, _SliceIndex), _Mip);
            if (_Mip >= 0.0)
                c = cmip;
            c.rgb = DecodeHDR (c, _MainTex_HDR) * _Intensity;
            c.rgb *= exp2(_Exposure);
            c = lerp (c, c.aaaa, _Alpha);
            return c;
            
        }
        ENDCG

        }
    }
    Fallback Off
}
