using UnityEngine;
using UnityEditor;

public class CreateCubeArrayTexture : MonoBehaviour
{
    [MenuItem("CreateExamples/CubemapArray")]
    private static void CreateCubemapArray()
    {
        // Configure the cubemap array and color data
        const int faceSize = 16;
        const int arraySize = 4;
        //Int arrays
        var kCubeXRemap = new [] { 2, 2, 0, 0, 0, 0 };
        var kCubeYRemap = new [] { 1, 1, 2, 2, 1, 1 };
        var kCubeZRemap = new [] { 0, 0, 1, 1, 2, 2 };
        //floats
        var kCubeXSign = new [] { -1.0F, 1.0F, 1.0F, 1.0F, 1.0F, -1.0F };
        var kCubeYSign = new [] { -1.0F, -1.0F, 1.0F, -1.0F, -1.0F, -1.0F };
        var kCubeZSign = new [] { 1.0F, -1.0F, 1.0F, -1.0F, 1.0F, -1.0F };
        var baseCols = new [] { Color.white, new Color(1, .5f, .5f, 1), new Color(.5f, 1, .5f, 1), new Color(.5f, .5f, 1, 1), Color.gray };
        
        // Create an instance of CubemapArray
        var tex = new CubemapArray(faceSize, arraySize, TextureFormat.ARGB32, true)
        {
            filterMode = FilterMode.Trilinear
        };

        // Iterate over each cubemap
        var col = new Color[tex.width * tex.width];
        var invSize = 1.0f / tex.width;
        for (var i = 0; i < tex.cubemapCount; ++i)
        {
            var baseCol = baseCols[i % baseCols.Length];

            // Iterate over each face of the current cubemap
            for (var face = 0; face < 6; ++face)
            {
                var idx = 0;
                var signScale = new Vector3(kCubeXSign[face], kCubeYSign[face], kCubeZSign[face]);
                
                // Iterate over each pixel of the current face
                for (var y = 0; y < tex.width; ++y)
                {
                    for (var x = 0; x < tex.width; ++x)
                    {
                        // Calculate a "normal direction" color for the current pixel
                        var uvDir = new Vector3(x * invSize * 2.0f - 1.0f, y * invSize * 2.0f - 1.0f, 1.0f);
                        uvDir = uvDir.normalized;
                        uvDir.Scale(signScale);
                        var dir = Vector3.zero;
                        dir[kCubeXRemap[face]] = uvDir[0];
                        dir[kCubeYRemap[face]] = uvDir[1];
                        dir[kCubeZRemap[face]] = uvDir[2];

                        // Shift the color into the 0.4..1.0 range
                        var c = new Color(dir.x * 0.3f + 0.7f, dir.y * 0.3f + 0.7f, dir.z * 0.3f + 0.7f, 1.0f);
                        
                        // Add a pattern to some pixels, so that mipmaps are more clearly visible
                        if (((x ^ y) & 3) == 1)
                            c *= 0.5f;
                        
                        // Tint the color with the baseCol tint
                        col[idx] = baseCol * c;
                        ++idx;
                    }
                }

                // Copy the color values for this face to the texture
                tex.SetPixels(col, (CubemapFace)face, i);
            }
        }

        // Apply the changes to the texture and upload the updated texture to the GPU
        tex.Apply();        

        // Save the texture to your Unity Project
        AssetDatabase.CreateAsset(tex, "Assets/ExampleCubemapArray.asset");
        AssetDatabase.SaveAssets();
    }
}
