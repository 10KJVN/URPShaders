// SDFFunctions2.hlsl
#include "PerlinNoiseFunction.hlsl"

// TERRAIN SDF
float TerrainSDF(float3 p, float frequency, float amplitude) 
{
    // Generate dune-like wave patterns with noise
    const float noiseFactor = perlinNoise(p.xz * frequency);
    const float duneHeight = sin(p.x * frequency) * sin(p.z * frequency) * (amplitude + noiseFactor);
    return p.y - duneHeight;
}

// NORMAL CALCULATION FOR TERRAIN
float3 CalculateNormal(float3 p, float eps, float frequency, float amplitude) 
{
    // Approximate gradient
    float dx = TerrainSDF(p + float3(eps, 0, 0), frequency, amplitude) - TerrainSDF(p - float3(eps, 0, 0), frequency, amplitude);
    float dy = TerrainSDF(p + float3(0, eps, 0), frequency, amplitude) - TerrainSDF(p - float3(0, eps, 0), frequency, amplitude);
    float dz = TerrainSDF(p + float3(0, 0, eps), frequency, amplitude) - TerrainSDF(p - float3(0, 0, eps), frequency, amplitude);
    return normalize(float3(dx, dy, dz));
}

// LIGHTING CALCULATION
float Lighting(float3 normal, float3 lightDirection)
{
    return max(dot(normal, lightDirection), 0.0);  // Diffuse lighting
}

// DEAD TREE SDF
float DeadTreeSDF(float3 p, float3 treePosition, float trunkHeight, float trunkRadius)
{
    // Calculate distance from the point to the trunk
    float3 relativePos = p - treePosition;
    float trunk = length(relativePos.xz) - trunkRadius;
    trunk = max(trunk, abs(relativePos.y) - trunkHeight);
    return trunk;
}

// MOON SDF
float MoonSDF(float3 p, float3 moonPosition, float moonRadius)
{
    return length(p - moonPosition) - moonRadius;
}

// COMBINED SDF FUNCTION
float CombinedSDF(float3 p, float frequency, float amplitude, float3 treePosition, float trunkHeight, float trunkRadius, float3 moonPosition, float moonRadius)
{
    float terrain = TerrainSDF(p, frequency, amplitude);
    float tree = DeadTreeSDF(p, treePosition, trunkHeight, trunkRadius);
    float moon = MoonSDF(p, moonPosition, moonRadius);
    
    return min(min(terrain, tree), moon); // Closest distance
}

// EMISSIVE CALCULATION FOR MOON
float3 CalculateEmissive(float3 p, float3 moonPosition, float moonRadius, float3 moonColor, float emissiveStrength)
{
    float moonSDF = MoonSDF(p, moonPosition, moonRadius);
    if (moonSDF < 0.001) 
    {
        return moonColor * emissiveStrength;
    }
    return float3(0, 0, 0); // No emissive if not hit
}

// RAYMARCH FUNCTION
float RaymarchTerrain(float3 ro, float3 rd, float frequency, float amplitude, float3 treePosition, float trunkHeight, float trunkRadius, float3 moonPosition, float moonRadius)
{
    float dist = 0.0;
    for (int i = 0; i < 1000; i++) 
    {
        float3 p = ro + rd * dist;
        float d = CombinedSDF(p, frequency, amplitude, treePosition, trunkHeight, trunkRadius, moonPosition, moonRadius);
        if (d < 0.001) break;
        dist += d;
        if (dist > 400.0) break;
    }
    return dist;
}
