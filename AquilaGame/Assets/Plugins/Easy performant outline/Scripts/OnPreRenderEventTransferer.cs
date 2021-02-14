using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPOOutline
{
    [ExecuteAlways]
    public class OnPreRenderEventTransferer : MonoBehaviour
    {
        private Camera attachedCamera;

        public Action<Camera> OnPreRenderEvent;

        private void Awake()
        {
            attachedCamera = GetComponent<Camera>();
        }

        private void OnPreRender()
        {
            if (OnPreRenderEvent != null)
                OnPreRenderEvent(attachedCamera);
        }
    }
}