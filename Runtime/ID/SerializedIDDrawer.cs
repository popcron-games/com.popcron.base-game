#nullable enable
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomPropertyDrawer(typeof(SerializedID))]
    public class SerializedIDDrawer : PropertyDrawer
    {
        public static readonly GUIStyle hashCodeStyle;

        static SerializedIDDrawer()
        {
            hashCodeStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold,
                fontSize = 10
            };

            hashCodeStyle.normal.textColor *= new Color(1, 1, 1, 0.4f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            SerializedProperty text = property.FindPropertyRelative("text");
            EditorGUI.PropertyField(position, text, GUIContent.none);

            EditorGUI.BeginDisabledGroup(true);
            int hashCode = ID.Parse(text.stringValue);
            EditorGUI.LabelField(position, hashCode.ToString(), hashCodeStyle);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif