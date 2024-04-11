using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TintRendererFeature : ScriptableRendererFeature
{ 
    private TintPass tintPass;
     
    public override void Create()
    {
        tintPass = new TintPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(tintPass);
    }

    private class TintPass : ScriptableRenderPass
    {
        private readonly Material _mat;
        int tintId = Shader.PropertyToID("_Temp");
        RenderTargetIdentifier src, tint;

        public TintPass()
        {
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("CustomPost/ScreenTint");
            }
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            src = renderingData.cameraData.renderer.cameraColorTargetHandle;
            cmd.GetTemporaryRT(tintId, desc, FilterMode.Bilinear);
            tint = new RenderTargetIdentifier(tintId);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("TintRenderFeature");
            VolumeStack volumes = VolumeManager.instance.stack;
            CustomPostScreenTint tintData = volumes.GetComponent<CustomPostScreenTint>();
            if (tintData.IsActive())
            {
                _mat.SetColor("_OverlayColor", (Color)tintData.tintColor);
                _mat.SetFloat("_OverlayIntensity", (float)tintData.tintIntensity);

                cmd.Blit(src, tint, _mat, 0);
                cmd.Blit(tint, src);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tintId);
        }
    }
}