using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.WSA;

namespace EPOOutline
{
#if UNITY_2019_1_OR_NEWER
    public class EPOSetuper : EditorWindow
    {
        private static readonly string SRPOutlineName = "URP_OUTLINE";
        private static readonly string EPODOTweenName = "EPO_DOTWEEN";
        private static readonly string SRPShownID = "EasyPerformantOutlineWasShownAndCanceled";
        private static bool pipelineWasFound = false;

        private static ListRequest request;
        private static AddRequest addRequest;

        private Texture2D logoImage;

        public static bool ShouldShow
        {
            get
            {
                return PlayerPrefs.GetInt(SRPShownID, 0) == 0;
            }

            set
            {
                PlayerPrefs.SetInt(SRPShownID, value ? 0 : 1);
            }
        }

        private static bool CheckHasDefinition(string definition)
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var splited = definitions.Split(';');

            return Array.Find(splited, x => x == definition) != null;
        }

        private static bool CheckHasSRPOutlineDefinition()
        {
            return CheckHasDefinition(SRPOutlineName);
        }

        private static bool CheckHasEPODotween()
        {
            return CheckHasDefinition(EPODOTweenName);
        }

#if URP_OUTLINE
        private static bool CheckShouldFixFeature()
        {
            var activeAssets = PipelineAssetUtility.ActiveAssets;

            foreach (var asset in activeAssets)
            {
                if (!PipelineAssetUtility.IsURPOrLWRP(asset))
                    continue;

                if (!PipelineAssetUtility.IsAssetContainsSRPOutlineFeature(asset))
                    return true;
            }

            return false;
        }

        private static bool CheckHasActiveRenderers()
        {
            return PipelineAssetUtility.ActiveAssets.Count > 0;
        }

        private static void SelectAssetToAddFeature(RenderPipelineAsset asset)
        {
            Selection.activeObject = PipelineAssetUtility.GetRenderer(asset);
        }
#endif

        private static void RemoveDefinition(string definition, Func<bool> check)
        {
            if (!check())
                return;

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var splited = definitions.Split(';');

            var builder = new StringBuilder();

            var addedCount = 0;
            foreach (var item in splited)
            {
                if (item == definition)
                    continue;

                builder.Append(item);
                builder.Append(';');
                addedCount++;
            }

            if (addedCount != 0)
                builder.Remove(builder.Length - 1, 1);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, builder.ToString());
        }

        private static void RemoveSRPOutlineFromDefinitions()
        {
            RemoveDefinition(SRPOutlineName, CheckHasSRPOutlineDefinition);
        }

        private static void AddSRPDefinition()
        {
            AddDefinition(SRPOutlineName, CheckHasSRPOutlineDefinition);
        }

        private static void RemoveDOTweenDefinition()
        {
            RemoveDefinition(EPODOTweenName, CheckHasEPODotween);
        }

        private static void AddDOTweenDefinition()
        {
            AddDefinition(EPODOTweenName, CheckHasEPODotween);
        }

        private static void AddDefinition(string definition, Func<bool> check)
        {
            if (check())
                return;

            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definitions + ";" + definition);
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            request = Client.List();
            EditorApplication.update += Check;
        }

        private static bool CheckSetup()
        {
            return CheckHasSRPOutlineDefinition()
#if URP_OUTLINE
                && !CheckShouldFixFeature() && CheckHasActiveRenderers();
#else
                ;
#endif
        }

        private static void Check()
        {
            if (EditorApplication.isPlaying)
                return;

            if (!request.IsCompleted)
                return;

            pipelineWasFound = HasURPOrLWRP(request.Result);
            if (!pipelineWasFound)
            {
                RemoveSRPOutlineFromDefinitions();
                return;
            }

            request = Client.List();

            if (CheckSetup())
                return;

            ShowWindow();
        }

        private static bool HasURPOrLWRP(PackageCollection result)
        {
            if (result == null)
                return false;

            var found = false;
            var name =
#if UNITY_2019_3_OR_NEWER
                    "com.unity.render-pipelines.universal";
#else
                    "com.unity.render-pipelines.lightweight";
#endif
            foreach (var item in result)
            {
                if (item.name == name)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        [MenuItem("Tools/Easy performant outline/Setup")]
        private static void ForceShowWindow()
        {
            ShouldShow = true;
            ShowWindow();
        }

        private static void ShowWindow()
        {
            if (!ShouldShow)
                return;

            var window = EditorWindow.GetWindow<EPOSetuper>(true, "EPO Setuper", false);
            window.maxSize = new Vector2(500, 500);
            window.minSize = new Vector2(500, 500);
        }

        public void OnGUI()
        {
            if (logoImage == null)
                logoImage = Resources.Load<Texture2D>("Easy performant outline/EP Outline logo");

            var height = 180;
            GUILayout.Space(height);

            var imagePosition = new Rect(Vector2.zero, new Vector2(position.width, height));

            GUI.DrawTexture(imagePosition, logoImage, ScaleMode.ScaleAndCrop, true);

            GUILayout.Space(10);

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Please stop running the app to start setup process", MessageType.Info);

                if (GUILayout.Button("Stop"))
                    EditorApplication.isPlaying = false;

                return;
            }

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox(new GUIContent("Compiling... please wait."));
                return;
            }

            EditorGUILayout.HelpBox("Warning!\n Don't add integrations that is not available in your project. This will lead to compilation errors", MessageType.Warning);

            EditorGUILayout.LabelField("Integrations");

            EditorGUI.indentLevel = 1;

            var shouldAddDotween = EditorGUILayout.Toggle(new GUIContent("DOTween support"), CheckHasEPODotween());
            if (shouldAddDotween)
                AddDOTweenDefinition();
            else
                RemoveDOTweenDefinition();

            EditorGUILayout.Space();

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("URP Setup");

            EditorGUI.indentLevel = 1;

            if (addRequest != null && !addRequest.IsCompleted)
            {
                EditorGUILayout.HelpBox(new GUIContent("Adding package..."));
                return;
            }

            var packageName =
#if UNITY_2019_3_OR_NEWER
                    "com.unity.render-pipelines.universal";
#else
                    "com.unity.render-pipelines.lightweight";
#endif

            if (!pipelineWasFound)
            {
                EditorGUILayout.HelpBox(new GUIContent("There are no package added. Chick 'Add' to add the pipeline package."));

                if (GUILayout.Button("Add"))
                    addRequest = Client.Add(packageName);

                return;
            }
            else
                EditorGUILayout.HelpBox(new GUIContent("Pipeline asset has been found in packages"));

            if (!CheckHasSRPOutlineDefinition())
            {
                EditorGUILayout.HelpBox(new GUIContent("There is no URP_OUTLINE feature added. Click 'Add' to fix it."));
                if (GUILayout.Button("Add"))
                    AddSRPDefinition();
            }
            else
                EditorGUILayout.HelpBox(new GUIContent("URP_OUTLINE definition is added"));

#if URP_OUTLINE
            if (!CheckHasActiveRenderers())
            {
                EditorGUILayout.HelpBox(new GUIContent("There are not renderer asset seted up. Create one?"));

                if (GUILayout.Button("Create"))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Asset location", "Rendering asset", "asset", "Select the folder to save rendering asset");
                    if (string.IsNullOrEmpty(path))
                        return;

                    var pathNoExt = Path.ChangeExtension(path, string.Empty);
                    pathNoExt = pathNoExt.Substring(0, pathNoExt.Length - 1);

                    var rendererAsset = PipelineAssetUtility.CreateRenderData();
                    var asset = PipelineAssetUtility.CreateAsset(rendererAsset);
                    GraphicsSettings.renderPipelineAsset = asset;
                    AssetDatabase.CreateAsset(rendererAsset, pathNoExt + " renderer.asset");
                    AssetDatabase.CreateAsset(asset, path);
                }
            }
            else
                EditorGUILayout.HelpBox(new GUIContent("At least one renderer asset is set up"));

            if (CheckShouldFixFeature())
            {
                var assets = PipelineAssetUtility.ActiveAssets;
                foreach (var asset in assets)
                {
                    if (PipelineAssetUtility.IsAssetContainsSRPOutlineFeature(asset))
                        continue;

                    GUILayout.BeginHorizontal();

                    var text = string.Format("There is no outline feature added to the pipeline asset called '{0}'. Click select to find asset that is missing outline features", asset.name);
                    EditorGUILayout.HelpBox(new GUIContent(text));
                    if (GUILayout.Button("Select"))
                        SelectAssetToAddFeature(asset);

                    GUILayout.EndHorizontal();
                }

                return;
            }
            else
                EditorGUILayout.HelpBox(new GUIContent("Feature is added for all renderers in use"));
#endif
        }

        public void OnDestroy()
        {
            ShouldShow = false;
        }
    }
#endif
}