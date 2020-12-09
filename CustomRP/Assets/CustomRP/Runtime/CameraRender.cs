using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public partial class CameraRender
    {
        static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");


        const string bufferName = "Render Camera";

        ScriptableRenderContext context;
        Camera camera;
        CommandBuffer buffer = new CommandBuffer() {
            name = bufferName
        };

        CullingResults cullingResults;

        public void Render(ScriptableRenderContext context , Camera camera)
        {
            this.context = context;
            this.camera = camera;
#if UNITY_EDITOR
            PrepareForSceneWindow();
#endif

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry();
#if UNITY_EDITOR
            DrawUnsupportedShaders();
#endif
            Submit();
        }

        void Setup()
        {
            buffer.ClearRenderTarget(true, true, Color.clear);

            context.SetupCameraProperties(camera);
            buffer.BeginSample(bufferName);

            ExecuteBuffer();

        }

        bool Cull()
        {
            if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                cullingResults = context.Cull(ref p);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 绘制可见的几何体
        /// </summary>
        void DrawVisibleGeometry()
        {
            //绘制不透明物体（从前往后）
            //绘制天空盒
            //绘制透明物体（从后往前）

            var sortingSettings = new SortingSettings(camera) { 
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(unlitShaderTagId , sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(cullingResults , ref drawingSettings , ref filteringSettings);

            context.DrawSkybox(camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;

            context.DrawRenderers(cullingResults , ref drawingSettings , ref filteringSettings);

        }



        void ExecuteBuffer()
        {
            context.ExecuteCommandBuffer(buffer);
            buffer.Clear();
        }

        void Submit()
        {
            buffer.EndSample(bufferName);
            ExecuteBuffer();

            context.Submit();
        }


    }

}
