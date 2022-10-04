#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public struct SerializedType
    {
        [SerializeField]
        private string fullName;

        public Type Type
        {
            get
            {
                if (!string.IsNullOrEmpty(fullName))
                {
                    if (TypeCache.TryGetType(fullName, out Type? type))
                    {
                        return type;
                    }
                    else
                    {
                        throw ExceptionBuilder.Format("Type {0} could not be found", fullName);
                    }
                }
                else
                {
                    throw new NullReferenceException("Serialized type has no backing type name value");
                }
            }
        }

        public bool TryGetType([NotNullWhen(true)] out Type? type)
        {
            if (!string.IsNullOrEmpty(fullName))
            {
                return TypeCache.TryGetType(fullName, out type);
            }
            else
            {
                type = null;
                return false;
            }
        }

        public static implicit operator Type(SerializedType serializedType) => serializedType.Type;
    }
}