using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EPOOutline
{
    public enum CutoutDescriptionType
    {
        None,
        Hash
    }

    [System.Serializable]
    public class OutlineTarget
    {
        [SerializeField]
        private float edgeDilateAmount = 5.0f;

        [SerializeField]
        private float frontEdgeDilateAmount = 5.0f;

        [SerializeField]
        private float backEdgeDilateAmount = 5.0f;

        [SerializeField]
        public Renderer Renderer;

        [SerializeField]
        public int SubmeshIndex;

        [SerializeField]
        public bool ForceRecalculateBounds = false;

        [SerializeField]
        public CutoutDescriptionType CutoutDescriptionType;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        public float CutoutThreshold = 0.5f;

        [SerializeField]
        public CullMode CullMode;

        [SerializeField]
        private string cutoutTextureName;

        [SerializeField]
        public DilateRenderMode DilateRenderingMode;

        [SerializeField]
        private int cutoutTextureIndex;
        
        private int? cutoutTextureId;

        public bool UsesCutout
        {
            get
            {
                return !string.IsNullOrEmpty(cutoutTextureName);
            }
        }

        public int CutoutTextureIndex
        {
            get
            {
                return cutoutTextureIndex;
            }

            set
            {
                cutoutTextureIndex = value;
                if (cutoutTextureIndex < 0)
                {
                    Debug.LogError("Trying to set cutout texture index less than zero");
                    cutoutTextureIndex = 0;
                }
            }
        }

        public bool CanUseEdgeDilateShift
        {
            get
            {
                return !UsesCutout && (Renderer is MeshRenderer || Renderer is SkinnedMeshRenderer) && (Renderer != null && !Renderer.isPartOfStaticBatch && !Renderer.gameObject.isStatic);
            }
        }

        public int ShiftedSubmeshIndex
        {
            get
            {
                return SubmeshIndex;
            }
        }

        public int CutoutTextureId
        {
            get
            {
                if (!cutoutTextureId.HasValue)
                    cutoutTextureId = Shader.PropertyToID(cutoutTextureName);

                return cutoutTextureId.Value;
            }
        }

        public string CutoutTextureName
        {
            get
            {
                return cutoutTextureName;
            }

            set
            {
                cutoutTextureName = value;
                cutoutTextureId = null;
            }
        }

        public float EdgeDilateAmount
        {
            get
            {
                return edgeDilateAmount;
            }

            set
            {
                if (value < 0)
                    edgeDilateAmount = 0;
                else
                    edgeDilateAmount = value;
            }
        }

        public float FrontEdgeDilateAmount
        {
            get
            {
                return frontEdgeDilateAmount;
            }

            set
            {
                if (value < 0)
                    frontEdgeDilateAmount = 0;
                else
                    frontEdgeDilateAmount = value;
            }
        }

        public float BackEdgeDilateAmount
        {
            get
            {
                return backEdgeDilateAmount;
            }

            set
            {
                if (value < 0)
                    backEdgeDilateAmount = 0;
                else
                    backEdgeDilateAmount = value;
            }
        }

        public OutlineTarget()
        {

        }

        public OutlineTarget(Renderer renderer, int submesh = 0)
        {
            SubmeshIndex = submesh;
            Renderer = renderer;
            CutoutDescriptionType = CutoutDescriptionType.None;
            CutoutThreshold = 0.5f;
            cutoutTextureId = null;
            cutoutTextureName = string.Empty;
            CullMode = renderer is SpriteRenderer ? CullMode.Off : CullMode.Back;
            DilateRenderingMode = DilateRenderMode.PostProcessing;
            frontEdgeDilateAmount = 5.0f;
            backEdgeDilateAmount = 5.0f;
            edgeDilateAmount = 5.0f;
        }

        public OutlineTarget(Renderer renderer, string cutoutTextureName, float cutoutThreshold = 0.5f)
        {
            SubmeshIndex = 0;
            Renderer = renderer;
            CutoutDescriptionType = CutoutDescriptionType.Hash;
            cutoutTextureId = Shader.PropertyToID(cutoutTextureName);
            CutoutThreshold = cutoutThreshold;
            this.cutoutTextureName = cutoutTextureName;
            CullMode = renderer is SpriteRenderer ? CullMode.Off : CullMode.Back;
            DilateRenderingMode = DilateRenderMode.PostProcessing;
            frontEdgeDilateAmount = 5.0f;
            backEdgeDilateAmount = 5.0f;
            edgeDilateAmount = 5.0f;
        }
    }
}