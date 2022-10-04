#nullable enable
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.LowLevel;
using static UnityEngine.PlayerLoop.Update;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace BaseGame
{
    public static class PlayerLoop
    {
        public static readonly Log Log = new("Player Loop");

        private static ISimulation? simulation;
        private static bool isReady;

        public static ISimulation Simulation
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
            InjectPlayerLoop();
        }

        public static bool TryGetSimulation([NotNullWhen(true)] out ISimulation? simulation)
        {
            simulation = PlayerLoop.simulation;
            return simulation is not null;
        }

        private static void InjectPlayerLoop()
        {
            PlayerLoopSystem current = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopExtensions.InjectAfter<ScriptRunBehaviourUpdate>(ref current, Update, typeof(PlayerLoop));
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(current);
        }

        private static void Update()
        {
            if (isReady && Application.isPlaying)
            {
                Simulation.Update();
            }
        }

        private static ISimulation LoadSimulation()
        {
            PlayerLoopSettings settings = PlayerLoopSettings.Instance;
            Simulation simulationPrefab = settings.SimulationPrefab;

            //perform an identical copy in memory
            string json = JsonUtility.ToJson(simulationPrefab);
            object clone = ScriptableObject.CreateInstance(simulationPrefab.GetType());
            JsonUtility.FromJsonOverwrite(json, clone);
            return (ISimulation)clone;
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

        private static void GameInitialized()
        {
            Log.LogInfo("Game is ready");
        }

        public readonly struct OnGameInit : IStaticListener<GameHasInitialized>
        {
            void IStaticListener<GameHasInitialized>.OnEvent(GameHasInitialized e)
            {
                GameInitialized();
            }
        }
    }
}