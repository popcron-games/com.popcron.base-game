#nullable enable
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame.Managers
{
    public class PlayerManager : SingletonComponent<PlayerManager>, ISingletonManager
    {
        public static User UserPrefab
        {
            get
            {
                PlayerManager instance = Instance;
                if (instance.customUser is not null && instance.customUser)
                {
                    return instance.customUser;
                }

                return instance.user;
            }
        }

        [SerializeField, MustBeAssigned]
        private User user = null!;

        private User? customUser;

        public static void SetUserPrefab(User? userPrefab)
        {
            Instance.customUser = userPrefab;
            log.LogInfoFormat("Set user prefab to {0}", userPrefab);
        }

        public static User CreateUser(ulong? ownerClientId = null)
        {
            User prefab = UserPrefab;
            User user = Instantiate(prefab);
            user.Initialize();

            if (ownerClientId is not null)
            {
                user.name = ValueStringBuilder.Format("User {0}:{1}", user.ID, ownerClientId.Value).ToString();
            }
            else
            {
                user.name = ValueStringBuilder.Format("User {0}", user.ID).ToString();
            }

            NetworkObject networkObject = user.GetComponent<NetworkObject>();
            if (networkObject)
            {
                if (ownerClientId is null)
                {
                    networkObject.Spawn(true);
                }
                else
                {
                    networkObject.SpawnWithOwnership(ownerClientId.Value, true);
                }
            }

            log.LogInfoFormat("Created user {0}", user.ID);
            return user;
        }

        UniTask IManager.Initialize()
        {
            return UniTask.CompletedTask;
        }
    }
}
