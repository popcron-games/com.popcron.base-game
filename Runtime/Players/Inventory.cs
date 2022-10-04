#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class Inventory : IInventory
    {
        [SerializeReference, SerializeReferenceButton]
        private List<IItem> items = new();

        private Log log = new(nameof(Inventory));

        public int Count => items.Count;
        public IReadOnlyList<IItem> Items => items;

        public IItem this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public void Enabled()
        {
            foreach (IItem item in items)
            {
                PlayerLoop.Add(item);
                if (item is IUnityLifecycle unity)
                {
                    unity.OnEnabled();
                }
            }
        }

        public void Disabled()
        {
            foreach (IItem item in items)
            {
                if (item is IUnityLifecycle unity)
                {
                    unity.OnDisabled();
                }

                PlayerLoop.Remove(item);
            }
        }

        public void Add<T>(T item) where T : IItem
        {
            PlayerLoop.Add(item);

            items.Add(item);
            log.LogInfoFormat("Added {0} to inventory", item);

            item.AddedTo(this);

            if (item is IUnityLifecycle unity)
            {
                unity.OnEnabled();
            }
        }

        public void Remove<T>(T item) where T : IItem
        {
            if (item is IUnityLifecycle unity)
            {
                unity.OnDisabled();
            }

            item.RemovedFrom(this);
            
            items.Remove(item);
            log.LogInfoFormat("Removed {0} from inventory", item);

            PlayerLoop.Remove(item);
        }

        public void AddRange(IEnumerable<ItemAsset> itemAssets)
        {
            foreach (ItemAsset asset in itemAssets)
            {
                IItem prefabItem = asset.PrefabItem;
                Add(BaseGame.Items.Create(prefabItem));
            }
        }

        public bool TryGet(ID id, [NotNullWhen(true)] out IItem? item)
        {
            foreach (IItem existingItem in items)
            {
                if (existingItem.ID == id)
                {
                    item = existingItem;
                    return true;
                }
            }

            item = default;
            return false;
        }

        public bool TryGet<T>(ID id, [NotNullWhen(true)] out T? item)
        {
            foreach (IItem existingItem in items)
            {
                if (existingItem.ID == id && existingItem is T t)
                {
                    item = t;
                    return true;
                }
            }

            item = default;
            return false;
        }

        public IItem? Get(ID id)
        {
            foreach (IItem item in items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return default;
        }

        public T? Get<T>(ID id)
        {
            foreach (IItem item in items)
            {
                if (item.ID == id && item is T t)
                {
                    return t;
                }
            }

            return default;
        }

        public T? GetFirst<T>()
        {
            foreach (IItem item in items)
            {
                if (item is T t)
                {
                    return t;
                }
            }

            return default;
        }

        public bool Contains<T>()
        {
            foreach (IItem item in items)
            {
                if (item is T)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(IItem item)
        {
            return items.Contains(item);
        }

        public IEnumerable<T> GetAll<T>()
        {
            foreach (IItem item in items)
            {
                if (item is T t)
                {
                    yield return t;
                }
            }
        }
    }
}