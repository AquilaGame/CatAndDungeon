using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if URP_OUTLINE && UNITY_2019_1_OR_NEWER
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#else
using UnityEngine.Rendering.LWRP;
#endif
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

#if HDRP_OUTLINE
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.HighDefinition;
#else
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
#endif

namespace EPOOutline
{
    public static class CameraUtility
    {
        public static int GetMSAA(Camera camera)
        {
            var antialiasing = GetRenderPipelineMSAA();

            var msaa = Mathf.Max(antialiasing, 1);
            if (!camera.allowMSAA)
                msaa = 1;

            if (camera.actualRenderingPath != RenderingPath.Forward &&
                camera.actualRenderingPath != RenderingPath.VertexLit)
                msaa = 1;

            return msaa;
        }
        
        private static int GetRenderPipelineMSAA()
        {
#if URP_OUTLINE && UNITY_2019_1_OR_NEWER
            if (PipelineFetcher.CurrentAsset is
#if UNITY_2019_3_OR_NEWER
                UniversalRenderPipelineAsset
#else
                LightweightRenderPipelineAsset
#endif
                )

                return (PipelineFetcher.CurrentAsset as
#if UNITY_2019_3_OR_NEWER
                    UniversalRenderPipelineAsset
#else
                    LightweightRenderPipelineAsset
#endif
                    ).msaaSampleCount;
#endif

#if HDRP_OUTLINE
            if (PipelineFetcher.CurrentAsset is HDRenderPipelineAsset)
                return 1;
#endif

            return QualitySettings.antiAliasing;
        }
    }
}