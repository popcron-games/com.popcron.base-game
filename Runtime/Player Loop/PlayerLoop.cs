#nullable enable
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.LowLevel;
using static UnityEngine.PlayerLoop.Update;

namespace BaseGame
{
    public static class PlayerLoop
    {
        public static readonly Log Log = new("Player Loop");

        private static Simulation? simulation;
        private static bool isReady;

        public static Simulation Simulation
        {
            get
            {
                if (simulation is null)
                {
                    simulation = LoadSimulation();
                }

                return simulation;
            }
        }

        static PlayerLoop()
        {
            Application.quitting += OnQuitting;
            InjectPlayerLoop();
        }

        public static Simulation? GetSimulation() => simulation;

        private static void InjectPlayerLoop()
        {
            PlayerLoopSystem current = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopExtensions.InjectAfter<ScriptRunBehaviourUpdate>(ref current, Update, typeof(Simulation));
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(current);
        }

        private static void OnQuitting()
        {
            if (simulation is not null)
            {
                simulation.OnQuitting();
                simulation = null;
            }
        }

        private static void Update()
        {
            if (isReady && Application.isPlaying)
            {
                Simulation.Update();
            }
        }

        private static Simulation LoadSimulation()
        {
            PlayerLoopSettings settings = PlayerLoopSettings.Instance;
            if (settings.SimulationType is Type simulationType)
            {
                Simulation simulation = (Simulation)Activator.CreateInstance(simulationType);
                Log.LogInfoFormat("Using simulation {0}", simulation);
                return simulation;
            }
            else
            {
                throw ExceptionBuilder.Format("No simulation type set in {0}", settings);
            }
        }

        public static void Add<T>(T obj) => Simulation.Add(obj);
        public static void Remove<T>(T obj) => Simulation.Remove(obj);
        public static IReadOnlyList<IComponent> GetAll() => Simulation.GetAll();
        public static IEnumerable<T> GetAll<T>() => Simulation.GetAll<T>();
        public static T? GetFirst<T>() => Simulation.GetFirst<T>();
        public static T? Get<T>(ID id) => Simulation.Get<T>(id);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Start()
        {
            isReady = false;
            _ = BeginAsync();
        }

        private static async UniTask BeginAsync()
        {
            await InitializeAddressables();
            await Simulation.Initialize();

            isReady = true;
            new GameHasInitialized().Dispatch();
        }

        private static async UniTask InitializeAddressables()
        {
            await Addressables.InitializeAsync().ToUniTask();
        }

        private static void OnInitialized(GameHasInitialized testEvent)
        {
            Debug.Log("Ready");
        }

        public readonly struct OnGameInit : IStaticListener<GameHasInitialized>
        {
            void IStaticListener<GameHasInitialized>.OnEvent(GameHasInitialized e)
            {
                OnInitialized(e);
            }
        }
    }
}