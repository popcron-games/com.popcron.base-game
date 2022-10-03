#nullable enable
#if UNITY_EDITOR
using UnityEditor;

namespace BaseGame
{
    [CustomEditor(typeof(IdentifiableAsset), true)]
    public class IdentifiableAssetInspector : Editor
    {
        private IdentifiableAsset? asset;

        private void OnEnable()
        {
            asset = target as IdentifiableAsset;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if (asset is IValidate validate)
            {
                if (validate.Validate())
                {
                    EditorUtility.SetDirty(asset);
                }
            }
        }
    }
}
#endif