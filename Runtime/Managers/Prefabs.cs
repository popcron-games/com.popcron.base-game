#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace BaseGame
{
    public class Prefabs : SingletonComponent<Prefabs>, IManager
    {
        [SerializeField]
        private List<PrefabData> prefabs = new();

        async UniTask IManager.Initialize()
        {
            await Addressables.LoadAssetsAsync<Object>("prefabs", (prefab) =>
            {
                Add(prefab);
            }).ToUniTask();

            await Addressables.LoadAssetsAsync<Object>("items", (prefab) =>
            {
                Add(prefab);
            }).ToUniTask();
        }

        public static bool HasPrefab(ID id)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefab in instance.prefabs)
            {
                if (prefab.ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Add<T>(T originalPrefab)
        {
            ID id;
            if (originalPrefab is IIdentifiable identifiable)
            {
                id = identifiable.ID;
            }
            else if (originalPrefab is Object unityPrefab)
            {
                id = unityPrefab.name;
            }
            else
            {
                throw ExceptionBuilder.Format("Prefab {0} does not have an ID", originalPrefab);
            }

            object? prefab = PrefabData.GetPrefab(originalPrefab);
            Prefabs instance = Instance;
            foreach (PrefabData data in instance.prefabs)
            {
                if (data.ID == id && data.Prefab == prefab)
                {
                    log.LogWarningFormat("Prefab {0} already exists", id);
                    return;
                }
            }

            instance.prefabs.Add(new PrefabData(id, originalPrefab));
            log.LogInfoFormat("Added prefab {0} with ID {1}", originalPrefab, id);
        }

        /// <summary>
        /// Enumerates through all prefabs of this type.
        /// </summary>
        public static IEnumerable<T> GetAll<T>()
        {
            Prefabs instance = Instance;
            foreach (PrefabData data in instance.prefabs)
            {
                if (data.Prefab is T t)
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Retrieves the first prefab with this ID.
        /// Case insensitive.
        /// </summary>
        public static T Get<T>(ReadOnlySpan<char> id)
        {
            return Get<T>(new ID(id));
        }

        public static T Get<T>(int hashCode)
        {
            return Get<T>(new ID(hashCode));
        }

        public static T Get<T>(ID id)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefab in instance.prefabs)
            {
                if (prefab.ID == id && prefab.Prefab is T item)
                {
                    return item;
                }
            }

            throw ExceptionBuilder.Format("Could not find identifiable asset with id {0}", id);
        }

        /// <summary>
        /// Attempts to retrieve the first prefab with this ID.
        /// Case insensitive.
        /// </summary>
        public static bool TryGet<T>(ReadOnlySpan<char> id, [NotNullWhen(true)] out T? prefab)
        {
            return TryGet(new ID(id), out prefab);
        }

        public static bool TryGet<T>(ID id, [NotNullWhen(true)] out T? prefab)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefabData in instance.prefabs)
            {
                if (prefabData.ID == id && prefabData.Prefab is T asset)
                {
                    prefab = asset;
                    return true;
                }
            }

            prefab = default;
            return false;
        }

        [Serializable]
        public class PrefabData
        {
            [SerializeField, FixedString]
            private string id;

            [SerializeField]
            private Object? originalPrefab;

            public ID ID => new ID(id);

            public object? Prefab => GetPrefab(originalPrefab);
            public object? OriginalPrefab => originalPrefab;

            public PrefabData(ID id, object originalPrefab)
            {
                this.id = id.ToString();
                this.originalPrefab = originalPrefab as Object;
            }

            public static object? GetPrefab<T>(T prefab)
            {
                if (prefab is ItemAsset itemAsset)
                {
                    return itemAsset.PrefabItem;
                }
                else
                {
                    return prefab;
                }
            }
        }
    }
}
