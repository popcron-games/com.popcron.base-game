#nullable enable
#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;

namespace UnityEngine
{
    [InitializeOnLoad]
    public static class CheckForMissingScriptableSingletons
    {
        static CheckForMissingScriptableSingletons()
        {
            TryToCheck();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        [DidReloadScripts]
        private static void TryToCheck()
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            foreach (Type type in TypeCache.types)
            {
                //must not be generic itself
                if (type.IsGenericTypeDefinition)
                {
                    continue;
                }

                //has to inherit from ScriptableSingleton<type>
                if (type.BaseType is not null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ScriptableSingleton<>))
                {
                    _ = type.GetProperty("Instance", flags).GetValue(null);
                }
            }
        }
    }
}
#endif