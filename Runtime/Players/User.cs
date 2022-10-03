#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame
{
    public class User : NetworkBehaviour, IComponent, IIdentifiable
    {
        private static User? myUser;

        public static User? MyUser
        {
            get
            {
                if (!myUser || myUser is null)
                {
                    foreach (User user in PlayerLoop.GetAll<User>())
                    {
                        if (user.IsLocalPlayer)
                        {
                            myUser = user;
                            break;
                        }
                    }
                }

                return myUser;
            }
        }

        [SerializeField]
        private NetworkVariable<bool> isAlive = new();

        [SerializeField]
        private Player? player;

        private Character? character;

        private Log? log;
        private SpawnInfo spawnInfo;
        private bool lastIsAlive;
        private UserBehaviour? userBehaviour;

        public bool IsAlive => isAlive.Value;
        public ID ID => new ID((int)NetworkObjectId);
        public IPlayer? Player => player;

        public UserBehaviour Behaviour
        {
            get
            {
                if (userBehaviour is null)
                {
                    Type behaviourType = TypeCache.GetType(spawnInfo.behaviourTypeName);
                    userBehaviour = UserBehaviour.Create(behaviourType, this);
                }

                return userBehaviour;
            }
        }

        protected Log Log
        {
            get
            {
                if (log is null)
                {
                    log = new Log(ValueStringBuilder.Format("User {0}", OwnerClientId));
                }

                return log;
            }
        }

        private void OnEnable()
        {
            PlayerLoop.Add(this);
        }

        private void OnDisable()
        {
            PlayerLoop.Remove(this);
        }

        private void OnApplicationQuit()
        {
            isAlive.SetFieldValue("m_InternalValue", false);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                isAlive.Value = false;
            }
        }

        private void Update()
        {
            if (lastIsAlive != isAlive.Value)
            {
                lastIsAlive = isAlive.Value;
                if (lastIsAlive)
                {
                    player = SpawnPlayer();
                }
                else
                {
                    if (player is not null && player)
                    {
                        player.DestroySelf();
                        player = null;
                    }
                }
            }

            if (userBehaviour is IUpdateLoop update)
            {
                update.OnUpdate(Time.deltaTime);
            }
        }

        protected virtual Spawnpoint? GetSpawnpoint()
        {
            foreach (Spawnpoint spawnpoint in PlayerLoop.GetAll<Spawnpoint>())
            {
                if (spawnpoint.CanUseSpawnpoint(this))
                {
                    return spawnpoint;
                }
            }

            return null;
        }

        private Player SpawnPlayer()
        {
            if (Prefabs.TryGetGameObjectPrefab(spawnInfo.playerPrefabName, out GameObject? prefab))
            {
                Spawnpoint? spawnpoint = GetSpawnpoint();
                Player player = Instantiate(prefab, transform).GetComponent<Player>();
                player.Initialize(this, spawnInfo, spawnpoint);
                return player;
            }
            else
            {
                Log.LogErrorFormat("Could not find prefab for {0}", spawnInfo.playerPrefabName);
                return null!;
            }
        }

        public InputState GetInputState(ReadOnlySpan<char> name) => userBehaviour?.GetInputState(name) ?? InputState.None;

        public bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState inputState)
        {
            if (userBehaviour is not null && userBehaviour.TryGetInputState(name, out inputState))
            {
                return true;
            }

            inputState = InputState.None;
            return false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskToSpawnPlayerServerRpc(SpawnInfo spawnInfo)
        {
            this.spawnInfo = spawnInfo;
            isAlive.Value = true;

            Type behaviourType = TypeCache.GetType(spawnInfo.behaviourTypeName);
            userBehaviour = UserBehaviour.Create(behaviourType, this);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskToRespawnServerRpc()
        {
            isAlive.Value = true;
        }

        [ServerRpc(RequireOwnership = false)]
        public void MarkAsDeadServerRpc()
        {
            isAlive.Value = false;
        }
    }
}