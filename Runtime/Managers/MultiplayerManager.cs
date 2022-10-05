#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame.Managers
{
    public class MultiplayerManager : SingletonComponent<MultiplayerManager>, IManager, IValidate, IDependentUpon
    {
        [SerializeField, HideInInspector]
        private NetworkManager? networkManager;

        IEnumerable<Type> IDependentUpon.Dependencies
        {
            get
            {
                yield return typeof(PlayerManager);
            }
        }

        protected override void OnEnabled()
        {
            if (networkManager != null)
            {
                networkManager.ConnectionApprovalCallback += OnConnectionApproval;
            }
        }

        protected override void OnDisabled()
        {
            if (networkManager != null)
            {
                networkManager.ConnectionApprovalCallback -= OnConnectionApproval;
            }
        }

        private void OnApplicationQuit()
        {
            if (networkManager != null)
            {
                networkManager.Shutdown();
            }
        }

        private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        UniTask IManager.Initialize(CancellationToken cancellationToken)
        {
            if (networkManager != null)
            {
                networkManager.NetworkConfig.ForceSamePrefabs = false;
                networkManager.StartHost();
                Log.LogInfo("Started host");
            }
            else
            {
                Log.LogErrorFormat("A {0} is missing", typeof(NetworkManager));
            }

            return UniTask.CompletedTask;
        }

        bool IValidate.Validate()
        {
            bool changed = false;
            if (networkManager is null)
            {
                networkManager = GetComponent<NetworkManager>();
                changed = networkManager is not null;
            }

            return changed;
        }

        public static void SetUserPrefab(User? userPrefab)
        {
            if (userPrefab != null)
            {
                NetworkManager.Singleton.NetworkConfig.PlayerPrefab = userPrefab.gameObject;
                NetworkManager.Singleton.AddNetworkPrefab(userPrefab.gameObject);
                Instance.Log.LogInfoFormat("Set user prefab to {0}", userPrefab.name);
            }
        }

        public static void AddPrefab(GameObject? prefab)
        {
            if (prefab != null)
            {
                NetworkManager.Singleton.AddNetworkPrefab(prefab);
            }
        }
    }
}
