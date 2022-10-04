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
        private List<Prefab> prefabs = new();

        async UniTask IManager.Initialize()
        {
            await Addressables.LoadAssetsAsync<Object>("prefabs", (prefab) =>
            {
                FixedString key = prefab.name;
                prefabs.Add(new Prefab(key, prefab));
            }).ToUniTask();
        }

        public static void Add<T>(FixedString name, T prefab) where T : Object
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Key == name && prefabData.Asset is T)
                {
                    return;
                }
            }
            
            Instance.prefabs.Add(new Prefab(name, prefab));
        }

        public static void Add<T>(T asset) where T : IdentifiableAsset
        {
            FixedString key = asset.ID.ToString();
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Key == key && prefabData.Asset is T)
                {
                    return;
                }
            }
            
            instance.prefabs.Add(new Prefab(key, asset));
        }

        public static GameObject GetGameObjectPrefab(FixedString prefabKey)
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Key == prefabKey && prefabData.Asset is GameObject)
                {
                    return (GameObject)prefabData.Asset;
                }
            }

            throw ExceptionBuilder.Format("Prefab {0} not found", prefabKey);
        }

        public static bool TryGetGameObjectPrefab(FixedString prefabKey, [MaybeNullWhen(false)] out GameObject prefab)
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Key == prefabKey && prefabData.Asset is GameObject gameObject)
                {
                    prefab = gameObject;
                    return gameObject != null;
                }
            }

            prefab = default;
            return false;
        }

        public static T GetIdentifiableAsset<T>(ReadOnlySpan<char> id) where T : IdentifiableAsset
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Asset is T identifiableAsset && identifiableAsset.ID == id)
                {
                    return identifiableAsset;
                }
            }

            throw ExceptionBuilder.Format("Could not find identifiable asset with id {0}", id);
        }

        public static bool TryGetIdentifiableAsset<T>(ReadOnlySpan<char> id, [MaybeNullWhen(false)] out T identifiableAsset) where T : IdentifiableAsset
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Asset is T identifiable && identifiable.ID == id)
                {
                    identifiableAsset = identifiable;
                    return true;
                }
            }

            identifiableAsset = default;
            return false;
        }

        public static IEnumerable<T> GetAll<T>()
        {
            Prefabs instance = Instance;
            foreach (Prefab prefabData in instance.prefabs)
            {
                if (prefabData.Asset is T t)
                {
                    yield return t;
                }
            }
        }

        [Serializable]
        public class Prefab
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private Object asset;

            public FixedString Key => name;
            public Object Asset => asset;

            public Prefab(FixedString key, Object asset)
            {
                name = key;
                this.asset = asset;
            }
        }
    }
}
