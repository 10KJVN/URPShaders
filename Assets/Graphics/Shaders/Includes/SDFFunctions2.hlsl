//SDFFunctions2.hlsl
#include "PerlinNoiseFunction.hlsl"

/*float TerrainSDF(float3 p) 
{
    // Control the frequency and amplitude of the sine waves
    float duneHeight = sin(p.x * 0.1) * sin(p.z * 0.1) * 5.0; // Dune-like wave patterns
    return p.y - duneHeight; // SDF of the terrain surface
}*/ //Simple Dune Waves

float TerrainSDF(float3 p) 
{
    // Use noise to vary the dunes
    float noiseFactor = perlinNoise(p.xz * 0.1);  // Apply Perlin noise to modulate the dunes
    float duneHeight = sin(p.x * 0.05) * sin(p.z * 0.05) * (5.0 + noiseFactor); 
    return p.y - duneHeight;
}

//float3 LightDirection = normalize(float3(0.0, 1.0, 0.0)); // Light coming from above (overhead light)

float3 CalculateNormal(float3 p)
{
    float eps = 0.001;
    float dx = TerrainSDF(p + float3(eps, 0, 0)) - TerrainSDF(p - float3(eps, 0, 0));
    float dy = TerrainSDF(p + float3(0, eps, 0)) - TerrainSDF(p - float3(0, eps, 0));
    float dz = TerrainSDF(p + float3(0, 0, eps)) - TerrainSDF(p - float3(0, 0, eps));
    return normalize(float3(dx, dy, dz));
}

float3 Lighting(float3 normal, float3 lightDirection)
{
    //return max(dot(normal, LightDirection), 0.0);  // Diffuse lighting
    return max(dot(normal, lightDirection), 0.0);
}

float3 DeadTreeSDF(float3 p, float3 treePosition)
{
    float trunkHeight = 10.0;  // Height of the tree trunk
    float trunkRadius = 0.5;   // Radius of the tree trunk
    
    // Translate position relative to the tree's position
    p -= treePosition;
    
    // Tree trunk as a vertical cylinder (centered at treePosition, extending upward)
    float trunk = length(p.xz) - trunkRadius; // Distance from the tree center on xz-plane
    
    // Limit the height of the tree trunk
    trunk = max(trunk, abs(p.y) - trunkHeight);
    
    return trunk;  // SDF for the tree trunk
}

float MoonSDF(float3 p, float3 moonPosition, float moonRadius)
{
    // SDF for a sphere (moon) at a specific position with a given radius
    return length(p - moonPosition) - moonRadius;
}

// Combine the SDFs
float CombinedSDF(float3 p)
{
    // Combine terrain, tree, and moon SDFs
    float terrain = TerrainSDF(p);
    
    // Example tree at a random position
    float3 treePosition = float3(10.0, 0.0, 20.0);
    float tree = DeadTreeSDF(p, treePosition);

    // Example moon with random position and radius
    float3 moonPosition = float3(50.0, 30.0, -100.0); // Randomized position in the sky
    float moonRadius = 10.0;  // Example moon size
    float moon = MoonSDF(p, moonPosition, moonRadius);

    // Return the minimum distance (closest surface) for rendering
    return min(min(terrain, tree), moon);
}

float3 CalculateEmissive(float3 p, float3 moonPosition, float moonRadius, float3 moonColor, float emissiveStrength)
{
    float moonSDF = MoonSDF(p, moonPosition, moonRadius);
    
    // If the ray hits the moon (SDF close to 0), make it emissive
    if (moonSDF < 0.001)
    {
        return moonColor * emissiveStrength;
    }
    
    return float3(0, 0, 0); // No emissive if not hit
}

float RaymarchTerrain(float3 ro, float3 rd)
{
    float dist = 0.0;
    for (int i = 0; i < 1000; i++) 
    {
        float3 p = ro + rd * dist;  // Current point along the ray
        float d = CombinedSDF(p);   // Get the distance to the nearest surface
        if (d < 0.001) break;       // Surface hit threshold
        dist += d;                  // Move along the ray by the distance
        if (dist > 200.0) break;    // Exit if too far away
    }
    return dist;
}