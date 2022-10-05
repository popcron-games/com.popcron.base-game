#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BaseGame
{
    public interface IPlayer : IIdentifiable, IVariables
    {
        ulong OwnerClientId { get; }

        IInventory Inventory { get; }
        InputState GetInputState(ReadOnlySpan<char> name);
        bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState state);

        IEnumerable<(string, BaseVariable)> IVariables.GetVariables()
        {
            foreach (IItem item in Inventory)
            {
                if (item is IVariables variables)
                {
                    foreach ((string name, BaseVariable variable) in variables.GetVariables())
                    {
                        yield return (name, variable);
                    }
                }
            }
        }
    }
}