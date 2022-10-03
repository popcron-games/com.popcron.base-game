#nullable enable
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MustBeAssignedAttribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MustBeAssignedAttribute))]
    public class MustBeAssignedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //show warning if property is of Object type and is unassigned
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                if (label != null)
                {
                    position = EditorGUI.PrefixLabel(position, label);
                }

                Color originalColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.PropertyField(position, property, GUIContent.none);
                GUI.color = originalColor;

                //error text style aligned to the right - 10px from the right edge
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleRight;
                style.normal.textColor = Color.red;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 12;

                //show error on the last rect
                position.xMax -= 18;
                EditorGUI.LabelField(position, "Must be assigned!", style);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
#endif
}
