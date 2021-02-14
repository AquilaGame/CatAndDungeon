using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EPOOutline
{
    public static class SerializedPropertyExtentions
    {
        private static MethodInfo getFieldInfoAndStaticTypeFromProperty;

        public static Attribute[] GetFieldAttributes(this SerializedProperty prop)
        {
            if (getFieldInfoAndStaticTypeFromProperty == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assembly.GetTypes())
                    {
                        if (t.Name != "ScriptAttributeUtility")
                            continue;

                        getFieldInfoAndStaticTypeFromProperty = t.GetMethod("GetFieldAttributes", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var info in getFieldInfoAndStaticTypeFromProperty.GetParameters())
                            Debug.Log(info.ParameterType + " " + info.Name);

                        break;
                    }

                    if (getFieldInfoAndStaticTypeFromProperty != null)
                        break;
                }
            }

            var p = new object[] { prop, null };
            var fieldInfo = getFieldInfoAndStaticTypeFromProperty.Invoke(null, p) as Attribute[];
            return fieldInfo;
        }

        public static T GetCustomAttributeFromProperty<T>(this SerializedProperty prop) where T : System.Attribute
        {
            return Array.Find(GetFieldAttributes(prop), x => x is T) as T;
        }
    }

    [CustomPropertyDrawer(typeof(SerializedPass))]
    public class SerializedPassPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var drawingPosition = position;
            drawingPosition.height = EditorGUIUtility.singleLineHeight;

            var shaderProperty = property.FindPropertyRelative("shader");

            var currentShaderReference = shaderProperty.objectReferenceValue as Shader;

            var attribute = (SerializedPassInfoAttribute)null;// property.GetCustomAttributeFromProperty<SerializedPassInfoAttribute>();

            var prefix = attribute == null ? "Hidden/EPO/Fill/" : attribute.ShadersFolder;
            var fillLabel = currentShaderReference == null ? "none" : currentShaderReference.name.Substring(prefix.Length);
            if (shaderProperty.hasMultipleDifferentValues)
                fillLabel = "-";

            if (EditorGUI.DropdownButton(position, new GUIContent(label.text + " : " + fillLabel), FocusType.Passive))
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("none"), currentShaderReference == null && !shaderProperty.hasMultipleDifferentValues, () =>
                    {
                        shaderProperty.objectReferenceValue = null;
                        shaderProperty.serializedObject.ApplyModifiedProperties();
                    });

                var shaders = AssetDatabase.FindAssets("t:Shader");
                foreach (var shader in shaders)
                {
                    var loadedShader = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(shader), typeof(Shader)) as Shader;
                    if (!loadedShader.name.StartsWith(prefix))
                        continue;

                    menu.AddItem(new GUIContent(loadedShader.name.Substring(prefix.Length)), loadedShader == shaderProperty.objectReferenceValue && !shaderProperty.hasMultipleDifferentValues, () =>
                        {
                            shaderProperty.objectReferenceValue = loadedShader;
                            shaderProperty.serializedObject.ApplyModifiedProperties();
                        });
                }

                menu.ShowAsContext();
            }
            
            if (shaderProperty.hasMultipleDifferentValues)
                return;

            if (currentShaderReference != null)
            {
                position.x += EditorGUIUtility.singleLineHeight;
                position.width -= EditorGUIUtility.singleLineHeight;
                var properties = new Dictionary<string, SerializedProperty>();

                var serializedProperties = property.FindPropertyRelative("serializedProperties");

                for (var index = 0; index < serializedProperties.arraySize; index++)
                {
                    var subProperty = serializedProperties.GetArrayElementAtIndex(index);

                    var propertyName = subProperty.FindPropertyRelative("PropertyName");
                    var propertyValue = subProperty.FindPropertyRelative("Property");

                    if (propertyName == null || propertyValue == null)
                        break;

                    properties.Add(propertyName.stringValue, propertyValue);
                }

                var fillParametersPosition = position;
                fillParametersPosition.height = EditorGUIUtility.singleLineHeight;
                for (var index = 0; index < ShaderUtil.GetPropertyCount(currentShaderReference); index++)
                {
                    var propertyName = ShaderUtil.GetPropertyName(currentShaderReference, index);
                    if (!propertyName.StartsWith("_Public"))
                        continue;

                    fillParametersPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    SerializedProperty currentProperty;
                    if (!properties.TryGetValue(propertyName, out currentProperty))
                    {
                        serializedProperties.InsertArrayElementAtIndex(serializedProperties.arraySize);
                        currentProperty = serializedProperties.GetArrayElementAtIndex(serializedProperties.arraySize - 1);
                        currentProperty.FindPropertyRelative("PropertyName").stringValue = propertyName;
                        currentProperty = currentProperty.FindPropertyRelative("Property");

                        var tempMaterial = new Material(currentShaderReference);

                        switch (ShaderUtil.GetPropertyType(currentShaderReference, index))
                        {
                            case ShaderUtil.ShaderPropertyType.Color:
                                currentProperty.FindPropertyRelative("ColorValue").colorValue = tempMaterial.GetColor(propertyName);
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                currentProperty.FindPropertyRelative("VectorValue").vector4Value = tempMaterial.GetVector(propertyName);
                                break;
                            case ShaderUtil.ShaderPropertyType.Float:
                                currentProperty.FindPropertyRelative("FloatValue").floatValue = tempMaterial.GetFloat(propertyName);
                                break;
                            case ShaderUtil.ShaderPropertyType.Range:
                                currentProperty.FindPropertyRelative("FloatValue").floatValue = tempMaterial.GetFloat(propertyName);
                                break;
                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                currentProperty.FindPropertyRelative("TextureValue").objectReferenceValue = tempMaterial.GetTexture(propertyName);
                                break;
                        }

                        GameObject.DestroyImmediate(tempMaterial);

                        properties.Add(propertyName, currentProperty);
                    }

                    if (currentProperty == null)
                        continue;

                    var content = new GUIContent(ShaderUtil.GetPropertyDescription(currentShaderReference, index));

                    switch (ShaderUtil.GetPropertyType(currentShaderReference, index))
                    {
                        case ShaderUtil.ShaderPropertyType.Color:
                            var colorProperty = currentProperty.FindPropertyRelative("ColorValue");
                            colorProperty.colorValue = EditorGUI.ColorField(fillParametersPosition, content, colorProperty.colorValue, true, true, true);
                            break;
                        case ShaderUtil.ShaderPropertyType.Vector:
                            var vectorProperty = currentProperty.FindPropertyRelative("VectorValue");
                            vectorProperty.vector4Value = EditorGUI.Vector4Field(fillParametersPosition, content, vectorProperty.vector4Value);
                            break;
                        case ShaderUtil.ShaderPropertyType.Float:
                            EditorGUI.PropertyField(fillParametersPosition, currentProperty.FindPropertyRelative("FloatValue"), content);
                            break;
                        case ShaderUtil.ShaderPropertyType.Range:
                            var floatProperty = currentProperty.FindPropertyRelative("FloatValue");
                            floatProperty.floatValue = EditorGUI.Slider(fillParametersPosition, content, floatProperty.floatValue, 
                                ShaderUtil.GetRangeLimits(currentShaderReference, index, 1), 
                                ShaderUtil.GetRangeLimits(currentShaderReference, index, 2));
                            break;
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            EditorGUI.PropertyField(fillParametersPosition, currentProperty.FindPropertyRelative("TextureValue"), content);
                            break;
                    }

                    currentProperty.FindPropertyRelative("PropertyType").intValue = (int)ShaderUtil.GetPropertyType(currentShaderReference, index);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("shader").hasMultipleDifferentValues)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var shaderProperty = property.FindPropertyRelative("shader");
            var currentShaderReference = shaderProperty.objectReferenceValue as Shader;

            var additionalCount = 0;
            if (currentShaderReference != null)
            {
                for (var index = 0; index < ShaderUtil.GetPropertyCount(currentShaderReference); index++)
                {
                    var propertyName = ShaderUtil.GetPropertyName(currentShaderReference, index);
                    if (!propertyName.StartsWith("_Public"))
                        continue;

                    additionalCount++;
                }
            }

            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (additionalCount + 1);
        }
    }
}