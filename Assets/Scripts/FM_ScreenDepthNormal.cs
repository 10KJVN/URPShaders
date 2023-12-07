using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FM_ScreenDepthNormal : MonoBehaviour
{
    public Material Mat;

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
    }

    void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (Mat == null)
        {
            Mat = new Material(Shader.Find("Universal Render Pipeline/FMShader_ScreenDepthNormal"));
        }

        //pass this camera matrix data to screen shader
        Shader.SetGlobalMatrix(Shader.PropertyToID("UNITY_MATRIX_IV"), camera.cameraToWorldMatrix);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //render source to screen w/ shader
        Graphics.Blit(source, destination, Mat);
    }
}