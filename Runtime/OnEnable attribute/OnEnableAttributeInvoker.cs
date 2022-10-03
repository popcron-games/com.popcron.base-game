#nullable enable
using System.Reflection;
using BaseGame.Events;
using UnityEngine;

namespace BaseGame
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class OnEnableAttributeInvoker
    {
        private static Log log = new(nameof(OnEnableAttributeInvoker));
        private static bool hasInvoked;

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScriptsWereReloaded()
        {
            TryToInvoke();
        }

        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorWasLoaded()
        {
            TryToInvoke();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            TryToInvoke(RuntimeInitializeLoadType.BeforeSceneLoad);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitializeAfterSceneLoad()
        {
            TryToInvoke(RuntimeInitializeLoadType.AfterSceneLoad);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeSubsystemRegistration()
        {
            TryToInvoke(RuntimeInitializeLoadType.SubsystemRegistration);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void RuntimeInitializeAfterAssembliesLoaded()
        {
            TryToInvoke(RuntimeInitializeLoadType.AfterAssembliesLoaded);
        }

        private static void TryToInvoke(RuntimeInitializeLoadType? loadType = null)
        {
            if (hasInvoked) return;
            hasInvoked = true;

            bool isPlaying = Application.isPlaying;
            foreach ((MethodInfo method, OnEnableAttribute attribute) in TypeCache.GetMethodsWithAttribute<OnEnableAttribute>())
            {
                //skip if the load type doesnt match
                if (loadType is not null && attribute.LoadType != loadType)
                {
                    continue;
                }

                //skip if we dont want to invoke during edit mode
                if (!isPlaying && !attribute.InvokeInEditMode)
                {
                    continue;
                }

                if (method.IsStatic)
                {
                    int parameterCount = method.GetParameters().Length;
                    if (parameterCount == 0)
                    {
                        method.Invoke(null, null);
                    }
                    else
                    {
                        log.LogErrorFormat("Method {0} has {1} parameters, but should have 0", method.Name, parameterCount);
                    }
                }
                else
                {
                    log.LogErrorFormat("Method {0} is not static", method.Name);
                }
            }
        }
    }
}