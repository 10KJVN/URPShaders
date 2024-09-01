// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Required for RenderGraph

public class DepthNormalsFeature : ScriptableRendererFeature {
    class RenderPass : ScriptableRenderPass {

        private Material material;
        private RTHandle destinationHandle; 
        private List<ShaderTagId> shaderTags;
        private FilteringSettings filteringSettings;

        public RenderPass(Material material) : base() {
            this.material = material;
            this.shaderTags = new List<ShaderTagId>() {
                new ShaderTagId("DepthOnly"),
            };
            this.filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            destinationHandle = RTHandles.Alloc("_DepthNormalsTexture", name: "_DepthNormalsTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            destinationHandle = RTHandles.Alloc(cameraTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_DepthNormalsTexture");
            ConfigureTarget(destinationHandle);  
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.overrideMaterial = material;
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }

        public override void FrameCleanup(CommandBuffer cmd) {
            RTHandles.Release(destinationHandle);
        }

        /* New method to implement Render Graph
        public override void RecordRenderGraph(UnityEngine.Rendering.RenderGraphModule.RenderGraph renderGraph, FrameResources frameResources, ref RenderingData renderingData) {
            // Create a RenderGraph pass
            using (var builder = renderGraph.AddRenderPass<PassData>("DepthNormals Pass", out var passData)) {
                passData.material = material;
                passData.filteringSettings = filteringSettings;

                builder.SetRenderFunc((PassData data, UnityEngine.Rendering.RenderGraphModule.RenderGraphContext rgContext) => {
                    var cmd = rgContext.cmd;
                    var drawSettings = CreateDrawingSettings(shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                    drawSettings.overrideMaterial = data.material;
                    rgContext.renderContext.DrawRenderers(renderingData.cullResults, ref drawSettings, ref data.filteringSettings);
                });
            }
        }*/

        private class PassData {
            public Material material;
            public FilteringSettings filteringSettings;
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
