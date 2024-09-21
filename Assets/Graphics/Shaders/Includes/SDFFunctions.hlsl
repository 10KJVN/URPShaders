// SDFFunctions.hlsl

// Terrain Signed Distance Function (SDF)
float TerrainSDF(float3 p) 
{
    float height = sin(p.x * 0.1) * sin(p.z * 0.1) * 10.0;  // Sine wave-based terrain
    return p.y - height; // Distance from point p to the terrain surface
}

// Normal Calculation for SDF
float3 CalculateNormal(float3 p)
{
    float eps = 0.001;
    float dx = TerrainSDF(p + float3(eps, 0, 0)) - TerrainSDF(p - float3(eps, 0, 0));
    float dy = TerrainSDF(p + float3(0, eps, 0)) - TerrainSDF(p - float3(0, eps, 0));
    float dz = TerrainSDF(p + float3(0, 0, eps)) - TerrainSDF(p - float3(0, 0, eps));
    return normalize(float3(dx, dy, dz));
}

float3 Lighting(float3 normal, float3 lightDir) {
    return max(dot(normal, lightDir), 0.0);  // Basic diffuse lighting
}

// Raymarching Function for Terrain
float RaymarchTerrain(float3 ro, float3 rd)
{
    float dist = 0.0;
    for (int i = 0; i < 100; i++) { // MAX_STEPS can be set to a value
        float3 p = ro + rd * dist;  // Current point along the ray
        float d = TerrainSDF(p);    // Get the distance to the nearest terrain surface
        if (d < 0.001) break;       // Surface hit threshold
        dist += d;                  // Move along the ray by the distance
        if (dist > 100.0) break;    // Exit if raymarching exceeds a certain distance
    }
    return dist;
}
