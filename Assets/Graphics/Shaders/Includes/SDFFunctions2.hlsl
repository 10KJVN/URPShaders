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

float RaymarchTerrain(float3 ro, float3 rd)
{
    float dist = 0.0;
    for (int i = 0; i < 1000; i++) 
    {
        float3 p = ro + rd * dist;  // Current point along the ray

        // Compute distances to the terrain and a tree at position (10,0,10)
        float terrainDist = TerrainSDF(p);
        float treeDist = DeadTreeSDF(p, float3(10, 0, 10));  // You can randomize or specify tree positions

        // Combine terrain and tree SDFs by taking the minimum distance
        float d = min(terrainDist, treeDist);
        
        if (d < 0.001) break;       // Surface hit threshold
        dist += d;                  // Move along the ray by the distance
        if (dist > 100.0) break;    // Exit if too far away
    }
    return dist;
}