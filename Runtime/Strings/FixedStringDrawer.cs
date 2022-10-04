#nullable enable
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomPropertyDrawer(typeof(FixedStringAttribute))]
    public class FixedStringDrawer : PropertyDrawer
    {
        private bool editingHashCode = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FixedStringAttribute attribute = (FixedStringAttribute)this.attribute;
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = DrawProperty(position, property.intValue, label);
            }
            else if (property.propertyType == SerializedPropertyType.Generic)
            {
                //check if NetworkVariable<int>
                if (property.type == "NetworkVariable`1")
                {
                    SerializedProperty value = property.FindPropertyRelative("m_InternalValue");
                    if (value.propertyType == SerializedPropertyType.Integer)
                    {
                        property.intValue = DrawProperty(position, property.intValue, label);
                    }
                }
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = DrawProperty(position, property.stringValue, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [FixedString] with int or string fields.");
            }
        }

        private string DrawProperty(Rect position, string value, GUIContent label)
        {
            int hashCode = value.GetSpanHashCode();
            float buttonWidth = 20;
            Rect fieldPosition = new Rect(position.x, position.y, position.width - buttonWidth - 2, position.height);
            Rect buttonPosition = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            if (editingHashCode)
            {
                hashCode = EditorGUI.IntField(fieldPosition, label, hashCode);
                if (GUI.Button(buttonPosition, "H"))
                {
                    editingHashCode = false;
                }

                if (FixedString.TryGetString(hashCode, out string text))
                {
                    value = text;
                }
                else if (ID.TryGetString(hashCode, out text))
                {
                    value = text;
                }
            }
            else
            {
                value = EditorGUI.TextField(fieldPosition, label, value);
                hashCode = new FixedString(value).GetHashCode();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.LabelField(fieldPosition, hashCode.ToString(), SerializedIDDrawer.hashCodeStyle);
                EditorGUI.EndDisabledGroup();

                if (GUI.Button(buttonPosition, "S"))
                {
                    editingHashCode = true;
                }
            }

            return value;
        }

        private int DrawProperty(Rect position, int hashCode, GUIContent label)
        {
            float buttonWidth = 20;
            Rect fieldPosition = new Rect(position.x, position.y, position.width - buttonWidth - 2, position.height);
            Rect buttonPosition = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            if (editingHashCode)
            {
                hashCode = EditorGUI.IntField(fieldPosition, label, hashCode);
                if (GUI.Button(buttonPosition, "H"))
                {
                    editingHashCode = false;
                }
            }
            else
            {
                if (FixedString.TryGetString(hashCode, out string text))
                {
                    text = EditorGUI.TextField(fieldPosition, label, text);
                    hashCode = FixedString.Parse(text);
                }
                else if (ID.TryGetString(hashCode, out text))
                {
                    text = EditorGUI.TextField(fieldPosition, label, text);
                    hashCode = ID.Parse(text);
                }
                else
                {
                    hashCode = EditorGUI.IntField(fieldPosition, label, hashCode);
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.LabelField(fieldPosition, hashCode.ToString(), SerializedIDDrawer.hashCodeStyle);
                EditorGUI.EndDisabledGroup();

                if (GUI.Button(buttonPosition, "S"))
                {
                    editingHashCode = true;
                }
            }

            return hashCode;
        }
    }
}
#endif