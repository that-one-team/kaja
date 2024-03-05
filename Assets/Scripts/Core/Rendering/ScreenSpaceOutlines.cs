using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{
    [System.Serializable]
    class ViewSpaceNormalsTextureSettings
    {
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public Color backgroundColor;
    }

    private class ViewSpaceNormalsTexturePass : ScriptableRenderPass
    {
        readonly RTHandle _normals;
        readonly List<ShaderTagId> _idList;
        readonly ViewSpaceNormalsTextureSettings _settings;
        readonly Material _normalMat;

        public ViewSpaceNormalsTexturePass(RenderPassEvent evt, ViewSpaceNormalsTextureSettings settings)
        {
            _idList = new() {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit"),
            };
            _normalMat = new(Shader.Find("Hidden/ViewSpaceNormalsShader"));

            renderPassEvent = evt;
            _settings = settings;
            _normals = RTHandles.Alloc("_SceneViewSpaceNormals", name: "_SceneViewSpaceNormals");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var texDesc = cameraTextureDescriptor;
            texDesc.colorFormat = _settings.colorFormat;
            texDesc.depthBufferBits = _settings.depthBufferBits;

            cmd.GetTemporaryRT(Shader.PropertyToID(_normals.name), texDesc, FilterMode.Point);
            ConfigureTarget(_normals);
            ConfigureClear(ClearFlag.All, _settings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!_normalMat) return;
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                var settings = CreateDrawingSettings(_idList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                settings.overrideMaterial = _normalMat;
                var filter = FilteringSettings.defaultValue;
                context.DrawRenderers(renderingData.cullResults, ref settings, ref filter);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(_normals.name));
        }
    }
    private class ScreenSpaceOutlinesPass : ScriptableRenderPass
    {
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }
    }

    [SerializeField] RenderPassEvent _renderPassEvent;

    ViewSpaceNormalsTexturePass _normalsPass;
    [SerializeField] ViewSpaceNormalsTextureSettings _normalsSettings;
    ScreenSpaceOutlinesPass _outlinesPass;

    public override void Create()
    {
        _normalsPass = new(_renderPassEvent, _normalsSettings);
        // _outlinesPass = new(_renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
    }
}
