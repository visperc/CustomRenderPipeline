using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    [CreateAssetMenu(menuName = "Rending/Custom Render Pipeline")]
    public class CustomRenderPiplineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline();
        }
    }
}

