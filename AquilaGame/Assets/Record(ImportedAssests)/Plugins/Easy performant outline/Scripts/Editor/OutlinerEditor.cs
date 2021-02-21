using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    [CustomEditor(typeof(Outliner))]
    public class OutlinerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var maskProperty = serializedObject.FindProperty("outlineLayerMask");
            var currentMask = maskProperty.longValue;

            var maskValue = "Mask: none";
            if (currentMask == -1 || currentMask == long.MaxValue)
                maskValue = "Mask: all";
            else if (currentMask != 0)
                maskValue = "Mask: mixed";

            if (GUILayout.Button(maskValue, EditorStyles.layerMaskField))
            {
                var maskMenu = new GenericMenu();

                maskMenu.AddItem(new GUIContent("none"), currentMask == 0, () =>
                    {
                        maskProperty.longValue = 0;
                        serializedObject.ApplyModifiedProperties();
                    });

                maskMenu.AddItem(new GUIContent("all"), currentMask == -1 || currentMask == long.MaxValue, () =>
                    {
                        maskProperty.longValue = -1;
                        serializedObject.ApplyModifiedProperties();
                    });

                for (var index = 0; index < sizeof(long) * 8; index++)
                {
                    var capturedIndex = index;

                    if (index >= 20)
                    {
                        var decima = index / 10;
                        var lowerDecima = decima * 10;
                        var higherDecima = (decima + 1) * 10;
                        if (higherDecima > 63)
                            higherDecima = 63;

                        maskMenu.AddItem(new GUIContent(lowerDecima + "-" + higherDecima + "/" + index), (currentMask & 1 << index) != 0, () =>
                            {
                                maskProperty.longValue = currentMask ^ (1 << capturedIndex);
                                serializedObject.ApplyModifiedProperties();
                            });
                    }
                    else
                    {
                         maskMenu.AddItem(new GUIContent(index.ToString()), (currentMask & 1 << index) != 0, () =>
                            {
                                maskProperty.longValue = currentMask ^ (1 << capturedIndex);
                                serializedObject.ApplyModifiedProperties();
                            });
                    }
                }

                maskMenu.ShowAsContext();
            }

            DrawPropertiesExcluding(serializedObject,
                "m_Script",
                "outlineLayerMask",
                "infoRendererScale");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("infoRendererScale"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}