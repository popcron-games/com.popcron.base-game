#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BaseGame.Events
{
    public static class GlobalEventBus<E> where E : IEvent
    {
        public delegate void EventDelegate(ref E e);
        private static readonly Log log = new Log(nameof(GlobalEventBus<E>));
        private static readonly List<EventDelegate> methodListeners = new();
        private static readonly List<IStaticListener<E>> staticListeners = new();

        static GlobalEventBus()
        {
            RegisterMethodsAsListeners();
            RegisterStaticListeners();
        }

        public static void Dispatch(ref E e)
        {
            for (int i = methodListeners.Count - 1; i >= 0; i--)
            {
                methodListeners[i].Invoke(ref e);
            }

            for (int i = staticListeners.Count - 1; i >= 0; i--)
            {
                staticListeners[i].OnEvent(e);
            }
        }

        public static void AddListener(EventDelegate callback)
        {
            methodListeners.Add(callback);
        }

        private static void RegisterStaticListeners()
        {
            foreach (Type type in TypeCache.GetTypesAssignableFrom<IStaticListener<E>>())
            {
                IStaticListener<E> listener = (IStaticListener<E>)Activator.CreateInstance(type);
                staticListeners.Add(listener);
            }
        }

        private static void RegisterMethodsAsListeners()
        {
            foreach ((MethodInfo method, OnEventAttribute attribute) in TypeCache.GetMethodsWithAttribute<OnEventAttribute>())
            {
                if (method.IsStatic)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    Type eventType = attribute.eventType;
                    if (parameters.Length == 1 && parameters[0].ParameterType == eventType)
                    {
                        if (eventType == typeof(E))
                        {
                            EventDelegate callback = (EventDelegate)Delegate.CreateDelegate(typeof(EventDelegate), method);
                            AddListener(callback);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("Method {0} should have 1 parameter of type {1}", method.Name, eventType);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Method {0} is not static, but has an OnEventAttribute", method.Name);
                }
            }
        }
    }
}