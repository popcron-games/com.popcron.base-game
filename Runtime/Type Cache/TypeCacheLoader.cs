#nullable enable
using System.Reflection;
using UnityEngine;

namespace BaseGame
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class TypeCacheLoader
    {
        private static bool hasLoaded;

        static TypeCacheLoader()
        {
            TryToLoad();
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScriptsWereReloaded()
        {
            TryToLoad();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AssembliesWereLoaded()
        {
            TryToLoad();
        }

        public static void TryToLoad()
        {
            if (hasLoaded) return;
            hasLoaded = true;

            TypeCache.LoadTypesFromAssembly(Assembly.GetExecutingAssembly());

            TypeCacheSettings settings = TypeCacheSettings.Instance;
            foreach (string typeName in settings.TypeNamesToLoadFrom)
            {
                TypeCache.CacheAssemblyOf(typeName);
            }
        }
    }
}