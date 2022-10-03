#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public struct SerializedType
    {
        [SerializeField]
        private string fullName;

        public Type? Type
        {
            get
            {
                if (TypeCache.TryGetType(fullName, out Type? type))
                {
                    return type;
                }
                else
                {
                    return null;
                }
            }
        }

        public static implicit operator Type(SerializedType serializedType)
        {
            return TypeCache.GetType(serializedType.fullName);
        }
    }
}