using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPOOutline
{
    [ExecuteAlways]
    public class TargetsHolder : MonoBehaviour
    {
        private static TargetsHolder instance;

        private Dictionary<string, RenderTexture> targets = new Dictionary<string, RenderTexture>();

        public static TargetsHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("TargetsHolder");
                    instance = go.AddComponent<TargetsHolder>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                }

                return instance;
            }
        }

        private void OnDestroy()
        {
            ReleaseTargets();
        }

        public RenderTexture GetAllocatedTarget(string name)
        {
            RenderTexture result = null;
            if (!targets.TryGetValue(name, out result))
                return null;

            return result;
        }

        public RenderTexture GetTarget(OutlineParameters parameters, string name)
        {
            RenderTexture result = null;
            if (!targets.TryGetValue(name, out result))
            {
                var info = RenderTargetUtility.GetTargetInfo(parameters, parameters.TargetWidth, parameters.TargetHeight, 24, false, false);
                result = RenderTexture.GetTemporary(info.Descriptor);
                result.filterMode = info.FilterMode;

                targets.Add(name, result);

                return result;
            }

            Shader.SetGlobalTexture(name, result);
            return result;
        }

        private void ReleaseTargets()
        {
            RenderTexture.active = null;

            foreach (var target in targets)
            {
                if (target.Value == null)
                    continue;

                Graphics.SetRenderTarget(target.Value);
                GL.Clear(true, true, Color.clear);

                RenderTexture.ReleaseTemporary(target.Value);
            }

            targets.Clear();
        }

        private void Update()
        {
            ReleaseTargets();
        }
    }
}