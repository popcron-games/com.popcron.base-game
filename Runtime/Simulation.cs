#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System;
using System.Threading;

namespace BaseGame
{
    [CreateAssetMenu(menuName = "Base Game/Simulation")]
    public class Simulation : IdentifiableAsset, ISimulation
    {
        public readonly List<IComponent> components = new();
        public readonly List<IUpdateLoop> updateLoopComponents = new();
        
        private User? myUser;

        async UniTask ISimulation.Initialize(CancellationToken cancellationToken)
        {
            OnInitialize();

            await UniTask.Yield(cancellationToken).SuppressCancellationThrow();
            if (cancellationToken.IsCancellationRequested) return;

            await InitializeManagers(cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            Log.LogInfo("Waiting for my user to spawn...");
            User? myUser;
            while (true) //safe
            {
                myUser = User.MyUser;
                if (myUser is not null) break;

                await UniTask.Yield(cancellationToken).SuppressCancellationThrow();
                if (cancellationToken.IsCancellationRequested) return;
            }

            //thank you cysharp/unitask <3
            try
            {
                await OnInitialized(myUser);
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
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

        private async UniTask InitializeManagers(CancellationToken cancellationToken)
        {
            List<IManager> managers = new();

            //get existing managers
            managers.AddRange(GetAll<IManager>());

            //create singleton managers
            await CreateSingletonManagers(managers, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;

            foreach (IManager manager in TypeWithDependencies.Sort(managers))
            {
                await manager.Initialize(cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
                
                new ManagerHasInitialized(manager).Dispatch();
                Log.LogInfoFormat("Initialized singleton manager {0}", manager);
            }
        }
        
        private async UniTask CreateSingletonManagers(List<IManager> managers, CancellationToken cancellationToken)
        {
            await Addressables.LoadAssetsAsync<GameObject>("managers", (prefab) =>
            {
                if (prefab.TryGetComponent(out IManager manager))
                {
                    //add if it doesnt exist yet
                    if (managers.FindIndex(x => x.GetType() == manager.GetType()) == -1)
                    {
                        managers.Add(Instantiate(prefab).GetComponent<IManager>());
                    }
                }
            }).ToUniTask().AttachExternalCancellation(cancellationToken).SuppressCancellationThrow();
        }

        protected virtual void OnInitialize() { }
        protected virtual UniTask OnInitialized(User myUser) => UniTask.CompletedTask;

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