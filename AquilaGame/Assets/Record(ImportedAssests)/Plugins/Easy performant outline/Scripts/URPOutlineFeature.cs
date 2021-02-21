using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using EPOOutline;
using System;

#if URP_OUTLINE && UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif

public class URPOutlineFeature : ScriptableRendererFeature
{
    private class SRPOutline : ScriptableRenderPass
    {
        private static List<Outlinable> temporaryOutlinables = new List<Outlinable>();

        public ScriptableRenderer Renderer;

        public bool UseColorTargetForDepth;

        public Outliner Outliner;

        public OutlineParameters Parameters = new OutlineParameters();

        private List<Outliner> outliners = new List<Outliner>();

        public SRPOutline()
        {
            Parameters.CheckInitialization();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera;

            var outlineEffect = Outliner;
            if (outlineEffect == null || !outlineEffect.enabled)
                return;

#if UNITY_EDITOR
            Parameters.Buffer.name = renderingData.cameraData.camera.name;
#endif

            Outlinable.GetAllActiveOutlinables(renderingData.cameraData.camera, Parameters.OutlinablesToRender);
            RendererFilteringUtility.Filter(renderingData.cameraData.camera, Parameters);

            Outliner.UpdateSharedParameters(Parameters, renderingData.cameraData.camera, renderingData.cameraData.isSceneViewCamera);

            var targetTexture = camera.targetTexture == null ? camera.activeTexture : camera.targetTexture;

            if (UnityEngine.XR.XRSettings.enabled
                    && !Parameters.IsEditorCamera
                    && Parameters.EyeMask != StereoTargetEyeMask.None)
            {
                var descriptor = UnityEngine.XR.XRSettings.eyeTextureDesc;
                Parameters.TargetWidth = descriptor.width;
                Parameters.TargetHeight = descriptor.height;
            }
            else
            { 
                Parameters.TargetWidth = targetTexture != null ? targetTexture.width : (int)(camera.scaledPixelWidth * renderingData.cameraData.renderScale);
                Parameters.TargetHeight = targetTexture != null ? targetTexture.height : (int)(camera.scaledPixelHeight * renderingData.cameraData.renderScale);
            }

            Parameters.Antialiasing = renderingData.cameraData.cameraTargetDescriptor.msaaSamples;

            var useCustomRenderTarget = Outliner.HasCutomRenderTarget && !renderingData.cameraData.isSceneViewCamera;
            Parameters.Target = RenderTargetUtility.ComposeTarget(Parameters, useCustomRenderTarget ? Outliner.GetRenderTarget(Parameters) : Renderer.cameraColorTarget);
            Parameters.DepthTarget =
#if UNITY_2019_3_OR_NEWER && !UNITY_2019_3_0 && !UNITY_2019_3_1 && !UNITY_2019_3_2 && !UNITY_2019_3_3 && !UNITY_2019_3_4 && !UNITY_2019_3_5 && !UNITY_2019_3_6 && !UNITY_2019_3_7 && !UNITY_2019_3_8
            RenderTargetUtility.ComposeTarget(Parameters, UseColorTargetForDepth ? Renderer.cameraColorTarget :
#if UNITY_2020_2_OR_NEWER
                Renderer.cameraDepthTarget);
#else
                Renderer.cameraDepth);
#endif
#else
                RenderTargetUtility.ComposeTarget(Parameters, Renderer.cameraColorTarget);
#endif

            Parameters.Buffer.Clear();
            if (Outliner.RenderingStrategy == OutlineRenderingStrategy.Default)
            {
                OutlineEffect.SetupOutline(Parameters);
                Parameters.BlitMesh = null;
                Parameters.MeshPool.ReleaseAllMeshes();
            }
            else
            {
                temporaryOutlinables.Clear();
                temporaryOutlinables.AddRange(Parameters.OutlinablesToRender);

                Parameters.OutlinablesToRender.Clear();
                Parameters.OutlinablesToRender.Add(null);

                foreach (var outlinable in temporaryOutlinables)
                {
                    Parameters.OutlinablesToRender[0] = outlinable;
                    OutlineEffect.SetupOutline(Parameters);
                    Parameters.BlitMesh = null;
                }

                Parameters.MeshPool.ReleaseAllMeshes();
            }

            context.ExecuteCommandBuffer(Parameters.Buffer);
        }
    }

    private class Pool
    {
        private Stack<SRPOutline> outlines = new Stack<SRPOutline>();

        private List<SRPOutline> createdOutlines = new List<SRPOutline>();

        public SRPOutline Get()
        {
            if (outlines.Count == 0)
            {
                outlines.Push(new SRPOutline());
                createdOutlines.Add(outlines.Peek());
            }

            return outlines.Pop();
        }

        public void ReleaseAll()
        {
            outlines.Clear();
            foreach (var outline in createdOutlines)
                outlines.Push(outline);
        }
    }

    private GameObject lastSelectedCamera;

    private Pool outlinePool = new Pool();

    private List<Outliner> outliners = new List<Outliner>();

    private bool GetOutlinersToRenderWith(RenderingData renderingData, List<Outliner> outliners)
    {
        outliners.Clear();
        var camera = renderingData.cameraData.camera.gameObject;
        camera.GetComponents(outliners);
        if (outliners.Count == 0)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.isSceneViewCamera)
            {
                var foundObject = Array.Find(
                    Array.ConvertAll(UnityEditor.Selection.gameObjects, x => x.GetComponent<Outliner>()),
                    x => x != null);

                camera = foundObject?.gameObject ?? lastSelectedCamera;

                if (camera == null)
                    return false;
                else
                    camera.GetComponents(outliners);
            }
            else
                return false;
#else
                return false;
#endif
        }

        var hasOutliners = outliners.Count > 0;
        if (hasOutliners)
            lastSelectedCamera = camera;

        return hasOutliners;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!GetOutlinersToRenderWith(renderingData, outliners))
            return;

#if UNITY_2019_3_OR_NEWER && !UNITY_2019_3_0 && !UNITY_2019_3_1 && !UNITY_2019_3_2 && !UNITY_2019_3_3 && !UNITY_2019_3_4 && !UNITY_2019_3_5 && !UNITY_2019_3_6 && !UNITY_2019_3_7 && !UNITY_2019_3_8
        var additionalCameraData = renderingData.cameraData.camera.GetUniversalAdditionalCameraData();
        var activeStackCount = 0;
        if (additionalCameraData != null)
        {
            var stack = additionalCameraData.renderType == CameraRenderType.Overlay ? null : additionalCameraData.cameraStack;
            if (stack != null)
            {
                foreach (var camera in stack)
                {
                    if (camera != null && camera.isActiveAndEnabled)
                        activeStackCount++;
                }
            }
        }
#endif

        var shouldUseDepthTarget = renderingData.cameraData.requiresDepthTexture && renderingData.cameraData.cameraTargetDescriptor.msaaSamples <= 1 && !renderingData.cameraData.isSceneViewCamera;

        foreach (var outliner in outliners)
        {
            var outline = outlinePool.Get();

            outline.Outliner = outliner;

            outline.UseColorTargetForDepth =
#if UNITY_2019_3_OR_NEWER && !UNITY_2019_3_0 && !UNITY_2019_3_1 && !UNITY_2019_3_2 && !UNITY_2019_3_3 && !UNITY_2019_3_4 && !UNITY_2019_3_5 && !UNITY_2019_3_6 && !UNITY_2019_3_7 && !UNITY_2019_3_8
                (additionalCameraData == null || activeStackCount == 0 && additionalCameraData.renderType != CameraRenderType.Overlay) &&
#endif
                !shouldUseDepthTarget;

            outline.Renderer = renderer;

            outline.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            renderer.EnqueuePass(outline);
        }

        outlinePool.ReleaseAll();
    }

    public override void Create()
    {
    }
}
#endif