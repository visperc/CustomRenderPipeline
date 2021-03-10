using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRender render = new CameraRender();
        bool useDynamicBatching, useGPUInstancing;
        public CustomRenderPipeline(bool useDynamicBatching , bool useGPUInstancing , bool useSRPBatcher)
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
        }
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (var camera in cameras)
            {
                render.Render(context , camera , useDynamicBatching , useGPUInstancing);
            }
        }
    }

}
