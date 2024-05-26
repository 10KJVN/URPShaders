using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

public class MyRenderPipeline : RenderPipeline
{

    public MyRenderPipeline()
    {
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        
        foreach (Camera camera in cameras)
        {
            context.SetupCameraProperties(camera);
            
            CommandBuffer cmd = new CommandBuffer { name = "Clear Render Target" };
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            
            // Create a RendererList
            var cullingParams = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out cullingParams))
                continue;

            var cullResults = context.Cull(ref cullingParams);
            
            var shaderTagId = new ShaderTagId("SRPDefaultUnlit");
            var sortingSettings = new SortingSettings(camera);
            var drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), new SortingSettings(camera)); 
            var filterSettings = new FilteringSettings(RenderQueueRange.all);

            var rendererListDesc = new RendererListDesc(shaderTagId, cullResults, camera)
            {
                sortingCriteria = drawSettings.sortingSettings.criteria,
                renderQueueRange = filterSettings.renderQueueRange,
                layerMask = filterSettings.layerMask,
            };

            var rendererList = context.CreateRendererList(rendererListDesc);
            
            cmd = new CommandBuffer { name = "Draw Renderers" };
            cmd.DrawRendererList(rendererList);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            
            context.Submit();
        }
    }
}       