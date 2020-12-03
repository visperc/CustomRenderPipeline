using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRender render = new CameraRender();
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {

            foreach (var camera in cameras)
            {
                render.Render(context , camera);
            }
        }
    }

}
