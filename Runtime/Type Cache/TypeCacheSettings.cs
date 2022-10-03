#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BaseGame
{
    public class TypeCacheSettings : ScriptableSingleton<TypeCacheSettings>, IValidate
    {
        [SerializeField]
        private List<string> typeNamesToLoadFrom = new();

        public IEnumerable<string> TypeNamesToLoadFrom => typeNamesToLoadFrom;

        public static IEnumerable<Type>? GetTypesToLoad(Assembly assembly)
        {
            try
            {
                Type[] exportedTypes = assembly.GetExportedTypes();
                if (exportedTypes.Length > 0)
                {
                    return exportedTypes;
                }
            }
            catch { }

            return null;
        }

        public static FixedString GetTypeNameToLoad(Assembly assembly)
        {
            IEnumerable<Type> exportedTypes = GetTypesToLoad(assembly) ?? Array.Empty<Type>();
            foreach (Type type in exportedTypes)
            {
                ReadOnlySpan<char> typeName = type.FullName.AsSpan();
                ReadOnlySpan<char> assemblyName = assembly.GetName().Name.AsSpan();

                Span<char> typeNameToLoad = stackalloc char[typeName.Length + 2 + assemblyName.Length];
                typeName.CopyTo(typeNameToLoad);
                typeNameToLoad[typeName.Length] = ',';
                typeNameToLoad[typeName.Length + 1] = ' ';
                assemblyName.CopyTo(typeNameToLoad.Slice(typeName.Length + 2));

                return new FixedString(typeNameToLoad);
            }

            return FixedString.Empty;
        }

        bool IValidate.Validate()
        {
            bool changed = false;
            for (int i = typeNamesToLoadFrom.Count - 1; i >= 0; i--)
            {
                string typeName = typeNamesToLoadFrom[i];
                if (string.IsNullOrEmpty(typeName))
                {
                    typeNamesToLoadFrom.RemoveAt(i);
                    changed = true;
                }
                else
                {
                    try
                    {
                        Type.GetType(typeName, true);
                    }
                    catch (Exception)
                    {
                        typeNamesToLoadFrom.RemoveAt(i);
                        changed = true;
                    }
                }
            }

            return changed;
        }
    }
}