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
        // Set camera parameters
        computeShader.SetFloat("_FieldOfView", mainCamera.fieldOfView);
        computeShader.SetFloat("_AspectRatio", (float)Screen.width / Screen.height);
        computeShader.SetVector("_CameraPosition", mainCamera.transform.position);
        computeShader.SetVector("_CameraForward", mainCamera.transform.forward);
        
        // Dispatch the compute shader (number of thread groups based on screen size)
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        // Assign the output texture to a material for display
        GetComponent<Renderer>().material.mainTexture = outputTexture;
    }
}
