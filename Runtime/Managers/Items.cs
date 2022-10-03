#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BaseGame
{
    public class Items : SingletonComponent<Items>, ISingletonManager
    {
        [SerializeField]
        private List<ItemAsset> assets = new();

        async UniTask IManager.Initialize()
        {
            await Addressables.LoadAssetsAsync<ItemAsset>("items", (asset) =>
            {
                assets.Add(asset);
            }).ToUniTask();
        }

        public static void AddPrefab(ItemAsset asset)
        {
            Items instance = Instance;
            if (!instance.assets.Contains(asset))
            {
                instance.assets.Add(asset);
            }
        }

        public static bool HasPrefab(ID id)
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static IItem GetPrefab(ID id)
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == id)
                {
                    return asset.PrefabItem;
                }
            }

            throw ExceptionBuilder.Format("Could not find item with ID {0}", id);
        }

        public static T GetPrefab<T>(ID id)
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == id && asset.PrefabItem is T item)
                {
                    return item;
                }
            }

            throw ExceptionBuilder.Format("Could not find prefab item with id {0}", id);
        }

        public static IItem Create(IItem prefab, ID? newId = null)
        {
            return prefab.Clone(newId);
        }

        public static IItem Create(ID prefabId, ID? newId = null)
        {
            IItem prefab = GetPrefab(prefabId);
            return Create(prefab, newId);
        }

        public static bool TryCreate(ID prefabId, [NotNullWhen(true)] out IItem? item, ID? newId = null)
        {
            try
            {
                item = Create(prefabId, newId);
                return true;
            }
            catch (Exception)
            {
                item = null;
                return false;
            }
        }

        public static T Create<T>(T prefab, ID? newId = null) where T : IItem
        {
            return (T)Create((IItem)prefab, newId);
        }

        /// <summary>
        /// Creates a new item instance of the prefab with this ID.
        /// </summary>
        public static T Create<T>(ID prefabId, ID? newId = null) where T : IItem
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == prefabId && asset.PrefabItem is T item)
                {
                    return Create(item, newId);
                }
            }

            throw ExceptionBuilder.Format("Could not find prefab item with id {0}", prefabId);
        }
    }
}
