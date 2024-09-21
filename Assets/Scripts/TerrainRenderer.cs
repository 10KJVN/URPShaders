using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture outputTexture;
    
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Set up the output texture
        outputTexture = new RenderTexture(Screen.width, Screen.height, 24);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
