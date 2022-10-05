#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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

        public void Initialize(User user, IPlayerSpawnInfo spawnInfo, Spawnpoint? spawnpoint = null)
        {
            this.user = user;

            //player visuals if character is given
            if (Prefabs.HasPrefab<Character>(spawnInfo.CharacterPrefabID))
            {
                PlayerVisuals visualsAbility = new(spawnInfo.CharacterPrefabID, ID.CreateRandom(), spawnpoint);
                inventory.Add(visualsAbility);
            }
            else if (spawnInfo.CharacterPrefabID.IsEmpty)
            {
                throw ExceptionBuilder.Format("Character prefab is empty on player '{0}'.", ID);
            }
            else
            {
                throw ExceptionBuilder.Format("Character prefab '{0}' not found.", spawnInfo.CharacterPrefabID);
            }

            //add items
            int i = 0;
            foreach (ID itemPrefabId in spawnInfo.ItemPrefabIDs)
            {
                if (Prefabs.TryCreate(itemPrefabId, out IItem? newItem))
                {
                    inventory.Add(newItem);
                }
                else if (itemPrefabId.IsEmpty)
                {
                    Log.LogErrorFormat("No item prefab ID assigned to {0} at index {1}", this, i);
                }
                else
                {
                    Log.LogErrorFormat("Could not add item with ID {0} at index {1} because the prefab doesnt exist", itemPrefabId, i);
                }

                i++;
            }
        }

        protected override void OnStart()
        {
            foreach (ItemAsset itemAsset in itemsToLoad)
            {
                IItem newItem = itemAsset.Create();
                inventory.Add(newItem);
            }
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