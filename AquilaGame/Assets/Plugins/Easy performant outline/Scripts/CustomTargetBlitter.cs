using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EPOOutline
{
    [ExecuteAlways]
    public class CustomTargetBlitter : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private string customTargetName;
#pragma warning restore 0649

        private CommandBuffer buffer;

        private Material blitMaterial;

        private Coroutine blitProcess = null;

        private void Awake()
        {
            buffer = new CommandBuffer();
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            blitProcess = StartCoroutine(Blit());
        }

        private void Update()
        {
            if (blitProcess != null)
                return;

            StopAllCoroutines();
            blitProcess = StartCoroutine(Blit());
        }

        private void OnDestroy()
        {
            if (buffer != null)
                buffer.Dispose();
        }

        private void CheckMaterial()
        {
            if (blitMaterial == null)
                blitMaterial = new Material(OutlineEffect.LoadMaterial("TransparentBlit"));
        }

        private IEnumerator Blit()
        {
            var waitForEndOfFrame = new WaitForEndOfFrame();
            
            if (buffer == null)
            {
                buffer = new CommandBuffer();
                buffer.name = "Custom target blitter";
            }

            while (enabled)
            {
                CheckMaterial();

                yield return waitForEndOfFrame;

                var target = TargetsHolder.Instance.GetAllocatedTarget(customTargetName);
                if (target == null)
                    continue;

                buffer.Clear();
                buffer.Blit(target, BuiltinRenderTextureType.None, blitMaterial);

                Graphics.ExecuteCommandBuffer(buffer);
            }
        }
    }
}