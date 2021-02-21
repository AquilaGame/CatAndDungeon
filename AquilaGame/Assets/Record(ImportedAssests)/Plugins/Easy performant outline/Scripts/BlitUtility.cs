using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.Profiling;

namespace EPOOutline
{
    public static class BlitUtility
    {
        private static readonly int MainTexHash = Shader.PropertyToID("_MainTex");

        private static Vector3[] vertices = new Vector3[8];
        private static Vector4[] matrix = new Vector4[8];

        private static Vector3[] tempVector3 = new Vector3[8];
        private static Vector2[] tempVector2 = new Vector2[8];

        private static int[] trianglesToAdd = new int[36];

        private static Vector3[] normals = new Vector3[]
            {
                new Vector3(-0.5773f, -0.5773f, -0.5773f),
                new Vector3(0.5773f, -0.5773f, -0.5773f),
                new Vector3(0.5773f, 0.5773f, -0.5773f),
                new Vector3(-0.5773f, 0.5773f, -0.5773f),
                new Vector3(-0.5773f, 0.5773f, 0.5773f),
                new Vector3(0.5773f, 0.5773f, 0.5773f),
                new Vector3(0.5773f, -0.5773f, 0.5773f),
                new Vector3(-0.5773f, -0.5773f, 0.5773f),
            };

        private static int[] triangles = 
            {
                0, 2, 1,
	            0, 3, 2,
                2, 3, 4,
	            2, 4, 5,
                1, 2, 5,
	            1, 5, 6,
                0, 7, 4,
	            0, 4, 3,
                5, 4, 7,
	            5, 7, 6,
                0, 6, 7,
	            0, 1, 6
            };

        private static List<Vector4> firstUV            = new List<Vector4>();
        private static List<Vector4> secondUV           = new List<Vector4>();
        private static List<Vector4> thirdUV            = new List<Vector4>();
        private static List<Vector4> fourthUV           = new List<Vector4>();
        private static List<Vector3> centers            = new List<Vector3>();
        private static List<Vector3> size               = new List<Vector3>();
        private static List<Vector3> stages             = new List<Vector3>();
        private static List<Vector2> additionalScale    = new List<Vector2>();

        private static List<Vector3> addedVertices = new List<Vector3>();
        public static List<int> addedTriangles = new List<int>();
        public static List<Vector3> addedNormals = new List<Vector3>();

        private static void UpdateBounds(Renderer renderer)
        {
            if (renderer is MeshRenderer)
            {
                var meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter.sharedMesh != null)
                    meshFilter.sharedMesh.RecalculateBounds();
            }
            else if (renderer is SkinnedMeshRenderer)
            {
                var skinedMeshRenderer = renderer as SkinnedMeshRenderer;
                if (skinedMeshRenderer.sharedMesh != null)
                    skinedMeshRenderer.sharedMesh.RecalculateBounds();
            }
        }

        public static void SetupMesh(OutlineParameters parameters, float baseShift)
        {
            if (parameters.BlitMesh == null)
                parameters.BlitMesh = parameters.MeshPool.AllocateMesh();

            parameters.BlitMesh.Clear(true);

            addedNormals.Clear();
            addedVertices.Clear();
            addedTriangles.Clear();

            var otherStages = new Vector4(1, 0, 0);
            var dilateStage = new Vector4(0, 1, 0);
            var blurStage   = new Vector4(0, 0, 1);
            var allStages = dilateStage + blurStage + otherStages;

            firstUV.Clear();
            secondUV.Clear();
            thirdUV.Clear();
            fourthUV.Clear();
            centers.Clear();
            size.Clear();
            stages.Clear();
            additionalScale.Clear();

            var minusBoundsX = -1;
            var plusBoundsX = 1;
            var minusBoundsY = -1;
            var plusBoundsY = 1;
            var minusBoundsZ = -1;
            var plusBoundsZ = 1;

            vertices[0] = new Vector3(minusBoundsX, minusBoundsY, minusBoundsZ);
            vertices[1] = new Vector3(plusBoundsX, minusBoundsY, minusBoundsZ);
            vertices[2] = new Vector3(plusBoundsX, plusBoundsY, minusBoundsZ);
            vertices[3] = new Vector3(minusBoundsX, plusBoundsY, minusBoundsZ);
            vertices[4] = new Vector3(minusBoundsX, plusBoundsY, plusBoundsZ);
            vertices[5] = new Vector3(plusBoundsX, plusBoundsY, plusBoundsZ);
            vertices[6] = new Vector3(plusBoundsX, minusBoundsY, plusBoundsZ);
            vertices[7] = new Vector3(minusBoundsX, minusBoundsY, plusBoundsZ);

            var currentIndex = 0;
            const int numberOfVertices = 8;
            var addedCount = 0;
            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                if (outlinable.DrawingMode != OutlinableDrawingMode.Normal)
                    continue;

                var frontParameters = outlinable.RenderStyle == RenderStyle.FrontBack ? outlinable.FrontParameters : outlinable.OutlineParameters;
                var backParameters = outlinable.RenderStyle == RenderStyle.FrontBack ? outlinable.BackParameters : outlinable.OutlineParameters;

                var useDilateDueToSettings = parameters.UseInfoBuffer && (frontParameters.DilateShift > 0.01f || backParameters.DilateShift > 0.01f) || !parameters.UseInfoBuffer;
                var useBlurDueToSettings = parameters.UseInfoBuffer && (frontParameters.BlurShift > 0.01f || backParameters.BlurShift > 0.01f) || !parameters.UseInfoBuffer;

                foreach (var target in outlinable.OutlineTargets)
                {
                    var renderer = target.Renderer;
                    if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                        continue;

                    Profiler.BeginSample("Getting mesh bounds");

                    if (target.ForceRecalculateBounds)
                        UpdateBounds(target.Renderer);

#if UNITY_2019_3_OR_NEWER
                    var meshRenderer = renderer as MeshRenderer;
                    var index = meshRenderer == null ? 0 : meshRenderer.subMeshStartIndex + target.SubmeshIndex;
                    var filter = meshRenderer == null ? null : meshRenderer.GetComponent<MeshFilter>();
                    var mesh = filter == null ? null : filter.sharedMesh;

                    var bounds = new Bounds();
                    if (meshRenderer != null && mesh != null && mesh.subMeshCount > index)
                        bounds = mesh.GetSubMesh(index).bounds;
                    else if (renderer != null)
                        bounds = renderer.bounds;
#else
                    var bounds = renderer.bounds;
#endif
                    
                    Profiler.EndSample();

                    var scale = 0.5f;
                    var boundsSize = bounds.size * scale;
                    var boundsCenter = bounds.center;

                    var stagesToSet = otherStages;
                    if ((!target.CanUseEdgeDilateShift || target.DilateRenderingMode == DilateRenderMode.PostProcessing) && useDilateDueToSettings)
                        stagesToSet += dilateStage;

                    if (useBlurDueToSettings)
                        stagesToSet += blurStage;

                    var additionalScaleToSet = Vector2.zero;
                    if (target.CanUseEdgeDilateShift && target.DilateRenderingMode == DilateRenderMode.EdgeShift)
                        additionalScaleToSet.x = Mathf.Max(target.BackEdgeDilateAmount, target.FrontEdgeDilateAmount);

                    Profiler.BeginSample("Setting vertex values");

#if UNITY_2019_3_OR_NEWER
                    if (meshRenderer != null && !meshRenderer.isPartOfStaticBatch)
                    {
                        var transformMatrix = meshRenderer.transform.localToWorldMatrix;

                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = transformMatrix.GetColumn(0);
                        firstUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = transformMatrix.GetColumn(1);
                        secondUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = transformMatrix.GetColumn(2);
                        thirdUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = transformMatrix.GetColumn(3);
                        fourthUV.AddRange(matrix);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = boundsCenter;
                        centers.AddRange(tempVector3);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = boundsSize;
                        size.AddRange(tempVector3);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = stagesToSet;
                        stages.AddRange(tempVector3);

                        tempVector2[0] = tempVector2[1] = tempVector2[2] = tempVector2[3] = tempVector2[4] = tempVector2[5] = tempVector2[6] = tempVector2[7] = additionalScaleToSet;
                        additionalScale.AddRange(tempVector2);
                    }
                    else
#endif
                    {
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = new Vector4(1, 0, 0, 0);
                        firstUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = new Vector4(0, 1, 0, 0);
                        secondUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = new Vector4(0, 0, 1, 0);
                        thirdUV.AddRange(matrix);
                        matrix[0] = matrix[1] = matrix[2] = matrix[3] = matrix[4] = matrix[5] = matrix[6] = matrix[7] = new Vector4(0, 0, 0, 1);
                        fourthUV.AddRange(matrix);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = boundsCenter;
                        centers.AddRange(tempVector3);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = boundsSize;
                        size.AddRange(tempVector3);

                        tempVector3[0] = tempVector3[1] = tempVector3[2] = tempVector3[3] = tempVector3[4] = tempVector3[5] = tempVector3[6] = tempVector3[7] = stagesToSet;
                        stages.AddRange(tempVector3);

                        tempVector2[0] = tempVector2[1] = tempVector2[2] = tempVector2[3] = tempVector2[4] = tempVector2[5] = tempVector2[6] = tempVector2[7] = additionalScaleToSet;
                        additionalScale.AddRange(tempVector2);
                    }


                    Profiler.EndSample();

                    addedVertices.AddRange(vertices);
                    addedNormals.AddRange(normals);

                    trianglesToAdd[0] =
                        trianglesToAdd[3] =
                        trianglesToAdd[18] =
                        trianglesToAdd[21] =
                        trianglesToAdd[30] =
                        trianglesToAdd[33] = currentIndex;

                    trianglesToAdd[2] =
                        trianglesToAdd[12] =
                        trianglesToAdd[15] =
                        trianglesToAdd[34] = 1 + currentIndex;

                    trianglesToAdd[1] =
                        trianglesToAdd[5] =
                        trianglesToAdd[6] =
                        trianglesToAdd[9] =
                        trianglesToAdd[13] = 2 + currentIndex;

                    trianglesToAdd[4] =
                        trianglesToAdd[7] =
                        trianglesToAdd[23] = 3 + currentIndex;

                    trianglesToAdd[8] =
                        trianglesToAdd[10] =
                        trianglesToAdd[20] =
                        trianglesToAdd[22] =
                        trianglesToAdd[25] = 4 + currentIndex;

                    trianglesToAdd[11] =
                        trianglesToAdd[14] =
                        trianglesToAdd[16] =
                        trianglesToAdd[24] =
                        trianglesToAdd[27] = 5 + currentIndex;

                    trianglesToAdd[17] =
                        trianglesToAdd[29] =
                        trianglesToAdd[31] =
                        trianglesToAdd[35] = 6 + currentIndex;

                    trianglesToAdd[19] =
                        trianglesToAdd[26] =
                        trianglesToAdd[28] =
                        trianglesToAdd[32] = 7 + currentIndex;

                    currentIndex += numberOfVertices;

                    addedTriangles.AddRange(trianglesToAdd);
                    addedCount++;
                }
            }

            if (addedCount == 0)
                return;

            Profiler.BeginSample("Setting mesh values");

            parameters.BlitMesh.SetVertices(addedVertices);

            parameters.BlitMesh.SetUVs(0, firstUV);
            parameters.BlitMesh.SetUVs(1, secondUV);
            parameters.BlitMesh.SetUVs(2, thirdUV);
            parameters.BlitMesh.SetUVs(3, fourthUV);
            parameters.BlitMesh.SetUVs(4, centers);
            parameters.BlitMesh.SetUVs(5, size);
            parameters.BlitMesh.SetUVs(6, stages);
            parameters.BlitMesh.SetUVs(7, additionalScale);

            parameters.BlitMesh.SetTriangles(addedTriangles, 0, false);
            parameters.BlitMesh.SetNormals(addedNormals);

            Profiler.EndSample();
        }

        public static void Blit(OutlineParameters parameters, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier destinationDepth, Material material, CommandBuffer targetBuffer, int pass = -1)
        {
            var buffer = targetBuffer == null ? parameters.Buffer : targetBuffer;
            buffer.SetRenderTarget(destination, destinationDepth);

            buffer.SetGlobalTexture(MainTexHash, source);

            buffer.DrawMesh(parameters.BlitMesh, Matrix4x4.identity, material, 0, pass);
        }

        public static void Draw(OutlineParameters parameters, RenderTargetIdentifier target, RenderTargetIdentifier depth, Material material)
        {
            parameters.Buffer.SetRenderTarget(target, depth);

            parameters.Buffer.DrawMesh(parameters.BlitMesh, Matrix4x4.identity, material, 0, -1);
        }
    }
}