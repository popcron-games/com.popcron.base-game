#nullable enable
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

public static class EditorAssemblyCache
{
    public readonly static HashSet<Assembly> assemblies = new(AppDomain.CurrentDomain.GetAssemblies());
}
#endif