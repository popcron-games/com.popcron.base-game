#nullable enable
using UnityEngine;

namespace BaseGame
{
    public class Validator
    {
        public static bool Validate<T>(T obj)
        {
            if (obj is IValidate validate)
            {
                return validate.Validate();
            }

            return true;
        }

        public static void PerformValidation<T>(T obj)
        {
            if (obj is IValidate validate)
            {
                if (validate.Validate())
                {
#if UNITY_EDITOR
                    if (obj is Object unityObject)
                    {
                        UnityEditor.EditorUtility.SetDirty(unityObject);
                    }
#endif
                }
            }
        }
    }
}
