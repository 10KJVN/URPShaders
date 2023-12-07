using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class FM_ScreenDepthNormal : MonoBehaviour
{
    public Camera Cam;
    public Material Mat;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (Cam == null)
        {
            Cam = this.GetComponent<Camera>();
            Cam.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        if (Mat == null)
        {  
            //assign shader "Hidden/FMShader_ScreenDepthNormal" to Mat
            Mat = new Material(Shader.Find("Shaders/FMShader_ScreenDepthNormal"));
        }
    }

    private void OnPreRender()
    {
        //pass this camera matrix data to screen shader
        Shader.SetGlobalMatrix(Shader.PropertyToID("UNITY_MATRIX_IV"), Cam.cameraToWorldMatrix);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Graphics.Blit(source, destination);
        
        //render source to screen w/ shader
        Graphics.Blit(source, destination, Mat);
    }
}