using System;

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnEventAttribute : Attribute
    {
        public readonly Type eventType;

        public OnEventAttribute(Type eventType)
        {
            this.eventType = eventType;
        }
    }
}