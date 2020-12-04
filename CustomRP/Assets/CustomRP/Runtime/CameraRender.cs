using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public class CameraRender
    {
        static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        //legacy shader pass
        static ShaderTagId[] legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };

        static Material errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));

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

            if (!Cull())
            {
                return;
            }

            Setup();
            DrawVisibleGeometry();
            DrawUnsupportedShaders();

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

        //绘制不支持的pass
        void DrawUnsupportedShaders()
        {
            var drawingSettings = new DrawingSettings();
            drawingSettings.overrideMaterial = errorMaterial;
            for (int i = 0; i < legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }

            var filteringSettings = FilteringSettings.defaultValue;
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
