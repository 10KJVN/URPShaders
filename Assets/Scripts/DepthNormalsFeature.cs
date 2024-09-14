using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RendererUtils;

public class DepthNormalsFeature : ScriptableRendererFeature {
    class RenderPass : ScriptableRenderPass {
        private Material material;
        private RTHandle destinationHandle;
        private List<ShaderTagId> shaderTags;
        private FilteringSettings filteringSettings; //To-Do fix this because its probably obsolete in Unity 6

        public RenderPass(Material material) : base() {
            this.material = material;
            this.shaderTags = new List<ShaderTagId>() {
                new ShaderTagId("DepthOnly"),
            };
            this.filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            destinationHandle = RTHandles.Alloc("_DepthNormalsTexture", name: "_DepthNormalsTexture");
        }

        // Configure the pass by creating a temporary render texture and readying it for rendering
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) { //To-Do fix this because its probably obsolete in Unity 6
            destinationHandle = RTHandles.Alloc(cameraTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_DepthNormalsTexture");
            ConfigureTarget(destinationHandle);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) { //To-Do fix this because its probably obsolete in Unity 6
            CommandBuffer cmd = CommandBufferPool.Get("DepthNormals Pass");

            // Create drawing settings
            var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.overrideMaterial = material;

            // Use the RendererList API
            var rendererListDesc = new RendererListDesc(shaderTags[0], renderingData.cullResults, renderingData.cameraData.camera) {
                sortingCriteria = SortingCriteria.CommonOpaque,
                renderQueueRange = RenderQueueRange.opaque,
                overrideMaterial = material
            };

            RendererList rendererList = context.CreateRendererList(rendererListDesc);
            cmd.DrawRendererList(rendererList);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            RTHandles.Release(destinationHandle);  // Properly release the RTHandle
        }
    }

    private RenderPass renderPass;

    public override void Create() {
        Material material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        this.renderPass = new RenderPass(material);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(renderPass);
    }
}
