using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class FM_ScreenDepthNormal : MonoBehaviour
{
    public Camera Cam;
    
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
    }

    private void OnRendeRImage(RenderTexture source, RenderTexture destination)
    {
        
    }
    
}
