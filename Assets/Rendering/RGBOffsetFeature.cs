using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class RGBOffsetFeature : ScriptableRendererFeature
    {
        private Material _material;
        private RenderTargetIdentifier _source;
        private RTHandle _tempTexture;

        private class RenderPass : ScriptableRenderPass
        {
            private Material _material;
            private RenderTargetIdentifier _source;
            private RTHandle _tempTexture;

            public RenderPass(Material material)
            {
                this._material = material;
            }

            public void SetSource(RenderTargetIdentifier source)
            {
                this._source = source;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("RGBOffsetFeature");

                RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
                cameraTextureDesc.depthBufferBits = 0;

                // Allocate RTHandle
                _tempTexture = RTHandles.Alloc(cameraTextureDesc);

                cmd.Blit(_source, _tempTexture, _material);
                cmd.Blit(_tempTexture, _source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

                // Release the RTHandle
                RTHandles.Release(_tempTexture);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                // No need to release the RTHandle here; it's released after Execute
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
