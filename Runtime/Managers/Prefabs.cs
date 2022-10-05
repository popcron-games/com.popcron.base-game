#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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

        async UniTask IManager.Initialize(CancellationToken cancellationToken)
        {
            await Addressables.LoadAssetsAsync<Object>("prefabs", (prefab) =>
            {
                Add(prefab);
            }).ToUniTask().AttachExternalCancellation(cancellationToken).SuppressCancellationThrow();

            if (cancellationToken.IsCancellationRequested) return;
        }

        public static bool HasPrefab(ID prefabId)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefab in instance.prefabs)
            {
                if (prefab.ID == prefabId)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasPrefab<T>(ID prefabId)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefab in instance.prefabs)
            {
                if (prefab.ID == prefabId && prefab.Prefab is T)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Add<T>(T prefab)
        {
            ID id;
            if (prefab is IIdentifiable identifiable)
            {
                id = identifiable.ID;
            }
            else if (prefab is Object unityPrefab)
            {
                id = unityPrefab.name;
            }
            else
            {
                throw ExceptionBuilder.Format("Prefab {0} does not have an ID", prefab);
            }

            //check if already exists
            object? prefabToRead = PrefabData.GetPrefab(prefab);
            Prefabs instance = Instance;
            foreach (PrefabData data in instance.prefabs)
            {
                if (data.ID == id && data.Prefab == prefabToRead)
                {
                    log.LogWarningFormat("Prefab {0} already exists", id);
                    return;
                }
            }

            instance.prefabs.Add(new PrefabData(id, prefab));
            log.LogInfoFormat("Added prefab {0} with ID {1}", prefab, id);
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
        public static T Get<T>(ReadOnlySpan<char> prefabId)
        {
            return Get<T>(new ID(prefabId));
        }

        public static T Get<T>(int prefabIdHashCode)
        {
            return Get<T>(new ID(prefabIdHashCode));
        }

        public static T Get<T>(ID prefabId)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefab in instance.prefabs)
            {
                if (prefab.ID == prefabId && prefab.Prefab is T item)
                {
                    return item;
                }
            }

            throw ExceptionBuilder.Format("Could not find {0} asset with ID {1}", typeof(T), prefabId);
        }

        /// <summary>
        /// Attempts to retrieve the first prefab with this ID.
        /// Case insensitive.
        /// </summary>
        public static bool TryGet<T>(ReadOnlySpan<char> prefabId, [NotNullWhen(true)] out T? prefab)
        {
            return TryGet(new ID(prefabId), out prefab);
        }

        public static bool TryGet<T>(ID prefabId, [NotNullWhen(true)] out T? prefab)
        {
            Prefabs instance = Instance;
            foreach (PrefabData prefabData in instance.prefabs)
            {
                if (prefabData.ID == prefabId && prefabData.Prefab is T asset)
                {
                    prefab = asset;
                    return true;
                }
            }

            prefab = default;
            return false;
        }

        public static T Create<T>(ID prefabId)
        {
            T prefab = Get<T>(prefabId);
            if (prefab is IItem item)
            {
                return (T)item.Clone();
            }
            else if (prefab is ScriptableObject soPrefab)
            {
                string json = JsonUtility.ToJson(soPrefab);
                ScriptableObject clone = ScriptableObject.CreateInstance(soPrefab.GetType());
                JsonUtility.FromJsonOverwrite(json, clone);
                return (T)(object)clone;
            }
            else if (prefab is GameObject goPrefab)
            {
                return (T)(object)Instantiate(goPrefab);
            }
            else if (prefab is Component componentPrefab)
            {
                return (T)(object)Instantiate(componentPrefab);
            }
            else
            {
                throw ExceptionBuilder.Format("Creating an instance of {0} is not implemented", prefab);
            }
        }

        public static bool TryCreate<T>(ID prefabId, [NotNullWhen(true)] out T? instance)
        {
            try
            {
                instance = Create<T>(prefabId);
                return true;
            }
            catch (Exception)
            {
                instance = default;
                return false;
            }
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
