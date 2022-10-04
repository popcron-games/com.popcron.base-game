#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

namespace BaseGame
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField]
        private List<ItemAsset> itemsToLoad = new();

        [SerializeField]
        private Inventory inventory = new();

        private User? user;

        public ID ID => User.ID;
        public ulong OwnerClientId => User.OwnerClientId;
        public IInventory Inventory => inventory;
        public User User
        {
            get
            {
                if (user is null || !user)
                {
                    foreach (var u in PlayerLoop.GetAll<User>())
                    {
                        if (u.Player == (IPlayer)this)
                        {
                            user = u;
                            break;
                        }
                    }

                    throw ExceptionBuilder.Format("User not found for player '{0}'", ID);
                }

                return user;
            }
        }

        public override string ToString()
        {
            return ValueStringBuilder.Format("Player {0}", ID).ToString();
        }

        public void Initialize(User user, SpawnInfo spawnInfo, Spawnpoint? spawnpoint = null)
        {
            this.user = user;

            //player visuals if character is given
            if (Items.TryCreate(spawnInfo.characterPrefabId, out Character? newCharacter))
            {
                PlayerVisuals visualsAbility = new(newCharacter, ID.CreateRandom(), spawnpoint);
                AddAbility(visualsAbility);
            }
            else if (spawnInfo.characterPrefabId != ID.Empty)
            {
                throw ExceptionBuilder.Format("Character prefab '{0}' not found.", spawnInfo.characterPrefabId);
            }
            else
            {
                throw ExceptionBuilder.Format("Character prefab is empty on player '{0}'.", ID);
            }

            //add items
            for (int i = 0; i < spawnInfo.itemPrefabIds.Length; i++)
            {
                ref ID itemPrefabId = ref spawnInfo.itemPrefabIds[i];
                if (Items.TryCreate(itemPrefabId, out IItem? item))
                {
                    inventory.Add(item);
                }
                else if (itemPrefabId == ID.Empty)
                {
                    Log.LogErrorFormat("No item prefab ID assigned to {0} at index {1}", this, i);
                }
                else
                {
                    Log.LogErrorFormat("Could not add item from prefab with ID {0}", itemPrefabId);
                }
            }
        }

        protected override void OnStart()
        {
            inventory.AddRange(itemsToLoad);
        }

        protected override void OnEnabled()
        {
            inventory.Enabled();
        }

        protected override void OnDisabled()
        {
            inventory.Disabled();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void MarkAsDead()
        {
            if (user is not null)
            {
                user.MakePlayerDead();
            }
            else
            {
                DestroySelf();
            }
        }

        public IAbility? GetAbility(ID id) => inventory.Get<IAbility>(id);
        public T GetFirstAbility<T>() => inventory.GetFirst<T>() ?? throw ExceptionBuilder.Format("No ability of type {0} found", typeof(T).Name);
        public bool HasAbility<T>() => inventory.Contains<T>();

        public bool AddAbility<T>(T ability) where T : IAbility
        {
            if (HasAbility<T>())
            {
                return false;
            }

            inventory.Add(ability);
            return true;
        }

        public InputState GetInputState(ReadOnlySpan<char> name)
        {
            if (user is null)
            {
                return InputState.None;
            }

            return user.GetInputState(name);
        }

        public bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState state)
        {
            if (user is null)
            {
                state = InputState.None;
                return false;
            }

            return user.TryGetInputState(name, out state);
        }
    }
}