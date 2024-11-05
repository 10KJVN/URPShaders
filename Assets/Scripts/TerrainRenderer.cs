using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture outputTexture;
    
    private Camera _mainCamera;
    private static readonly int Result = Shader.PropertyToID("Result");
    private static readonly int FieldOfView = Shader.PropertyToID("_FieldOfView");
    private static readonly int AspectRatio = Shader.PropertyToID("_AspectRatio");
    private static readonly int CameraPosition = Shader.PropertyToID("_CameraPosition");
    private static readonly int CameraForward = Shader.PropertyToID("_CameraForward");
    private static readonly int CameraRight = Shader.PropertyToID("_CameraRight");
    private static readonly int CameraUp = Shader.PropertyToID("_CameraUp");

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

    // Update is called once per frame
    void Update()
    {
        // Set camera parameters
        computeShader.SetFloat(FieldOfView, _mainCamera.fieldOfView);
        computeShader.SetFloat(AspectRatio, (float)Screen.width / Screen.height);
        computeShader.SetVector(CameraPosition, _mainCamera.transform.position);
        computeShader.SetVector(CameraForward, _mainCamera.transform.forward);
        computeShader.SetVector(CameraRight, _mainCamera.transform.right);
        computeShader.SetVector(CameraUp, _mainCamera.transform.up);

        // Set the output texture for both _Result and Result
        int kernelHandle = computeShader.FindKernel("CSMain");

        // Bind texture for result output
        computeShader.SetTexture(kernelHandle, Result, outputTexture);

        // Dispatch the compute shader (number of thread groups based on screen size)
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);

        // Assign the output texture to a material for display
        GetComponent<Renderer>().material.mainTexture = outputTexture;
    }
}