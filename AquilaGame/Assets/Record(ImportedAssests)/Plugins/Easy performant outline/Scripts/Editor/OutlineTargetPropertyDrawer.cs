using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EPOOutline
{
    [CustomPropertyDrawer(typeof(OutlineTarget))]
    public class OutlineTargetPropertyDrawer : PropertyDrawer
    {
        private static float lastWidth = 0.0f;

        private static GUIContent errorContent = new GUIContent("'Optimize mesh data' option is enabled in build settings.\n In order the feature to work it should be disabled.\n It might seems to work in editor but will not work in build if the setting is enabled.");

        private void Shift(ref Rect rect, bool right)
        {
            if (right)
                rect.x += EditorGUIUtility.singleLineHeight;

            rect.width -= EditorGUIUtility.singleLineHeight * (right ? 1.0f : 0.5f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            lastWidth = position.width;

            var initialPosition = position;

            var labelWidth = EditorGUIUtility.labelWidth;
            position.y += EditorGUIUtility.singleLineHeight * 0.2f;

            position.height = EditorGUIUtility.singleLineHeight;
            var rendererPosition = position;
            rendererPosition.width = position.width / 2;
            Shift(ref rendererPosition, false);
            var renderer = property.FindPropertyRelative("Renderer");
            EditorGUI.PropertyField(rendererPosition, renderer, GUIContent.none);

            var menu = new GenericMenu();

            var useCutoutProperty = property.FindPropertyRelative("CutoutDescriptionType");

            var cutoutIsInUse = useCutoutProperty.intValue != (int)CutoutDescriptionType.None;

            menu.AddItem(new GUIContent("none"), !cutoutIsInUse, () =>
                {
                    useCutoutProperty.intValue = (int)CutoutDescriptionType.None;
                    useCutoutProperty.serializedObject.ApplyModifiedProperties();
                });

            var textureNameProperty = property.FindPropertyRelative("cutoutTextureName");

            var rendererReference = renderer.objectReferenceValue as Renderer;
            var referenceName = "none";
            var usingCutout = cutoutIsInUse && rendererReference != null;
            if (rendererReference != null)
            {
                var material = rendererReference.sharedMaterial;
                if (material != null)
                {
                    var propertiesCount = ShaderUtil.GetPropertyCount(material.shader);
                    for (var index = 0; index < propertiesCount; index++)
                    {
                        var propertyType = ShaderUtil.GetPropertyType(material.shader, index);
                        if (propertyType != ShaderUtil.ShaderPropertyType.TexEnv)
                            continue;

                        var propertyName = ShaderUtil.GetPropertyName(material.shader, index);
                        var equals = propertyName == textureNameProperty.stringValue;
                        if (equals)
                            referenceName = ShaderUtil.GetPropertyDescription(material.shader, index);

                        menu.AddItem(new GUIContent(ShaderUtil.GetPropertyDescription(material.shader, index)), equals && usingCutout, () =>
                            {
                                textureNameProperty.stringValue = propertyName;
                                useCutoutProperty.intValue = (int)CutoutDescriptionType.Hash;
                                textureNameProperty.serializedObject.ApplyModifiedProperties();
                            });
                    }
                }
            }

            var cutoutPosition = position;
            cutoutPosition.x = rendererPosition.x + rendererPosition.width;
            cutoutPosition.width -= rendererPosition.width;
            Shift(ref cutoutPosition, true);

            var sourceLable = usingCutout ? referenceName : "none";

            if (EditorGUI.DropdownButton(cutoutPosition, new GUIContent("Cutout source: " + sourceLable), FocusType.Passive))
                menu.ShowAsContext();

            var drawingPosition = position;

            EditorGUIUtility.labelWidth = 160;
            drawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var forceRecalculateBoundsDrawingPosition = initialPosition;
            forceRecalculateBoundsDrawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 3.0f;
            forceRecalculateBoundsDrawingPosition.width = initialPosition.width;
            forceRecalculateBoundsDrawingPosition.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(forceRecalculateBoundsDrawingPosition, property.FindPropertyRelative("ForceRecalculateBounds"));

            forceRecalculateBoundsDrawingPosition.width /= 2;
            forceRecalculateBoundsDrawingPosition.x += forceRecalculateBoundsDrawingPosition.width;

            EditorGUIUtility.labelWidth = 80;
            Shift(ref forceRecalculateBoundsDrawingPosition, true);
            EditorGUI.PropertyField(forceRecalculateBoundsDrawingPosition, property.FindPropertyRelative("cutoutTextureIndex"), new GUIContent("Texture index"));

            EditorGUIUtility.labelWidth = labelWidth;

            if (usingCutout || rendererReference is SpriteRenderer)
            {
                drawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(drawingPosition, property.FindPropertyRelative("CutoutThreshold"));
            }
            else
            {
                var isDilateRenderingMode = property.FindPropertyRelative("DilateRenderingMode").intValue == (int)DilateRenderMode.EdgeShift;
                var appropriateToUseEdgeDilate = renderer.objectReferenceValue != null && !(renderer.objectReferenceValue as Renderer).gameObject.isStatic;
                if (appropriateToUseEdgeDilate)
                    drawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                var modeDrawingPosition = drawingPosition;
                modeDrawingPosition.width /= 2;

                if (appropriateToUseEdgeDilate)
                {
                    Shift(ref modeDrawingPosition, false);
                    EditorGUI.LabelField(modeDrawingPosition, property.FindPropertyRelative("DilateRenderingMode").displayName);

                    modeDrawingPosition.x += modeDrawingPosition.width;
                    Shift(ref modeDrawingPosition, true);

                    var initialColor = GUI.color;
                    if (isDilateRenderingMode && PlayerSettings.stripUnusedMeshComponents)
                        GUI.color = Color.red;

                    EditorGUI.PropertyField(modeDrawingPosition, property.FindPropertyRelative("DilateRenderingMode"), GUIContent.none);

                    GUI.color = initialColor;
                }

                if (isDilateRenderingMode && appropriateToUseEdgeDilate)
                {
                    drawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    if (PlayerSettings.stripUnusedMeshComponents)
                    {
                        var helpBoxPosition = drawingPosition;

                        var buttonStart = EditorStyles.helpBox.CalcHeight(errorContent, lastWidth - 60) + EditorGUIUtility.standardVerticalSpacing;
                        helpBoxPosition.height = buttonStart + EditorGUIUtility.singleLineHeight * 2.0f;

                        EditorGUI.HelpBox(helpBoxPosition, errorContent.text, MessageType.Error);

                        var buttonPosition = drawingPosition;
                        buttonPosition.y = helpBoxPosition.y + buttonStart + EditorGUIUtility.singleLineHeight * 0.5f;
                        buttonPosition.width -= EditorGUIUtility.singleLineHeight * 2.0f;
                        buttonPosition.x += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(buttonPosition, "Disable 'Optimize mesh data' option"))
                            PlayerSettings.stripUnusedMeshComponents = false;

                        drawingPosition.y += helpBoxPosition.height + EditorGUIUtility.singleLineHeight;
                    }

                    var shiftDrawingPosition = drawingPosition;
                    shiftDrawingPosition.width /= 2;

                    var parentRenderStyle = property.serializedObject.FindProperty("renderStyle");

                    if (parentRenderStyle.intValue == (int)RenderStyle.Single)
                    {
                        EditorGUI.LabelField(shiftDrawingPosition, "Edge shift");

                        shiftDrawingPosition.x += shiftDrawingPosition.width;
                        Shift(ref shiftDrawingPosition, true);

                        EditorGUI.PropertyField(shiftDrawingPosition, property.FindPropertyRelative("edgeDilateAmount"), GUIContent.none);
                    }
                    else
                    {
                        EditorGUIUtility.labelWidth = 80;

                        Shift(ref shiftDrawingPosition, false);
                        EditorGUI.PropertyField(shiftDrawingPosition, property.FindPropertyRelative("frontEdgeDilateAmount"), new GUIContent("Front dilate"));

                        shiftDrawingPosition.x += shiftDrawingPosition.width;
                        Shift(ref shiftDrawingPosition, true);

                        EditorGUI.PropertyField(shiftDrawingPosition, property.FindPropertyRelative("backEdgeDilateAmount"), new GUIContent("Back dilate"));

                        EditorGUIUtility.labelWidth = labelWidth;
                    }
                }
            }

            drawingPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var linePosition = drawingPosition;
            linePosition.width /= 2;
            Shift(ref linePosition, false);

            var cullPosition = linePosition;
            cullPosition.width /= 2;
            EditorGUI.LabelField(cullPosition, new GUIContent("Cull mode"));
            cullPosition.x += cullPosition.width;

            EditorGUI.PropertyField(cullPosition, property.FindPropertyRelative("CullMode"), GUIContent.none);

            linePosition.x += linePosition.width;
            Shift(ref linePosition, true);

            var submeshIndex = property.FindPropertyRelative("SubmeshIndex");

            EditorGUIUtility.labelWidth = 90;

            EditorGUI.PropertyField(linePosition, submeshIndex);
            if (submeshIndex.intValue < 0)
                submeshIndex.intValue = 0;

            EditorGUIUtility.labelWidth = labelWidth;

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var renderer = property.FindPropertyRelative("Renderer");

            var useCutoutProperty = property.FindPropertyRelative("CutoutDescriptionType");

            var rendererReference = renderer.objectReferenceValue as Renderer;
            var usingCutout = useCutoutProperty.intValue != (int)CutoutDescriptionType.None && rendererReference != null;

            var appropriateToUseEdgeDilate = renderer.objectReferenceValue != null && !(renderer.objectReferenceValue as Renderer).gameObject.isStatic;

            var linesCount = renderer.objectReferenceValue == null ||
                renderer.objectReferenceValue != null && (renderer.objectReferenceValue as Renderer).gameObject.isStatic
                ? 3.0f : 4.0f;

            if (usingCutout)
                linesCount += 1.0f;

            if (property.FindPropertyRelative("DilateRenderingMode").intValue == (int)DilateRenderMode.EdgeShift && appropriateToUseEdgeDilate)
                linesCount += 2.0f;

            float shift = 0.0f;
            var isDilateRenderingMode = property.FindPropertyRelative("DilateRenderingMode").intValue == (int)DilateRenderMode.EdgeShift;
            if (isDilateRenderingMode && PlayerSettings.stripUnusedMeshComponents)
                shift = EditorStyles.helpBox.CalcHeight(errorContent, lastWidth - 60) + EditorGUIUtility.singleLineHeight * 3.0f;

            return (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * (linesCount + 0.5f) + shift;
        }
    }
}