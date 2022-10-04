#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame
{
    public class User : NetworkBehaviour, IIdentifiable
    {
        private static User? myUser;

        public static User? MyUser
        {
            get
            {
                if (!myUser || myUser is null)
                {
                    Connection? myConnection = Connection.LocalPlayer;
                    if (myConnection is not null)
                    {
                        myUser = myConnection.User;
                    }
                }

                return myUser;
            }
        }

        [SerializeField]
        private Player? player;

        private int userId;
        private SpawnInfo spawnInfo;
        private UserBehaviour? userBehaviour;
        private Log? log;

        public bool IsAlive
        {
            get
            {
                if (player is null || !player)
                {
                    return false;
                }

                return true;
            }
        }

        public ID ID => new ID(userId);
        public IPlayer? Player => player;

        protected Log Log
        {
            get
            {
                if (log is null)
                {
                    log = new Log(name);
                }

                return log;
            }
        }

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

        public void Initialize()
        {
            userId = ID.CreateRandom().GetHashCode();
        }

        private void OnEnable()
        {
            PlayerLoop.Add(this);
        }

        private void OnDisable()
        {
            PlayerLoop.Remove(this);
        }

        private void Update()
        {
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
            FixedString playerPrefabName = spawnInfo.playerPrefabName;
            if (playerPrefabName == FixedString.Empty)
            {
                Log.LogWarningFormat("No player prefab name given for user '{0}', using Player as fallback", ID);
                playerPrefabName = "Player";
            }
            
            if (Prefabs.TryGetGameObjectPrefab(playerPrefabName, out GameObject? prefab))
            {
                Spawnpoint? spawnpoint = GetSpawnpoint();
                Player player = Instantiate(prefab, transform).GetComponent<Player>();
                player.Initialize(this, spawnInfo, spawnpoint);
                return player;
            }
            else
            {
                Log.LogErrorFormat("Could not find prefab for {0}", playerPrefabName);
                return null!;
            }
        }

        public InputState GetInputState(ReadOnlySpan<char> name)
        {
            if (TryGetInputState(name, out InputState state))
            {
                return state;
            }

            throw ExceptionBuilder.Format("No input state with name {0} found", name);
        }

        public bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState inputState)
        {
            if (userBehaviour is not null && userBehaviour.TryGetInputState(name, out inputState))
            {
                return true;
            }

            inputState = InputState.None;
            return false;
        }

        public void SetSpawnInfo(SpawnInfo spawnInfo)
        {
            this.spawnInfo = spawnInfo;
            Type behaviourType = TypeCache.GetType(spawnInfo.behaviourTypeName);
            userBehaviour = UserBehaviour.Create(behaviourType, this);
        }

        public void MakePlayerAlive()
        {
            if (player is null || !player)
            {
                player = SpawnPlayer();
            }
        }

        public void MakePlayerDead()
        {
            if (player is not null && player)
            {
                player.DestroySelf();
            }

            player = null;
        }
    }
}