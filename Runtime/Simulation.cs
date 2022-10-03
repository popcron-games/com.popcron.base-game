#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BaseGame
{
    public abstract class Simulation
    {
        public readonly List<IComponent> components = new();
        public readonly List<IUpdateLoop> updateLoopComponents = new();

        private Log? log;
        private User? myUser;

        protected Log Log
        {
            get
            {
                if (log is null)
                {
                    log = new Log(GetType().Name);
                }

                return log;
            }
        }

        public async UniTask Initialize()
        {
            await InitializeManagers();

            Log.LogInfo("Finished initializing simulation");
            await UniTask.WaitUntil(() => myUser is not null);
            await OnInitialized(myUser!);
        }

        public virtual void OnQuitting() { }
        protected abstract UniTask OnInitialized(User myUser);

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

        public void Update()
        {
            float delta = Time.deltaTime;
            for (int i = updateLoopComponents.Count - 1; i >= 0; i--)
            {
                IUpdateLoop component = updateLoopComponents[i];
                component.OnUpdate(delta);
            }

            OnUpdate(delta);
        }

        protected virtual void OnUpdate(float delta) { }
    }
}