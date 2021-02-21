using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    [CustomPropertyDrawer(typeof(Outlinable.OutlineProperties))]
    public class OutlinePropertiesPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelPosition = position;
            labelPosition.height = EditorGUIUtility.singleLineHeight;

            var enabledProperty = property.FindPropertyRelative("enabled");
            enabledProperty.boolValue = EditorGUI.ToggleLeft(labelPosition, label, enabledProperty.boolValue);

            var drawingPosition = position;
            drawingPosition.width -= EditorGUIUtility.singleLineHeight * 1.5f;
            drawingPosition.x += EditorGUIUtility.singleLineHeight;
            drawingPosition.height = EditorGUIUtility.singleLineHeight;

            NewLine(ref drawingPosition);

            //var style = property.FindPropertyRelative("outlineRenderingPass");
            //var styleHeight = EditorGUI.GetPropertyHeight(style);
            //var propertyDrawingPosition = drawingPosition;
            //EditorGUI.PropertyField(propertyDrawingPosition, style);

            //drawingPosition.y += styleHeight + EditorGUIUtility.standardVerticalSpacing;

            var infoPosition = drawingPosition;
            infoPosition.height = EditorGUIUtility.singleLineHeight * 3.0f + EditorGUIUtility.standardVerticalSpacing * 2.0f;
            EditorGUI.HelpBox(infoPosition, "Changing dilate and blur settings will enable info buffer which will increase draw calls and will have some performance impact. Use Outliner settings if you don't need per object settings.", MessageType.Warning);

            NewLine(ref drawingPosition);
            NewLine(ref drawingPosition);
            NewLine(ref drawingPosition);

            var colorPosition = drawingPosition;
            colorPosition.width = 46;

            var colorProperty = property.FindPropertyRelative("color");
            colorProperty.colorValue = EditorGUI.ColorField(colorPosition, GUIContent.none, colorProperty.colorValue, true, true, true);

            float width = 45;
            var shiftPropertiesPositions = drawingPosition;
            shiftPropertiesPositions.x += colorPosition.width + EditorGUIUtility.standardVerticalSpacing;
            shiftPropertiesPositions.width -= colorPosition.width + EditorGUIUtility.standardVerticalSpacing;
            shiftPropertiesPositions.width /= 2;
            shiftPropertiesPositions.width -= EditorGUIUtility.standardVerticalSpacing + width;

            shiftPropertiesPositions.x += width;

            var labelRect = shiftPropertiesPositions;
            labelRect.x -= width - EditorGUIUtility.singleLineHeight * 0.25f;
            labelRect.width = width - EditorGUIUtility.singleLineHeight * 0.5f;

            EditorGUI.LabelField(labelRect, new GUIContent("Dilate"));

            EditorGUI.PropertyField(shiftPropertiesPositions, property.FindPropertyRelative("dilateShift"), GUIContent.none);

            shiftPropertiesPositions.x += shiftPropertiesPositions.width + EditorGUIUtility.standardVerticalSpacing + width;

            labelRect.x += shiftPropertiesPositions.width + width;
            EditorGUI.LabelField(labelRect, new GUIContent("Blur"));

            EditorGUI.PropertyField(shiftPropertiesPositions, property.FindPropertyRelative("blurShift"), GUIContent.none);

            NewLine(ref drawingPosition);

            EditorGUI.PropertyField(drawingPosition, property.FindPropertyRelative("fillPass"), new GUIContent("Fill parameters"));

            property.serializedObject.ApplyModifiedProperties();
        }

        private void NewLine(ref Rect rect)
        {
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3.0f + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("fillPass")) +
                EditorGUIUtility.singleLineHeight * 2.0f + EditorGUIUtility.standardVerticalSpacing * 1.0f;// + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("outlineRenderingPass"));
        }
    }
}