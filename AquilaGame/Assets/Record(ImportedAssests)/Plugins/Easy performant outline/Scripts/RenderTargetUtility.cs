using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace EPOOutline
{
    public static class RenderTargetUtility
    {
        private static RenderTextureFormat? hdrFormat = null;

        public static int GetDepthSliceForEye(StereoTargetEyeMask mask)
        {
            switch (mask)
            {
                case StereoTargetEyeMask.Both:
                    return -1;
                case StereoTargetEyeMask.None:
                case StereoTargetEyeMask.Left:
                    return 0;
                case StereoTargetEyeMask.Right:
                    return 1;
                default:
                    throw new ArgumentException("Unknown mode");
            }
        }

        public struct RenderTextureInfo
        {
            public readonly RenderTextureDescriptor Descriptor;
            public readonly FilterMode FilterMode;

            public RenderTextureInfo(RenderTextureDescriptor descriptor, FilterMode filterMode)
            {
                Descriptor = descriptor;
                FilterMode = filterMode;
            }
        }

        public static RenderTargetIdentifier ComposeTarget(OutlineParameters parameters, RenderTargetIdentifier target)
        {
            return new RenderTargetIdentifier(target, 0, CubemapFace.Unknown, GetDepthSliceForEye(parameters.EyeMask));
        }

        public static bool IsUsingVR(OutlineParameters parameters)
        {
            return UnityEngine.XR.XRSettings.enabled
                && !parameters.IsEditorCamera
                && parameters.EyeMask != StereoTargetEyeMask.None;
        }

        public static RenderTextureInfo GetTargetInfo(OutlineParameters parameters, int width, int height, int depthBuffer, bool forceNoAA, bool noFiltering)
        {
            var filterType = noFiltering ? FilterMode.Point : FilterMode.Bilinear;
            var rtFormat = parameters.UseHDR ? GetHDRFormat() : RenderTextureFormat.ARGB32;

            if (IsUsingVR(parameters))
            {
                var descriptor = UnityEngine.XR.XRSettings.eyeTextureDesc;
                descriptor.colorFormat = rtFormat;
                descriptor.width = width;
                descriptor.height = height;
                descriptor.depthBufferBits = depthBuffer;
                descriptor.msaaSamples = forceNoAA ? 1 : Mathf.Max(parameters.Antialiasing, 1);

                var eyesCount = parameters.EyeMask == StereoTargetEyeMask.Both ? VRTextureUsage.TwoEyes : VRTextureUsage.OneEye;
                descriptor.vrUsage = eyesCount;

                return new RenderTextureInfo(descriptor, filterType);
            }
            else
            {
                var descriptor = new RenderTextureDescriptor(width, height, rtFormat, depthBuffer);
                descriptor.dimension = TextureDimension.Tex2D;
                descriptor.msaaSamples = forceNoAA ? 1 : Mathf.Max(parameters.Antialiasing, 1);

                return new RenderTextureInfo(descriptor, filterType);
            }
        }

        public static void GetTemporaryRT(OutlineParameters parameters, int id, int width, int height, int depthBuffer, bool clear, bool forceNoAA, bool noFiltering)
        {
            var info = GetTargetInfo(parameters, width, height, depthBuffer, forceNoAA, noFiltering);

            parameters.Buffer.GetTemporaryRT(id, info.Descriptor, info.FilterMode);
            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, id));
            if (clear)
                parameters.Buffer.ClearRenderTarget(true, true, Color.clear);
        }

        private static RenderTextureFormat GetHDRFormat()
        {
            if (hdrFormat.HasValue)
                return hdrFormat.Value;

            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                hdrFormat = RenderTextureFormat.ARGBHalf;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                hdrFormat = RenderTextureFormat.ARGBFloat;
            else if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB64))
                hdrFormat = RenderTextureFormat.ARGB64;
            else
                hdrFormat = RenderTextureFormat.ARGB32;

            return hdrFormat.Value;
        }
    }
}