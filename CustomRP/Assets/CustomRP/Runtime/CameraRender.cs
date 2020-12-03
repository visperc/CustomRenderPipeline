using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public class CameraRender
    {
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

        void DrawVisibleGeometry()
        {

            context.DrawSkybox(camera);

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
