//SDFFunctions2.hlsl

float TerrainSDF(float3 p) 
{
    // Control the frequency and amplitude of the sine waves
    float duneHeight = sin(p.x * 0.1) * sin(p.z * 0.1) * 5.0; // Dune-like wave patterns
    return p.y - duneHeight; // SDF of the terrain surface
}

/*float TerrainSDF(float3 p) 
{
    // Use noise to vary the dunes
    float noiseFactor = perlinNoise(p.xz * 0.1);  // Apply Perlin noise to modulate the dunes
    float duneHeight = sin(p.x * 0.05) * sin(p.z * 0.05) * (5.0 + noiseFactor); 
    return p.y - duneHeight;
}*/

float3 LightDirection = normalize(float3(0.0, 1.0, 0.0)); // Light coming from above (overhead light)

float3 CalculateNormal(float3 p)
{
    float eps = 0.001;
    float dx = TerrainSDF(p + float3(eps, 0, 0)) - TerrainSDF(p - float3(eps, 0, 0));
    float dy = TerrainSDF(p + float3(0, eps, 0)) - TerrainSDF(p - float3(0, eps, 0));
    float dz = TerrainSDF(p + float3(0, 0, eps)) - TerrainSDF(p - float3(0, 0, eps));
    return normalize(float3(dx, dy, dz));
}

float3 Lighting(float3 normal)
{
    return max(dot(normal, LightDirection), 0.0);  // Diffuse lighting
}

float RaymarchTerrain(float3 ro, float3 rd)
{
    float dist = 0.0;
    for (int i = 0; i < 1000; i++) 
    {
        float3 p = ro + rd * dist;  // Current point along the ray
        float d = TerrainSDF(p);    // Get the distance to the nearest terrain surface
        if (d < 0.001) break;       // Surface hit threshold
        dist += d;                  // Move along the ray by the distance
        if (dist > 100.0) break;    // Exit if raymarching exceeds a certain distance
    }
    return dist;
}

float3 DeadTreeSDF(float3 p)
{
    // Simple SDF for tree trunk or branches (cylindrical shapes)
    return length(p.xz) - 1.0; // Cylinder SDF
}

