using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Outlinable))]
    public class OutlinableEditor : Editor
    {
        private UnityEditorInternal.ReorderableList targetsList;

        private void CheckList(UnityEditor.SerializedProperty targets)
        {
            if (targetsList == null)
            {
                targetsList = new UnityEditorInternal.ReorderableList(serializedObject, targets);

                targetsList.drawHeaderCallback = position => EditorGUI.LabelField(position, "Renderers. All renderers that will be included to outline rendering should be in the list.");

                targetsList.drawElementCallback = (position, item, isActive, isFocused) =>
                    {
                        var renderPosition = position;
                        var element = targets.GetArrayElementAtIndex(item);
                        var rendererItem = element.FindPropertyRelative("Renderer");
                        var reference = rendererItem.objectReferenceValue;

                        EditorGUI.PropertyField(renderPosition, element, new GUIContent(reference == null ? "Null" : reference.name), true);
                    };

                targetsList.elementHeightCallback = (index) => EditorGUI.GetPropertyHeight(targets.GetArrayElementAtIndex(index));

                targetsList.onRemoveCallback = (list) =>
                    {
                        var index = list.index;
                        targets.DeleteArrayElementAtIndex(index);
                        targets.serializedObject.ApplyModifiedProperties();
                    };

                targetsList.onAddDropdownCallback = (buttonRect, targetList) =>
                    {
                        var outlinable = target as Outlinable;
                        var items = outlinable.gameObject.GetComponentsInChildren<Renderer>(true);
                        var menu = new GenericMenu();

                        if (!Application.isPlaying)
                        {
                            menu.AddItem(new GUIContent("Add all"), false, () =>
                                {
                                    (target as Outlinable).AddAllChildRenderersToRenderingList(RenderersAddingMode.All);

                                    EditorUtility.SetDirty(target);
                                });

                            menu.AddItem(new GUIContent("Add all basic"), false, () =>
                                {
                                    (target as Outlinable).AddAllChildRenderersToRenderingList(RenderersAddingMode.MeshRenderer | RenderersAddingMode.SkinnedMeshRenderer);

                                    EditorUtility.SetDirty(target);
                                });
                        }

                        menu.AddItem(new GUIContent("Empty"), false, () =>
                            {
                                (target as Outlinable).OutlineTargets.Add(new OutlineTarget());

                                EditorUtility.SetDirty(target);
                            });

                        foreach (var item in items)
                        {
                            var found = false;
                            for (var index = 0; index < targets.arraySize; index++)
                            {
                                var element = targets.GetArrayElementAtIndex(index);
                                var elementRenderer = element.FindPropertyRelative("Renderer");
                                if (elementRenderer.objectReferenceValue == item)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            var path = string.Empty;
                            if (item.transform != outlinable.transform)
                            {
                                var parent = item.transform;
                                do
                                {
                                    path = string.Format("{0}/{1}", parent.ToString(), path);
                                    parent = parent.transform.parent;
                                }
                                while (parent != outlinable.transform);

                                path = string.Format("{0}/{1}", parent.ToString(), path);

                                path = path.Substring(0, path.Length - 1);
                            }
                            else
                                path = item.ToString();

                            GenericMenu.MenuFunction function = () =>
                                {
                                    var index = targets.arraySize;
                                    targets.InsertArrayElementAtIndex(index);
                                    var arrayItem = targets.GetArrayElementAtIndex(index);
                                    var renderer = arrayItem.FindPropertyRelative("Renderer");
                                    arrayItem.FindPropertyRelative("CutoutThreshold").floatValue = 0.5f;
                                    renderer.objectReferenceValue = item;

                                    serializedObject.ApplyModifiedProperties();
                                };

                            if (found)
                                function = null;

                            menu.AddItem(new GUIContent(path), found, function);
                        }

                        menu.ShowAsContext();
                    };
            }
        }

        public override void OnInspectorGUI()
        {
            if ((serializedObject.FindProperty("drawingMode").intValue & (int)OutlinableDrawingMode.Normal) != 0)
            {
                if (serializedObject.FindProperty("renderStyle").intValue == 1)
                {
                    DrawPropertiesExcluding(serializedObject,
                        "frontParameters",
                        "backParameters",
                        "outlineTargets",
                        "outlineTargets",
                        "m_Script");
                }
                else
                {
                    DrawPropertiesExcluding(serializedObject,
                        "outlineParameters",
                        "outlineTargets",
                        "outlineTargets",
                        "m_Script");
                }
            }
            else
            {
                DrawPropertiesExcluding(serializedObject,
                    "outlineParameters",
                    "frontParameters",
                    "backParameters",
                    "outlineTargets",
                    "m_Script");
            }

            serializedObject.ApplyModifiedProperties();

            var renderers = serializedObject.FindProperty("outlineTargets");

            CheckList(renderers);

            if (serializedObject.targetObjects.Count() == 1)
                targetsList.DoLayoutList();
        }
    }
}
