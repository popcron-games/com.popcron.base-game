#nullable enable
#if UNITY_EDITOR
using UnityEditor;

namespace BaseGame
{
    [CustomEditor(typeof(IdentifiableAsset), true)]
    public class IdentifiableAssetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            Validator.PerformValidation(target);
        }
    }
}
#endif