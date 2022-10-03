#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BaseGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TypeCache
{
    public static readonly HashSet<Type> types = new();

    private static readonly Dictionary<int, Type> nameToType = new();
    private static Dictionary<Type, List<MethodInfo>> methodsWithAttribute = new();
    private static Log log = new("TypeCache");

    public static void CacheAssemblyOf(ReadOnlySpan<char> typeFullName)
    {
        Type type = Type.GetType(typeFullName.ToString());
        if (type is null)
        {
            log.LogErrorFormat("Could not find type {0} to cache assembly of", typeFullName);
        }
        else
        {
            CacheAssemblyOf(type);
        }
    }

    public static void CacheAssemblyOf(Type? type)
    {
        if (type is not null)
        {
            LoadTypesFromAssembly(type.Assembly);
        }
    }

    public static void LoadTypesFromAssembly(Assembly assembly)
    {
        int loadedTypes = 0;
        if (TypeCacheSettings.GetTypesToLoad(assembly) is IEnumerable<Type> typesToLoad)
        {
            foreach (Type type in typesToLoad)
            {
                if (!types.Contains(type))
                {
                    types.Add(type);
                    nameToType[type.Name.GetSpanHashCode()] = type;
                    nameToType[type.FullName.GetSpanHashCode()] = type;
                    loadedTypes++;
                }
            }
        }
    }

    public static Type GetType(ReadOnlySpan<char> typeName)
    {
        if (TryGetType(typeName, out Type? foundType))
        {
            return foundType;
        }
        else
        {
#if UNITY_EDITOR
            //find type in all assemblies
            TypeCacheSettings settings = TypeCacheSettings.Instance;
            SerializedObject serializedObject = new SerializedObject(settings);
            SerializedProperty typeNamesToLoadFrom = serializedObject.FindProperty("typeNamesToLoadFrom");
            foreach (Assembly assembly in EditorAssemblyCache.assemblies)
            {
                IEnumerable<Type> typesToLoad = TypeCacheSettings.GetTypesToLoad(assembly) ?? Array.Empty<Type>();
                foreach (Type type in typesToLoad)
                {
                    bool add = false;
                    if (type.Name.AsSpan().Equals(typeName, StringComparison.Ordinal))
                    {
                        add = true;
                    }
                    else if (type.FullName.AsSpan().Equals(typeName, StringComparison.Ordinal))
                    {
                        add = true;
                    }

                    if (add)
                    {
                        foundType = type;
                        string typeNameToLoad = TypeCacheSettings.GetTypeNameToLoad(assembly);
                        if (!typeNamesToLoadFrom.Contains(typeNameToLoad))
                        {
                            typeNamesToLoadFrom.InsertArrayElementAtIndex(typeNamesToLoadFrom.arraySize);
                            typeNamesToLoadFrom.GetArrayElementAtIndex(typeNamesToLoadFrom.arraySize - 1).stringValue = typeNameToLoad;
                            serializedObject.ApplyModifiedProperties();
                        }

                        LoadTypesFromAssembly(assembly);
                        return foundType;
                    }
                }
            }

#endif
            throw ExceptionBuilder.Format("Could not find type {0}", typeName);
        }
    }

    public static bool TryGetType(ReadOnlySpan<char> typeName, [NotNullWhen(true)] out Type? type)
    {
        foreach (Type t in types)
        {
            if (t.Name.AsSpan().Equals(typeName, StringComparison.Ordinal))
            {
                type = t;
                return true;
            }
            else if (t.FullName.AsSpan().Equals(typeName, StringComparison.Ordinal))
            {
                type = t;
                return true;
            }
        }

        type = null;
        return false;
    }

    public static IEnumerable<(MethodInfo method, T attribute)> GetMethodsWithAttribute<T>() where T : Attribute
    {
        if (!methodsWithAttribute.TryGetValue(typeof(T), out List<MethodInfo> methods))
        {
            methods = new List<MethodInfo>();
            foreach (Type type in types)
            {
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    if (method.GetCustomAttribute<T>() is T attribute)
                    {
                        methods.Add(method);
                    }
                }
            }

            methodsWithAttribute.Add(typeof(T), methods);
        }

        foreach (MethodInfo method in methods)
        {
            yield return (method, method.GetCustomAttribute<T>());
        }
    }

    public static IEnumerable<Type> GetTypesAssignableFrom(Type type)
    {
        foreach (Type t in types)
        {
            if (type.IsAssignableFrom(t))
            {
                yield return t;
            }
        }
    }

    /// <summary>
    /// Returns all types that are assignable from this type.
    /// Works for interfaces too.
    /// </summary>
    public static IEnumerable<Type> GetTypesAssignableFrom<T>()
    {
        foreach (Type type in types)
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                yield return type;
            }
        }
    }

    /// <summary>
    /// Returns all types that inherit from the specified base class
    /// </summary>
    public static IEnumerable<Type> GetSubclassesOf<T>() where T : class
    {
        foreach (Type type in types)
        {
            if (type.IsSubclassOf(typeof(T)))
            {
                yield return type;
            }
        }
    }
}