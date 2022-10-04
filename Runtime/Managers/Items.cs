#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BaseGame
{
    public class Items : SingletonComponent<Items>, IManager
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
            if (TryGetPrefab(id, out T? prefab))
            {
                return prefab;
            }

            throw ExceptionBuilder.Format("Could not find prefab item with id {0}", id);
        }

        public static bool TryGetPrefab<T>(ID id, [NotNullWhen(true)] out T? item)
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == id && asset.PrefabItem is T t)
                {
                    item = t;
                    return true;
                }
            }

            item = default;
            return false;
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
            if (TryGetPrefab(prefabId, out IItem? prefab))
            {
                item = Create(prefab, newId);
                return true;
            }

            item = default;
            return false;
        }

        public static bool TryCreate<T>(ID prefabId, [NotNullWhen(true)] out T? item, ID? newId = null)
        {
            Items instance = Instance;
            foreach (ItemAsset asset in instance.assets)
            {
                if (asset.ID == prefabId && asset.PrefabItem is T)
                {
                    item = (T)Create(asset.PrefabItem, newId);
                    return item is not null;
                }
            }

            item = default;
            return false;
        }
    }
}
