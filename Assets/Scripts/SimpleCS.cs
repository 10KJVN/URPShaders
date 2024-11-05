using UnityEngine;

public class SimpleCS : MonoBehaviour
{
    public ComputeShader simpleComputeShader;
    public RenderTexture outputTexture;
    
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;

        // Set up the output texture
        outputTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat)
        {
            enableRandomWrite = true
        };
        outputTexture.Create();
    }

    void Update()
    {
        int kernelHandle = simpleComputeShader.FindKernel("CSMain");
    
        // Set the output texture
        simpleComputeShader.SetTexture(kernelHandle, "_Result", outputTexture);

        // Dispatch the kernel
        simpleComputeShader.Dispatch(kernelHandle, Mathf.CeilToInt(Screen.width / 8.0f), Mathf.CeilToInt(Screen.height / 8.0f), 1);

        // Display the result
        GetComponent<Renderer>().material.mainTexture = outputTexture;
    }

}
