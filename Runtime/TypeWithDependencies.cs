#nullable enable
using System;
using System.Collections.Generic;

namespace BaseGame
{
    public class TypeWithDependencies
    {
        public readonly Type type;
        public readonly List<Type> dependencies;

        public TypeWithDependencies(Type type, List<Type> dependencies)
        {
            this.type = type;
            this.dependencies = dependencies;
        }

        public static IEnumerable<T> Sort<T>(List<T> values)
        {
            List<TypeWithDependencies> types = new();
            Dictionary<TypeWithDependencies, T> typeToValue = new();
            foreach (T value in values)
            {
                if (value is not null)
                {
                    Type type = value.GetType();
                    List<Type> dependencies = new();
                    if (value is IDependentUpon dependentUpon)
                    {
                        dependencies.AddRange(dependentUpon.Dependencies);
                    }

                    TypeWithDependencies typeWithDependencies = new(type, dependencies);
                    types.Add(typeWithDependencies);
                    typeToValue.Add(typeWithDependencies, value);
                }
            }

            List<TypeWithDependencies> sorted = new();
            while (types.Count > 0)
            {
                TypeWithDependencies type = types[0];
                types.RemoveAt(0);
                if (type.dependencies.Count == 0)
                {
                    sorted.Add(type);
                }
                else
                {
                    bool hasDependencies = false;
                    for (int i = 0; i < type.dependencies.Count; i++)
                    {
                        Type dependency = type.dependencies[i];
                        if (sorted.FindIndex(x => x.type == dependency) == -1)
                        {
                            hasDependencies = true;
                            break;
                        }
                    }

                    if (hasDependencies)
                    {
                        types.Add(type);
                    }
                    else
                    {
                        sorted.Add(type);
                    }
                }
            }

            foreach (TypeWithDependencies type in sorted)
            {
                yield return typeToValue[type];
            }
        }
    }
}
