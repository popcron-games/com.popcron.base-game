#nullable enable
#if UNITY_EDITOR
using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static bool Contains(this SerializedProperty serializedProperty, string value)
    {
        if (serializedProperty.isArray)
        {
            int length = serializedProperty.arraySize;
            for (int i = 0; i < length; i++)
            {
                SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
                if (element.stringValue == value)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
#endif