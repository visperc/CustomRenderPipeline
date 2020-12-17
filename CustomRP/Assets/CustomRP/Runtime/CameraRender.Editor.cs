using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Visperc.CRP
{
    public partial class CameraRender
    {
#if UNITY_EDITOR
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
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        }

        void PrepareForSceneWindow()
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
        }

        void PrepareBuffer()
        {
            buffer.name = SampleName = camera.name;
        }

    }
#endif
}
