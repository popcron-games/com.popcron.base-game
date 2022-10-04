#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace BaseGame
{
    public abstract class Simulation : IdentifiableAsset, ISimulation
    {
        public readonly List<IComponent> components = new();
        public readonly List<IUpdateLoop> updateLoopComponents = new();
        private User? myUser;
        
        async UniTask ISimulation.Initialize()
        {
            Log.LogInfo("Initializing simulation...");

            OnInitialize();
            await UniTask.Yield();

            Log.LogInfo("Initializing managers...");
            await InitializeManagers();

            await UniTask.WaitUntil(() => myUser is not null);
            await OnInitialized(myUser!);
        }

        void ISimulation.Update()
        {
            float delta = Time.deltaTime;
            for (int i = updateLoopComponents.Count - 1; i >= 0; i--)
            {
                IUpdateLoop component = updateLoopComponents[i];
                component.OnUpdate(delta);
            }
        }

        private async UniTask InitializeManagers()
        {
            List<IManager> managers = new();

            //get existing managers
            managers.AddRange(GetAll<IManager>());

            //create singleton managers
            await CreateSingletonManagers(managers);

            foreach (IManager manager in TypeWithDependencies.Sort(managers))
            {
                await manager.Initialize();
                new ManagerHasInitialized(manager).Dispatch();
                Log.LogInfoFormat("Initialized singleton manager {0}", manager);
            }
        }
        
        private async UniTask CreateSingletonManagers(List<IManager> managers)
        {
            await Addressables.LoadAssetsAsync<GameObject>("managers", (prefab) =>
            {
                if (prefab.TryGetComponent(out ISingletonManager manager))
                {
                    if (managers.FindIndex(x => x.GetType() == manager.GetType()) == -1)
                    {
                        managers.Add(GameObject.Instantiate(prefab).GetComponent<IManager>());
                    }
                }
                else
                {
                    Log.LogErrorFormat("Prefab {0} does not have a {1} component", prefab, nameof(ISingletonManager));
                }
            }).ToUniTask();
        }

        protected virtual void OnInitialize() { }

        protected abstract UniTask OnInitialized(User myUser);

        public void Add<T>(T obj)
        {
            if (obj is IComponent component)
            {
                components.Add(component);
            }

            if (obj is IUpdateLoop updateLoop)
            {
                updateLoopComponents.Add(updateLoop);
            }

            if (myUser is null && obj is User user)
            {
                myUser = user;
            }
        }

        public void Remove<T>(T obj)
        {
            if (obj is IComponent component)
            {
                components.Remove(component);
            }

            if (obj is IUpdateLoop updateLoop)
            {
                updateLoopComponents.Remove(updateLoop);
            }
        }

        public IReadOnlyList<IComponent> GetAll()
        {
            return components;
        }

        public IEnumerable<T> GetAll<T>()
        {
            foreach (IComponent obj in components)
            {
                if (obj is T component)
                {
                    yield return component;
                }
            }
        }

        public T? GetFirst<T>()
        {
            foreach (IComponent obj in components)
            {
                if (obj is T component)
                {
                    return component;
                }
            }

            return default;
        }

        /// <summary>
        /// Returns an instance that can be identified with this ID.
        /// </summary>
        public T? Get<T>(ID id)
        {
            foreach (IComponent obj in components)
            {
                if (obj is IIdentifiable identifiable && identifiable.ID == id)
                {
                    if (obj is T component)
                    {
                        return component;
                    }
                }
            }

            return default;
        }
    }
}