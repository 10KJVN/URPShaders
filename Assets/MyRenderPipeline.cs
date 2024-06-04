using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

public class MyRenderPipeline : RenderPipeline
{
        
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        //Debug.Log("Render pipeline executing");
        foreach (Camera camera in cameras)
        {
            //Debug.Log($"Rendering camera: {camera.name}");

            context.SetupCameraProperties(camera);
            
            CommandBuffer cmd = new CommandBuffer { name = "Clear Render Target" };
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
                
            //var cullingParams = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParams))
                continue;

            CullingResults cullResults = context.Cull(ref cullingParams);
            //var cullResults = context.Cull(ref cullingParams);
            
            var shaderTagId = new ShaderTagId("SRPDefaultUnlit");
            var sortingSettings = new SortingSettings(camera);
            var drawSettings = new DrawingSettings( new ShaderTagId("SRPDefaultUnlit"), new SortingSettings(camera));
            var filterSettings = new FilteringSettings(RenderQueueRange.all);

            var rendererListDesc = new RendererListDesc(shaderTagId, cullResults, camera)
            {
                sortingCriteria = sortingSettings.criteria,
                renderQueueRange = filterSettings.renderQueueRange,
                layerMask = filterSettings.layerMask,
                overrideMaterial = drawSettings.overrideMaterial,
                overrideMaterialPassIndex = drawSettings.overrideMaterialPassIndex
            };

            RendererList rendererList = context.CreateRendererList(rendererListDesc);
            
            cmd = new CommandBuffer { name = "Draw Renderers" };
            cmd.DrawRendererList(rendererList);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            
            context.Submit(); 
        }
    }
}