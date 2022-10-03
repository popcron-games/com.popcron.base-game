#nullable enable
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame.Managers
{
    public class MultiplayerManager : MonoBehaviour, ISingletonManager, IValidate
    {
        [SerializeField, HideInInspector]
        private NetworkManager? networkManager;

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

        UniTask IManager.Initialize()
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

        public static User CreateConnection(ulong clientId)
        {
            GameObject userPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
            User user = Instantiate(userPrefab).GetComponent<User>();
            user.NetworkObject.SpawnWithOwnership(clientId);
            return user;
        }

        public static void SetUserPrefab(User? userPrefab)
        {
            if (userPrefab != null)
            {
                NetworkManager.Singleton.NetworkConfig.PlayerPrefab = userPrefab.gameObject;
                NetworkManager.Singleton.AddNetworkPrefab(userPrefab.gameObject);
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
