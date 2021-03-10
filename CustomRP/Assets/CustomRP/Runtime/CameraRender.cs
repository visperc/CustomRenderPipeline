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

#if UNITY_EDITOR
        string SampleName { get; set; }
#else
        const string SampleName = bufferName;
#endif

        CullingResults cullingResults;

        public void Render(ScriptableRenderContext context , Camera camera , bool useDynamicBatch , bool useGPUInstancing)
        {
            this.context = context;
            this.camera = camera;
#if UNITY_EDITOR
            PrepareBuffer();
            PrepareForSceneWindow();
#endif

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry(useDynamicBatch , useGPUInstancing);
#if UNITY_EDITOR
            DrawUnsupportedShaders();
#endif
            Submit();
        }

        void Setup()
        {
            CameraClearFlags flags = camera.clearFlags;
            buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color, flags == CameraClearFlags.Color ? camera.backgroundColor.linear: Color.clear);

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
        void DrawVisibleGeometry(bool useDynamicBatch , bool useGPUInstancing)
        {
            //绘制不透明物体（从前往后）
            //绘制天空盒
            //绘制透明物体（从后往前）

            var sortingSettings = new SortingSettings(camera) { 
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings) {
                enableInstancing = useGPUInstancing,
                enableDynamicBatching = useDynamicBatch,
            };
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
            buffer.EndSample(SampleName);
            ExecuteBuffer();

            context.Submit();
        }


    }

}
