#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BaseGame
{
    public interface IInventory : IReadOnlyList<IItem>
    {
        void Add<T>(T item) where T : IItem;
        void Remove<T>(T item) where T : IItem;
        bool TryGetFirst<T>([NotNullWhen(true)] out T? item);
    }
}