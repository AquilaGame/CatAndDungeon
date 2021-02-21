using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EPOOutline
{
    public class ModelPostprocessor : AssetPostprocessor, IPreprocessBuildWithReport
    {
        private static readonly int UVIndex = 6;

        private static readonly float MinVertexDistance = 0.02f;

        public int callbackOrder
        {
            get
            {
                return int.MaxValue;
            }
        }

        private class VertexGroup
        {
            public Vector3 Position;
            public List<int> Others = new List<int>();
            public Vector3 Normal;
        }

        [MenuItem("Tools/Easy performant outline/Check models")]
        private static void CheckModelMenu()
        {
            EditorPrefs.DeleteKey("Never ask about models check");
            EditorPrefs.DeleteKey("Models checked");
            CheckModels();
        }

        [InitializeOnLoadMethod]
        private static void CheckModels()
        {
            if (EditorPrefs.HasKey("Models checked"))
                return;

            EditorPrefs.SetString("Models checked", "true");
            var models = AssetDatabase.FindAssets("t:Model");

            try
            {
                var index = 0;
                foreach (var modelGUID in models)
                {
                    var model = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(modelGUID));

                    var title = "Checking models for easy performant outlines";
                    var info = "Some model postprocessing will be applied. Checked {0}/{1}";

                    EditorUtility.DisplayProgressBar(title, string.Format(info, index, models.Length), (float)index / (float)models.Length);
                    index++;

                    var mesh = GetMesh(model);
                    if (mesh == null)
                        continue;

                    PostprocessModel(model, mesh);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            EditorUtility.ClearProgressBar();
        }

        private static Mesh GetMesh(GameObject model)
        {
            Mesh mesh = null;
            var renderer = model.GetComponent<Renderer>();
            if (renderer is MeshRenderer)
                mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            else if (renderer is SkinnedMeshRenderer)
                mesh = (renderer as SkinnedMeshRenderer).sharedMesh;

            return mesh;
        }

        public void OnPostprocessModel(GameObject model)
        {
            var mesh = GetMesh(model);
            if (mesh == null)
                return;

            try
            {
                PostprocessModel(model, mesh);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static void PostprocessModel(GameObject model, Mesh mesh)
        {
            var meshCopy = new Mesh();
            meshCopy.vertices = mesh.vertices;
            meshCopy.triangles = mesh.triangles;

            meshCopy.RecalculateNormals();

            var vertices = meshCopy.vertices;
            var normals = meshCopy.normals;

            var uvs = new List<Vector3>(new Vector3[vertices.Length]);

            for (var submesh = 0; submesh < mesh.subMeshCount; submesh++)
            {
                var verticesOfTheSubmesh = new HashSet<int>();
                var triangles = mesh.GetTriangles(submesh);

                foreach (var index in triangles)
                    verticesOfTheSubmesh.Add(index);

                var similarVertices = new List<VertexGroup>();
                foreach (var vertex in verticesOfTheSubmesh)
                {
                    var vertexPosition = vertices[vertex];
                    var similar = similarVertices.Find(x => Vector3.Distance(x.Position, vertexPosition) < MinVertexDistance);
                    if (similar == null)
                    {
                        similar = new VertexGroup() { Position = vertexPosition };
                        similarVertices.Add(similar);
                    }

                    similar.Normal += normals[vertex];
                    similar.Others.Add(vertex);
                }

                foreach (var group in similarVertices)
                {
                    var normal = (group.Normal / group.Others.Count).normalized;
                    foreach (var other in group.Others)
                        uvs[other] = normal;
                }
            }

            mesh.SetUVs(UVIndex, uvs);

            mesh.UploadMeshData(false);
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (EditorPrefs.GetBool("Never ask about models check"))
                return;

            var result = EditorUtility.DisplayDialogComplex("Would you like to check models?", "If you don't use edge shift outline you can skip this", "Check", "Skip", "Never ask again");
            switch (result)
            {
                case 0:
                    EditorPrefs.DeleteKey("Models checked");
                    CheckModels();
                    break;
                case 1:
                    return;
                case 2:
                    EditorPrefs.SetBool("Never ask about models check", true);
                    break;
            }

        }
    }
}