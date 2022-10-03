#nullable enable

using UnityEngine;

namespace BaseGame
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent(out T component))
            {
                return component;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
    }
}