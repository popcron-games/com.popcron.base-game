#nullable enable
using System;

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentDescriptionAttribute : Attribute
    {
        public readonly string description;

        public ComponentDescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
}
