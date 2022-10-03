#nullable enable
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomEditor(typeof(SealedMonoBehaviour), true)]
    public class MonoBehaviourInspector : Editor
    {
        private Component? component;

        private void OnEnable()
        {
            component = target as Component;
        }

        public override void OnInspectorGUI()
        {
            //show description
            Type type = component?.GetType() ?? typeof(Component);
            object[] descriptionAttributes = type.GetCustomAttributes(typeof(ComponentDescriptionAttribute), false);
            if (descriptionAttributes.Length > 0)
            {
                ComponentDescriptionAttribute attr = (ComponentDescriptionAttribute)descriptionAttributes[0];
                EditorGUILayout.HelpBox(attr.description, MessageType.Info);
            }

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if (component is IValidate validate)
            {
                if (validate.Validate())
                {
                    EditorUtility.SetDirty(component);
                }
            }
        }
    }
}
#endif