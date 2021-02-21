using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace EPOOutline
{
    public static class OutlineEffect
    {
        private static int FillRefHash = Shader.PropertyToID("_FillRef");
        private static int DilateShiftHash = Shader.PropertyToID("_DilateShift");
        private static int ColorMaskHash = Shader.PropertyToID("_ColorMask");
        private static int OutlineRefHash = Shader.PropertyToID("_OutlineRef");
        private static int RefHash = Shader.PropertyToID("_Ref");
        private static int ZWriteHash = Shader.PropertyToID("_ZWrite");
        private static int EffectSizeHash = Shader.PropertyToID("_EffectSize");
        private static int CullHash = Shader.PropertyToID("_Cull");
        private static int ZTestHash = Shader.PropertyToID("_ZTest");
        private static int ColorHash = Shader.PropertyToID("_Color");
        private static int ShiftHash = Shader.PropertyToID("_Shift");
        private static int InitialTexHash = Shader.PropertyToID("_InitialTex");
        private static int InfoBufferHash = Shader.PropertyToID("_InfoBuffer");
        private static int ComparisonHash = Shader.PropertyToID("_Comparison");
        private static int ReadMaskHash = Shader.PropertyToID("_ReadMask");
        private static int WriteMaskHash = Shader.PropertyToID("_WriteMask");
        private static int OperationHash = Shader.PropertyToID("_Operation");
        private static int CutoutThresholdHash = Shader.PropertyToID("_CutoutThreshold");
        private static int TextureIndexHash = Shader.PropertyToID("_TextureIndex");
        private static int CutoutTextureHash = Shader.PropertyToID("_CutoutTexture");
        private static int CutoutTextureSTHash = Shader.PropertyToID("_CutoutTexture_ST");
        private static int SrcBlendHash = Shader.PropertyToID("_SrcBlend");
        private static int DstBlendHash = Shader.PropertyToID("_DstBlend");

        private static int TargetHash = Shader.PropertyToID("ScreenRenderTargetTexture");
        private static int InfoTargetHash = Shader.PropertyToID("ScreenInfoRenderTargetTexture");

        private static int PrimaryBufferHash = Shader.PropertyToID("PrimaryBuffer");
        private static int HelperBufferHash = Shader.PropertyToID("HelperBuffer");

        private static int PrimaryInfoBufferHash = Shader.PropertyToID("PrimaryInfoBuffer");
        private static int HelperInfoBufferHash = Shader.PropertyToID("HelperInfoBuffer");

        private static Material TransparentBlitMaterial;
        private static Material EmptyFillMaterial;
        private static Material OutlineMaterial;
        private static Material PartialBlitMaterial;
        private static Material FillMaskMaterial;
        private static Material ZPrepassMaterial;
        private static Material OutlineMaskMaterial;
        private static Material DilateMaterial;
        private static Material EdgeDilateMaterial;
        private static Material BlurMaterial;
        private static Material FinalBlitMaterial;
        private static Material BasicBlitMaterial;
        private static Material ClearStencilMaterial;

        private struct OutlineTargetGroup
        {
            public readonly Outlinable Outlinable;
            public readonly OutlineTarget Target;

            public OutlineTargetGroup(Outlinable outlinable, OutlineTarget target)
            {
                Outlinable = outlinable;
                Target = target;
            }
        }

        private static List<OutlineTargetGroup> targets = new List<OutlineTargetGroup>();

        private static List<string> keywords = new List<string>();

        public static Material LoadMaterial(string shaderName)
        {
            return new Material(Resources.Load<Shader>(string.Format("Easy performant outline/Shaders/{0}", shaderName)));
        }

        private static void InitMaterials()
        {
            if (PartialBlitMaterial == null)
                PartialBlitMaterial = LoadMaterial("PartialBlit");

            if (OutlineMaterial == null)
                OutlineMaterial = LoadMaterial("Outline");

            if (TransparentBlitMaterial == null)
                TransparentBlitMaterial = LoadMaterial("TransparentBlit");

            if (ZPrepassMaterial == null)
                ZPrepassMaterial = LoadMaterial("ZPrepass");

            if (OutlineMaskMaterial == null)
                OutlineMaskMaterial = LoadMaterial("OutlineMask");

            if (DilateMaterial == null)
                DilateMaterial = LoadMaterial("Dilate");

            if (BlurMaterial == null)
                BlurMaterial = LoadMaterial("Blur");

            if (FinalBlitMaterial == null)
                FinalBlitMaterial = LoadMaterial("FinalBlit");

            if (BasicBlitMaterial == null)
                BasicBlitMaterial = LoadMaterial("BasicBlit");

            if (EmptyFillMaterial == null)
                EmptyFillMaterial = LoadMaterial("Fills/EmptyFill");

            if (FillMaskMaterial == null)
                FillMaskMaterial = LoadMaterial("Fills/FillMask");

            if (ClearStencilMaterial == null)
                ClearStencilMaterial = LoadMaterial("ClearStencil");

            if (EdgeDilateMaterial == null)
                EdgeDilateMaterial = LoadMaterial("EdgeDilate");
        }

        private static void Postprocess(OutlineParameters parameters, float shiftScale, bool scaleIndependent, float bufferScale, int first, int second, Material material, int iterrations, bool additionalShift, float shiftValue, ref int stencil)
        {
            if (iterrations <= 0)
                return;

            var scalingFactor = scaleIndependent ? bufferScale : 1.0f;

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);

            for (var index = 1; index <= iterrations; index++)
            {
                parameters.Buffer.SetGlobalInt(RefHash, stencil);

                var shift = (additionalShift ? (float)index : 1.0f) * scalingFactor;

                parameters.Buffer.SetGlobalVector(ShiftHash, new Vector4(shift * shiftScale, 0));
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, first), RenderTargetUtility.ComposeTarget(parameters, second), RenderTargetUtility.ComposeTarget(parameters, second), material, shiftValue, null);

                parameters.Buffer.SetGlobalVector(ShiftHash, new Vector4(0, shift * shiftScale));
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, second), RenderTargetUtility.ComposeTarget(parameters, first), RenderTargetUtility.ComposeTarget(parameters, first), material, shiftValue, null);

                stencil = (stencil + 1) & 0xFF;
            }
        }

        private static void Blit(OutlineParameters parameters, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier destinationDepth, Material material, float effectSize, CommandBuffer buffer, int pass = -1)
        {
            parameters.Buffer.SetGlobalFloat(EffectSizeHash, effectSize);
            BlitUtility.Blit(parameters, source, destination, destinationDepth, material, buffer, pass);
        }

        private static float GetBlurShift(BlurType blurType, int iterrationsCount)
        {
            switch (blurType)
            {
                case BlurType.Anisotropic:
                case BlurType.Box:
                    return (float)(iterrationsCount * 0.65f) + 1.0f;
                case BlurType.Gaussian5x5:
                    return 3.0f + iterrationsCount;
                case BlurType.Gaussian9x9:
                    return 5.0f + iterrationsCount;
                case BlurType.Gaussian13x13:
                    return 7.0f + iterrationsCount;
                default:
                    throw new ArgumentException("Unknown blur type");
            }
        }

        private static float ComputeEffectShift(OutlineParameters parameters)
        {
            var effectShift = GetBlurShift(parameters.BlurType, parameters.BlurIterrantions) * parameters.BlurShift + parameters.DilateIterrations * 4.0f * parameters.DilateShift;
            if (!parameters.ScaleIndependent)
                effectShift /= parameters.PrimaryBufferScale;

            return effectShift * 2.0f;
        }
        
        private static void PrepareTargets(OutlineParameters parameters)
        {
            targets.Clear();

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                foreach (var target in outlinable.OutlineTargets)
                {
                    var renderer = target.Renderer;
                    if (renderer == null || !renderer.gameObject.activeInHierarchy || !renderer.enabled)
                    {
                        if ((outlinable.DrawingMode & OutlinableDrawingMode.MaskOnly) == 0 || renderer == null)
                            continue;
                    }

                    targets.Add(new OutlineTargetGroup(outlinable, target));
                }
            }
        }

        public static void SetupOutline(OutlineParameters parameters)
        {
            PrepareTargets(parameters);

            Profiler.BeginSample("Setup outline");

            Profiler.BeginSample("Check materials");
            InitMaterials();
            Profiler.EndSample();

            var effectShift = ComputeEffectShift(parameters);
            var targetWidth = parameters.TargetWidth;
            var targetHeight = parameters.TargetHeight;

            parameters.Camera.forceIntoRenderTexture = parameters.EyeMask == StereoTargetEyeMask.None || !UnityEngine.XR.XRSettings.enabled || parameters.IsEditorCamera;

            parameters.Buffer.SetGlobalInt(SrcBlendHash, (int)BlendMode.One);
            parameters.Buffer.SetGlobalInt(DstBlendHash, (int)BlendMode.Zero);

            var outlineRef = 1;
            parameters.Buffer.SetGlobalInt(OutlineRefHash, outlineRef);

            SetupDilateKeyword(parameters);

            RenderTargetUtility.GetTemporaryRT(parameters, TargetHash, targetWidth, targetHeight, 24, true, false, false);
            var scaledWidth = (int)(targetWidth * parameters.PrimaryBufferScale);
            if (scaledWidth % 2 != 0)
                scaledWidth++;

            var scaledHeight = (int)(targetHeight * parameters.PrimaryBufferScale);
            if (scaledHeight % 2 != 0)
                scaledHeight++;

            RenderTargetUtility.GetTemporaryRT(parameters, PrimaryBufferHash, scaledWidth, scaledHeight, 24, true, false, false);
            RenderTargetUtility.GetTemporaryRT(parameters, HelperBufferHash, scaledWidth, scaledHeight, 24, true, false, false);

            if (parameters.UseInfoBuffer)
            {
                var scaledInfoWidth = (int)(targetWidth * Mathf.Min(parameters.PrimaryBufferScale, parameters.InfoBufferScale));
                if (scaledInfoWidth % 2 != 0)
                    scaledInfoWidth++;

                var scaledInfoHeight = (int)(targetHeight * Mathf.Min(parameters.PrimaryBufferScale, parameters.InfoBufferScale));
                if (scaledInfoHeight % 2 != 0)
                    scaledInfoHeight++;

                RenderTargetUtility.GetTemporaryRT(parameters, InfoTargetHash, targetWidth, targetHeight, 0, false, false, false);

                RenderTargetUtility.GetTemporaryRT(parameters, PrimaryInfoBufferHash, scaledInfoWidth, scaledInfoHeight, 0, true, true, false);
                RenderTargetUtility.GetTemporaryRT(parameters, HelperInfoBufferHash, scaledInfoWidth, scaledInfoHeight, 0, true, true, false);
            }

            Profiler.BeginSample("Updating mesh");
            BlitUtility.SetupMesh(parameters, effectShift);
            Profiler.EndSample();

            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, TargetHash), parameters.DepthTarget);
            DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => true, x => Color.clear, x => ZPrepassMaterial, RenderStyle.FrontBack | RenderStyle.Single, OutlinableDrawingMode.ZOnly);

            parameters.Buffer.ClearRenderTarget(false, true, Color.clear);

            var drawnOutlinablesCount = 0;
            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.Always,    false, 0.0f, x => x.OutlineParameters.Enabled, x => x.OutlineParameters.Color, 
                x => OutlineMaterial,
                RenderStyle.Single, OutlinableDrawingMode.Normal);

            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.NotEqual,  false, 0.0f, x => x.BackParameters.Enabled,    x => x.BackParameters.Color,
                x => OutlineMaterial,
                RenderStyle.FrontBack, OutlinableDrawingMode.Normal);

            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => x.FrontParameters.Enabled,   x => x.FrontParameters.Color,
                x => OutlineMaterial,
                RenderStyle.FrontBack, OutlinableDrawingMode.Normal);

            parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetEnabledInfoBufferKeyword());
            parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetWeightedAverateKeyword());

            if (parameters.UseInfoBuffer)
            {
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetInfoBufferStageKeyword());

                parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), parameters.DepthTarget);
                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);
                
                DrawOutlineables(parameters, CompareFunction.Always,    false, 0.0f, x => x.OutlineParameters.Enabled,   x => new Color(x.OutlineParameters.DilateShift * parameters.DilateShift, x.OutlineParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.Single);

                DrawOutlineables(parameters, CompareFunction.NotEqual,  false, 0.0f, x => x.BackParameters.Enabled,      x => new Color(x.BackParameters.DilateShift * parameters.DilateShift, x.BackParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.FrontBack);

                DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => x.FrontParameters.Enabled,     x => new Color(x.FrontParameters.DilateShift * parameters.DilateShift, x.FrontParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.FrontBack);

                parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);
                parameters.Buffer.SetGlobalInt(RefHash, 0);
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), RenderTargetUtility.ComposeTarget(parameters, PrimaryInfoBufferHash), RenderTargetUtility.ComposeTarget(parameters, PrimaryInfoBufferHash), BasicBlitMaterial, effectShift, null);

                var infoRef = 0;
                var bufferScale = parameters.PrimaryBufferScale * parameters.InfoBufferScale;
                //var scaleRatio = parameters.PrimaryBufferScale / bufferScale;
                Postprocess(parameters, 1.0f, false, bufferScale, PrimaryInfoBufferHash, HelperInfoBufferHash, DilateMaterial,
                    parameters.DilateIterrations + parameters.BlurIterrantions, false,
                    effectShift, ref infoRef);

                parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), parameters.DepthTarget);
                DrawOutlineables(parameters, CompareFunction.Always,    true, effectShift,  x => x.OutlineParameters.Enabled,   x => new Color(x.OutlineParameters.DilateShift * 0.5f * parameters.DilateShift, x.OutlineParameters.BlurShift * 0.5f * parameters.BlurShift, 0, 1), x => EdgeDilateMaterial, RenderStyle.Single);
                DrawOutlineables(parameters, CompareFunction.NotEqual,  true, effectShift, x => x.BackParameters.Enabled,      x => new Color(x.BackParameters.DilateShift * 0.5f * parameters.DilateShift, x.BackParameters.BlurShift * 0.5f * parameters.BlurShift, 0, 1), x => EdgeDilateMaterial, RenderStyle.FrontBack);
                DrawOutlineables(parameters, CompareFunction.LessEqual, true, effectShift, x => x.FrontParameters.Enabled,     x => new Color(x.FrontParameters.DilateShift * 0.5f * parameters.DilateShift, x.FrontParameters.BlurShift * 0.5f * parameters.BlurShift, 0, 1), x => EdgeDilateMaterial, RenderStyle.FrontBack);

                parameters.Buffer.SetGlobalTexture(InfoBufferHash, PrimaryInfoBufferHash);

                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetInfoBufferStageKeyword());
            }

            if (parameters.UseInfoBuffer)
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetEnabledInfoBufferKeyword());

            //parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetWeightedAverateKeyword());

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);

            var postProcessingRef = 0;
            if (drawnOutlinablesCount > 0)
            {
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, TargetHash),
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash),
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash),
                    BasicBlitMaterial, effectShift, null);

                Postprocess(parameters, parameters.DilateShift, parameters.ScaleIndependent, parameters.PrimaryBufferScale, PrimaryBufferHash, HelperBufferHash, DilateMaterial, parameters.DilateIterrations, false, effectShift, ref postProcessingRef);
            }

            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, TargetHash), parameters.DepthTarget);

            if (drawnOutlinablesCount > 0)
                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);

            var drawnSimpleEdgeDilateOutlinesCount = DrawOutlineables(parameters, CompareFunction.Always,   true, -1, x => x.OutlineParameters.Enabled, x => x.OutlineParameters.Color, x => EdgeDilateMaterial, RenderStyle.Single);
            drawnSimpleEdgeDilateOutlinesCount += DrawOutlineables(parameters, CompareFunction.NotEqual,    true, -1, x => x.BackParameters.Enabled,    x => x.BackParameters.Color, x => EdgeDilateMaterial, RenderStyle.FrontBack);
            drawnSimpleEdgeDilateOutlinesCount += DrawOutlineables(parameters, CompareFunction.LessEqual,   true, -1, x => x.FrontParameters.Enabled,   x => x.FrontParameters.Color, x => EdgeDilateMaterial, RenderStyle.FrontBack);

            if (drawnSimpleEdgeDilateOutlinesCount > 0)
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, TargetHash), RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), PartialBlitMaterial, effectShift, null);

            if (parameters.BlurIterrantions > 0)
            {
                SetupBlurKeyword(parameters);
                Postprocess(parameters, parameters.BlurShift, parameters.ScaleIndependent, parameters.PrimaryBufferScale, PrimaryBufferHash, HelperBufferHash, BlurMaterial, parameters.BlurIterrantions, false, effectShift, ref postProcessingRef);
            }

            DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => true, x => Color.clear, x => OutlineMaskMaterial, RenderStyle.FrontBack | RenderStyle.Single, OutlinableDrawingMode.MaskOnly);

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.NotEqual);
            parameters.Buffer.SetGlobalInt(ReadMaskHash, 255);
            parameters.Buffer.SetGlobalInt(OperationHash, (int)StencilOp.Replace);
            Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), parameters.Target, parameters.DepthTarget, FinalBlitMaterial, effectShift, null);

            DrawFill(parameters, parameters.Target);

            parameters.Buffer.SetGlobalFloat(EffectSizeHash, effectShift);
            BlitUtility.Draw(parameters, parameters.Target, parameters.DepthTarget, ClearStencilMaterial);

            parameters.Buffer.ReleaseTemporaryRT(PrimaryBufferHash);

            parameters.Buffer.ReleaseTemporaryRT(HelperBufferHash);
            parameters.Buffer.ReleaseTemporaryRT(TargetHash);

            if (parameters.UseInfoBuffer)
            {
                parameters.Buffer.ReleaseTemporaryRT(InfoBufferHash);
                parameters.Buffer.ReleaseTemporaryRT(PrimaryInfoBufferHash);
                parameters.Buffer.ReleaseTemporaryRT(HelperInfoBufferHash);
            }

            Profiler.EndSample();
        }

        private static void SetupDilateKeyword(OutlineParameters parameters)
        {
            KeywordsUtility.GetAllDilateKeywords(keywords);
            foreach (var keyword in keywords)
                parameters.Buffer.DisableShaderKeyword(keyword);

            parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetDilateQualityKeyword(parameters.DilateQuality));
        }

        private static void SetupBlurKeyword(OutlineParameters parameters)
        {
            KeywordsUtility.GetAllBlurKeywords(keywords);
            foreach (var keyword in keywords)
                parameters.Buffer.DisableShaderKeyword(keyword);

            parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetBlurKeyword(parameters.BlurType));
        }

        private static int DrawOutlineables(OutlineParameters parameters, CompareFunction function, bool edgeShiftOnly, float shift, Func<Outlinable, bool> shouldRender, Func<Outlinable, Color> colorProvider, Func<Outlinable, Material> materialProvider, RenderStyle styleMask, OutlinableDrawingMode modeMask = OutlinableDrawingMode.Normal)
        {
            var drawnCount = 0;
            parameters.Buffer.SetGlobalInt(ZTestHash, (int)function);

            foreach (var targetGroup in targets)
            {
                var outlinable = targetGroup.Outlinable;
                if ((int)(outlinable.RenderStyle & styleMask) == 0)
                    continue;

                if ((int)(outlinable.DrawingMode & modeMask) == 0)
                    continue;

                var color = shouldRender(outlinable) ? colorProvider(outlinable) : Color.clear;

                parameters.Buffer.SetGlobalColor(ColorHash, color);
                var target = targetGroup.Target;

                var postProcessing = !target.CanUseEdgeDilateShift || target.DilateRenderingMode == DilateRenderMode.PostProcessing;
                if (edgeShiftOnly && postProcessing)
                    continue;

                if (!postProcessing)
                {
                    var dilateShift = 0.0f;
                    switch (function)
                    {
                        case CompareFunction.Always:
                            dilateShift = target.EdgeDilateAmount;
                            break;
                        case CompareFunction.NotEqual:
                            dilateShift = target.BackEdgeDilateAmount;
                            break;
                        case CompareFunction.LessEqual:
                            dilateShift = target.FrontEdgeDilateAmount;
                            break;
                    }

                    parameters.Buffer.SetGlobalFloat(DilateShiftHash, shift < 0.0f ? dilateShift : shift);
                }

                parameters.Buffer.SetGlobalInt(ColorMaskHash, postProcessing ? 255 : 0);

                SetupCutout(parameters, target);
                SetupCull(parameters, target);

                if (postProcessing || edgeShiftOnly)
                    drawnCount++;

                var materialToUse = materialProvider(outlinable);
                parameters.Buffer.DrawRenderer(target.Renderer, materialToUse, target.ShiftedSubmeshIndex);
            }

            return drawnCount;
        }

        private static void DrawFill(OutlineParameters parameters, RenderTargetIdentifier targetSurfance)
        {
            parameters.Buffer.SetRenderTarget(targetSurfance, parameters.DepthTarget);

            var singleMask = 1;
            var frontMask = 2;
            var backMask = 3;

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                if (outlinable.RenderStyle == RenderStyle.FrontBack)
                {
                    if ((outlinable.BackParameters.FillPass.Material == null || !outlinable.BackParameters.Enabled) &&
                        (outlinable.FrontParameters.FillPass.Material == null || !outlinable.FrontParameters.Enabled))
                        continue;

                    parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.Greater);
                    foreach (var target in outlinable.OutlineTargets)
                    {
                        if (target.Renderer == null)
                            continue;

                        var renderer = target.Renderer;
                        if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                            continue;

                        SetupCutout(parameters, target);
                        SetupCull(parameters, target);

                        parameters.Buffer.SetGlobalInt(FillRefHash, backMask);
                        parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                    }

                    parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.LessEqual);
                    foreach (var target in outlinable.OutlineTargets)
                    {
                        if (target.Renderer == null)
                            continue;

                        var renderer = target.Renderer;
                        if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                            continue;

                        SetupCutout(parameters, target);
                        SetupCull(parameters, target);

                        parameters.Buffer.SetGlobalInt(FillRefHash, frontMask);
                        parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                    }

                    var frontMaterial = outlinable.FrontParameters.FillPass.Material;
                    parameters.Buffer.SetGlobalInt(FillRefHash, frontMask);
                    if (frontMaterial != null && outlinable.FrontParameters.Enabled)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, frontMaterial, target.ShiftedSubmeshIndex);
                        }
                    }

                    var backMaterial = outlinable.BackParameters.FillPass.Material;
                    parameters.Buffer.SetGlobalInt(FillRefHash, backMask);
                    if (backMaterial != null && outlinable.BackParameters.Enabled)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, backMaterial, target.ShiftedSubmeshIndex);
                        }
                    }
                }
                else
                {
                    if (outlinable.OutlineParameters.FillPass.Material == null)
                        continue;

                    if (!outlinable.OutlineParameters.Enabled)
                        continue;

                    parameters.Buffer.SetGlobalInt(FillRefHash, singleMask);
                    parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.Always);

                    foreach (var target in outlinable.OutlineTargets)
                    {
                        if (target.Renderer == null)
                            continue;

                        var renderer = target.Renderer;
                        if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                            continue;

                        SetupCutout(parameters, target);
                        SetupCull(parameters, target);

                        parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                    }

                    parameters.Buffer.SetGlobalInt(FillRefHash, singleMask);
                    var fillMaterial = outlinable.OutlineParameters.FillPass.Material;
                    if (FillMaskMaterial != null)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, fillMaterial, target.ShiftedSubmeshIndex);
                        }
                    }
                }
            }
        }

        private static void SetupCutout(OutlineParameters parameters, OutlineTarget target)
        {
            if (target.Renderer == null)
                return;

            if (target.Renderer is SpriteRenderer)
            {
                var spriteRenderer = target.Renderer as SpriteRenderer;
                var sprite = spriteRenderer.sprite;
                if (sprite == null)
                {
                    parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                    return;
                }

                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                parameters.Buffer.SetGlobalFloat(CutoutThresholdHash, target.CutoutThreshold);
                parameters.Buffer.SetGlobalTexture(CutoutTextureHash, spriteRenderer.sprite.texture);

                return;
            }

            var materialToGetTextureFrom = target.Renderer.sharedMaterial;

            if (target.CutoutDescriptionType != CutoutDescriptionType.None &&
                materialToGetTextureFrom != null &&
                materialToGetTextureFrom.HasProperty(target.CutoutTextureId))
            {
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                parameters.Buffer.SetGlobalFloat(CutoutThresholdHash, target.CutoutThreshold);

                var offset = materialToGetTextureFrom.GetTextureOffset(target.CutoutTextureId);
                var scale = materialToGetTextureFrom.GetTextureScale(target.CutoutTextureId);

                parameters.Buffer.SetGlobalVector(CutoutTextureSTHash, new Vector4(scale.x, scale.y, offset.x, offset.y));

                var texture = materialToGetTextureFrom.GetTexture(target.CutoutTextureId);
                if (texture == null || texture.dimension != TextureDimension.Tex2DArray)
                    parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetTextureArrayCutoutKeyword());
                else
                {
                    parameters.Buffer.SetGlobalFloat(TextureIndexHash, target.CutoutTextureIndex);
                    parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetTextureArrayCutoutKeyword());
                }

                parameters.Buffer.SetGlobalTexture(CutoutTextureHash, texture);
            }
            else
                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetCutoutKeyword());

        }

        private static void SetupCull(OutlineParameters parameters, OutlineTarget target)
        {
            parameters.Buffer.SetGlobalInt(CullHash, (int)target.CullMode);
        }
    }
}