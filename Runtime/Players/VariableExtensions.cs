#nullable enable
using System;
using System.Collections.Generic;

namespace BaseGame
{
    public static class VariableExtensions
    {
        public static IEnumerable<Variable<T>> GetVariables<T>(this IVariables? variables, string? name) where T : unmanaged
        {
            if (name is not null && variables is not null)
            {
                int nameHash = name.GetSpanHashCode();
                foreach ((string name, BaseVariable variable) v in variables.GetVariables())
                {
                    if (v.name.GetSpanHashCode() == nameHash)
                    {
                        if (v.variable is Variable<T> typedVariable)
                        {
                            yield return typedVariable;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Variable<T>> GetVariables<T>(this IVariables? variables, ID id) where T : unmanaged
        {
            if (variables is not null)
            {
                int nameHash = id.GetHashCode();
                foreach ((string name, BaseVariable variable) v in variables.GetVariables())
                {
                    if (v.name.GetSpanHashCode() == nameHash)
                    {
                        if (v.variable is Variable<T> typedVariable)
                        {
                            yield return typedVariable;
                        }
                    }
                }
            }
        }
    }
}