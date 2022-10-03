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
        private SerializedID id = new();

        [SerializeField]
        private List<ItemAsset> itemsToLoad = new();

        [SerializeField]
        private Inventory inventory = new();

        private User? user;

        public ID ID => id.ID;
        public ulong OwnerClientId => user?.OwnerClientId ?? ulong.MaxValue;
        public IInventory Inventory => inventory;

        public override string ToString()
        {
            return ValueStringBuilder.Format("Player {0}", ID).ToString();
        }

        public void Initialize(User user, SpawnInfo spawnInfo, Spawnpoint? spawnpoint = null)
        {
            this.user = user;
            id = ID.CreateRandom();

            //player visuals if character is given
            Character? character = Items.GetPrefab<Character>(spawnInfo.characterPrefabId);
            if (character is not null)
            {
                PlayerVisuals visualsAbility = new(character, ID.CreateRandom(), spawnpoint);
                AddAbility(visualsAbility);
            }

            //add items
            for (int i = 0; i < spawnInfo.itemPrefabIds.Length; i++)
            {
                ref ID itemPrefabId = ref spawnInfo.itemPrefabIds[i];
                if (Items.TryCreate(itemPrefabId, out IItem? item))
                {
                    inventory.Add(item);
                }
                else if (itemPrefabId == default)
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
                user.MarkAsDeadServerRpc();
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