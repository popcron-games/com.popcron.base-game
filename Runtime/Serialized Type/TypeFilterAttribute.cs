#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeFilterAttribute : PropertyAttribute
    {
        public readonly Type assignableFrom;

        public TypeFilterAttribute()
        {
            assignableFrom = typeof(object);
        }

        /// <summary>
        /// Will only show types that inherit from the given type.
        /// </summary>
        public TypeFilterAttribute(Type assignableFrom)
        {
            this.assignableFrom = assignableFrom;
        }
    }
}