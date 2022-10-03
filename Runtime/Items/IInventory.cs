#nullable enable
using System.Collections.Generic;

namespace BaseGame
{
    public interface IInventory
    {
        int Count { get; }
        IItem this[int index] { get; set; }
        IReadOnlyList<IItem> Items { get; }

        void Add<T>(T item) where T : IItem;
        void Remove<T>(T item) where T : IItem;
        bool Contains<T>();
        bool Contains(IItem item);
        T? GetFirst<T>();
    }
}