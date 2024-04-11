using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class RGBOffsetFeature : ScriptableRendererFeature
    {
        private Material _material;

        private class RenderPass : ScriptableRenderPass
        {
            private Material _material;
            private RenderTargetIdentifier _source;
            private RenderTargetHandle _tempTexture;

            public RenderPass(Material material)
            {
                this._material = material;
                _tempTexture.Init("_TempRGBTexture");
            }

            public void SetSource(RenderTargetIdentifier source)
            {
                this._source = source;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                _tempTexture.Init("_TempRGBTexture");
                cmd.GetTemporaryRT(_tempTexture.id, cameraTextureDescriptor, FilterMode.Bilinear);
                ConfigureTarget(_tempTexture.Identifier());
                ConfigureClear(ClearFlag.All, Color.clear);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("RGBOffsetFeature");

                // Blit to the temporary texture
                cmd.Blit(_source, _tempTexture.Identifier(), _material);

                // Blit from the temporary texture back to the source
                cmd.Blit(_tempTexture.Identifier(), _source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(_tempTexture.id);
            }
        }

        private RenderPass _renderPass;

        public override void Create()
        {
            _material = new Material(Shader.Find("Shader Graphs/RGBO"));
            _renderPass = new RenderPass(_material)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _renderPass.SetSource(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(_renderPass);
        }
    }
}
