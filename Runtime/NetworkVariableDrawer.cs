#nullable enable
#if UNITY_EDITOR
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomPropertyDrawer(typeof(NetworkVariableBase), true)]
    public class NetworkVariableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type == "NetworkVariable`1")
            {
                SerializedProperty value = property.FindPropertyRelative("m_InternalValue");
                EditorGUI.PropertyField(position, value, label);
            }
        }
    }
}
#endif