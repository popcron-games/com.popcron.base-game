#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using BaseGame;

public static class ReflectionExtensions
{
    private static Log log = new(nameof(ReflectionExtensions));
    private static BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static readonly Dictionary<Type, Dictionary<int, FieldInfo>> fields = new();
    private static readonly Dictionary<Type, Dictionary<int, MethodInfo>> methods = new();

    public static object? GetFieldValue(this object obj, ReadOnlySpan<char> fieldName)
    {
        return GetField(obj.GetType(), fieldName)?.GetValue(obj);
    }

    public static void SetFieldValue(this object obj, ReadOnlySpan<char> fieldName, object value)
    {
        GetField(obj.GetType(), fieldName)?.SetValue(obj, value);
    }

    public static FieldInfo? GetField(this Type type, ReadOnlySpan<char> fieldName)
    {
        if (!fields.TryGetValue(type, out Dictionary<int, FieldInfo> fieldMap))
        {
            fieldMap = new Dictionary<int, FieldInfo>();
            while (type is not null)
            {
                foreach (FieldInfo field in type.GetFields(Flags))
                {
                    int nameHashCode = field.Name.GetSpanHashCode();
                    fieldMap[nameHashCode] = field;
                }

                type = type.BaseType;
            }
        }

        int fieldNameHashCode = fieldName.GetSpanHashCode();
        if (fieldMap.TryGetValue(fieldNameHashCode, out FieldInfo fieldInfo))
        {
            return fieldInfo;
        }

        return null;
    }

    public static object? InvokeStaticMethod(this Type type, ReadOnlySpan<char> methodName)
    {
        if (type.GetMethod(methodName) is MethodInfo method && method.IsStatic)
        {
            return method.Invoke(null, null);
        }
        else
        {
            log.LogErrorFormat("Could not find static method {0} on type {1}", methodName.ToString(), type.FullName);
            return null;
        }
    }

    public static object? InvokeStaticMethod<T1>(this Type type, ReadOnlySpan<char> methodName, T1 arg1)
    {
        if (type.GetMethod(methodName) is MethodInfo method && method.IsStatic)
        {
            return method.Invoke(null, new object?[] { arg1 });
        }
        else
        {
            log.LogErrorFormat("Could not find static method {0} on type {1}", methodName.ToString(), type.FullName);
            return null;
        }
    }

    public static object? InvokeStaticMethod<T1, T2>(this Type type, ReadOnlySpan<char> methodName, T1 arg1, T2 arg2)
    {
        if (type.GetMethod(methodName) is MethodInfo method && method.IsStatic)
        {
            return method.Invoke(null, new object?[] { arg1, arg2 });
        }
        else
        {
            log.LogErrorFormat("Could not find static method {0} on type {1}", methodName.ToString(), type.FullName);
            return null;
        }
    }

    public static object? InvokeStaticMethod<T1, T2, T3>(this Type type, ReadOnlySpan<char> methodName, T1 arg1, T2 arg2, T3 arg3)
    {
        if (type.GetMethod(methodName) is MethodInfo method && method.IsStatic)
        {
            return method.Invoke(null, new object?[] { arg1, arg2, arg3 });
        }
        else
        {
            log.LogErrorFormat("Could not find static method {0} on type {1}", methodName.ToString(), type.FullName);
            return null;
        }
    }

    public static object? InvokeStaticMethod<T1, T2, T3, T4>(this Type type, ReadOnlySpan<char> methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (type.GetMethod(methodName) is MethodInfo method && method.IsStatic)
        {
            return method.Invoke(null, new object?[] { arg1, arg2, arg3, arg4 });
        }
        else
        {
            log.LogErrorFormat("Could not find static method {0} on type {1}", methodName.ToString(), type.FullName);
            return null;
        }
    }

    public static MethodInfo? GetMethod(this Type type, ReadOnlySpan<char> methodName)
    {
        if (!methods.TryGetValue(type, out Dictionary<int, MethodInfo> methodMap))
        {
            methodMap = new Dictionary<int, MethodInfo>();
            while (type is not null)
            {
                foreach (MethodInfo method in type.GetMethods(Flags))
                {
                    int nameHashCode = method.Name.GetSpanHashCode();
                    methodMap[nameHashCode] = method;
                }

                type = type.BaseType;
            }
        }

        int methodNameHashCode = methodName.GetSpanHashCode();
        if (methodMap.TryGetValue(methodNameHashCode, out MethodInfo methodInfo))
        {
            return methodInfo;
        }

        return null;
    }
}
