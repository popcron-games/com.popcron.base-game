#nullable enable
#define STORE_STRING
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomPropertyDrawer(typeof(FixedStringAttribute))]
    public class FixedStringDrawer : PropertyDrawer
    {
        private static HashSet<int> checkedHashCodes = new();
        
        private bool editingHashCode = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FixedStringAttribute attribute = (FixedStringAttribute)this.attribute;
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                DrawProperty(position, property, label);
            }
            else if (property.propertyType == SerializedPropertyType.Generic)
            {
                //check if NetworkVariable<int>
                if (property.type == "NetworkVariable`1")
                {
                    SerializedProperty value = property.FindPropertyRelative("m_InternalValue");
                    if (value.propertyType == SerializedPropertyType.Integer)
                    {
                        DrawProperty(position, value, label);
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [FixedString] with int fields.");
            }
        }

        private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            //check if the hashcode has a string available for it
            int hashCode = property.intValue;
            #if STORE_STRING
            if (!checkedHashCodes.Contains(hashCode) && !FixedString.HasString(property.intValue))
            {
                checkedHashCodes.Add(hashCode);
                if (ID.idToName.TryGetValue(hashCode, out string text))
                {
                    FixedString.SetString(hashCode, text);
                }
            }
            #endif
            
            float buttonWidth = 20;
            Rect fieldPosition = new Rect(position.x, position.y, position.width - buttonWidth - 2, position.height);
            Rect buttonPosition = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            if (editingHashCode)
            {
                EditorGUI.PropertyField(fieldPosition, property, label);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                string text = EditorGUI.TextField(fieldPosition, label, FixedString.ToString(hashCode));
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = new FixedString(text).GetHashCode();
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.LabelField(fieldPosition, hashCode.ToString(), SerializedIDDrawer.hashCodeStyle);
                EditorGUI.EndDisabledGroup();
            }

            //button to toggle between hash code and fixed string
            if (GUI.Button(buttonPosition, editingHashCode ? "S" : "H"))
            {
                editingHashCode = !editingHashCode;
            }
        }
    }
}
#endif