#nullable enable
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Generic && fieldInfo.FieldType == typeof(SerializedType))
            {
                SerializedProperty name = property.FindPropertyRelative("fullName");
                Type assignableFrom = typeof(object);

                //get type filter attribute
                if (fieldInfo.GetCustomAttributes(typeof(TypeFilterAttribute), false) is TypeFilterAttribute[] attributes && attributes.Length > 0)
                {
                    TypeFilterAttribute attribute = attributes[0];
                    assignableFrom = attribute.assignableFrom;
                }

                List<Type> types = GetTypesToShow(assignableFrom);
                string[] options = new string[types.Count];
                for (int i = 0; i < types.Count; i++)
                {
                    options[i] = types[i].FullName;
                }

                if (label != null)
                {
                    position = EditorGUI.PrefixLabel(position, label);
                }

                int index = Array.IndexOf(options, name.stringValue);
                index = EditorGUI.Popup(position, index, options);
                if (index != -1)
                {
                    name.stringValue = options[index];
                }
                else
                {
                    name.stringValue = string.Empty;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private static List<Type> GetTypesToShow(Type assignableFrom)
        {
            List<Type> types = new();
            foreach (Type type in TypeCache.GetTypesAssignableFrom(assignableFrom))
            {
                if (type.IsAbstract || type.IsGenericTypeDefinition)
                {
                    continue;
                }

                types.Add(type);
            }

            return types;
        }
    }
}
#endif