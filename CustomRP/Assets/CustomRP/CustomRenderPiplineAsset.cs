using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    [CreateAssetMenu(menuName = "Rending/Custom Render Pipeline")]
    public class CustomRenderPiplineAsset : RenderPipelineAsset
    {
        [SerializeField]
        bool useDynamicBatching, useGPUInstancing, useSRPBatcher = true;
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline(useDynamicBatching , useGPUInstancing , useSRPBatcher );
        }
    }
}

