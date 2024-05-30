using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/My Render Pipeline")]
public class MyRenderPipelineAsset : RenderPipelineAsset<MyRenderPipeline>
{
    protected override RenderPipeline CreatePipeline()
    {
        return new MyRenderPipeline();
    }
   
} 


