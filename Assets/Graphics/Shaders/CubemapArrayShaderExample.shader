Shader "Custom/CubemapArrayWithSDF"
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
        Tags { "RenderType"="Transparent" }
        Pass 
        {
        Cull Off

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma require sampleLOD
        #pragma require cubearray
        #include "UnityCG.cginc"
        #include "Includes/SDFFunctions.hlsl"  // Include the SDF file
        
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

        // Vertex Shader
        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.pos);
            float3 viewDir = -normalize(ObjSpaceViewDir(v.pos));
            o.uv = reflect(viewDir, v.nor);
            return o;
        }

        half4 _MainTex_HDR;
        UNITY_DECLARE_TEXCUBEARRAY(_MainTex);
        
        // Fragment Shader
        fixed4 frag (v2f i) : COLOR0
        {
            fixed4 c = UNITY_SAMPLE_TEXCUBEARRAY(_MainTex, float4(i.uv, _SliceIndex));
            fixed4 cmip = UNITY_SAMPLE_TEXCUBEARRAY_LOD(_MainTex, float4(i.uv, _SliceIndex), _Mip);

            // Raymarching Logic (using the included functions)
            float3 ro = _WorldSpaceCameraPos;       // Ray origin (camera position)
            float3 rd = normalize(i.uv);            // Ray direction
            float dist = RaymarchTerrain(ro, rd);   // Perform raymarching

            // If raymarching hits terrain, color based on that
            if (dist < 100.0) {
                float3 hitPoint = ro + rd * dist;                   // Calculate the hit point
                float3 normal = CalculateNormal(hitPoint);          // Terrain normal
                float3 lightDir = normalize(float3(0.0, 1.0, 0.0)); // Simple overhead light

                c.rgb = Lighting(normal, lightDir);  // Apply lighting
            }
            else {    
                // Fallback to texture
                if (_Mip >= 0.0) c = cmip;
                c.rgb = DecodeHDR(c, _MainTex_HDR) * _Intensity;
                c.rgb *= exp2(_Exposure);
            }
            return c;
        }

        ENDCG
        }
    }
    Fallback Off
}
