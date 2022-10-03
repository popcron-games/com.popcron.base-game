#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnEnableAttribute : Attribute
    {
        public bool InvokeInEditMode { get; }
        public RuntimeInitializeLoadType LoadType { get; }

        public OnEnableAttribute(bool invokeInEditMode = false, RuntimeInitializeLoadType loadType = RuntimeInitializeLoadType.AfterAssembliesLoaded)
        {
            InvokeInEditMode = invokeInEditMode;
            LoadType = loadType;
        }
    }
}