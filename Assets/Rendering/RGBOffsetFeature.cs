using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*namespace Rendering
{
    public class RGBOffsetFeature : ScriptableRendererFeature
    {
        private Material material;
        private RenderTargetIdentiefier source;
        private RenderTargetHandle tempTexture;

        private class RenderPass : ScriptableRenderPass
        {
            public RenderPass(Material material) : base()
            {
                this.material = material;
                tempTexture.Init("TempRGBTexture");
            }
            
            public void SetSource(RenderTargetIdentifier source)
            {
                this.source = source;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("RGBOffsetFeature");
                
                RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
                cameraTextureDesc.depthBufferBits = 0;
                cmd.GetTemporaryRT(tempTexture.id, cameraTextureDesc, FilterMode.Bilinear);

                Blit(cmd, source, tempTexture.Identifier(), material, 0);
                Blit(cmd, tempTexture.Identifier(), source);
                
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(tempTexture.id);
            }
        }

        private RenderPass renderPass;
        
        public override void Create()
        {
            var material = new Material(Shader.find("Shader Graphs/Full Screen ShaderGraph"));
            this.renderPass = new Renderpass(material);

            renderPass = new RenderPass
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderPass.SetSource(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(renderPass);
        }
    }
}
*/

