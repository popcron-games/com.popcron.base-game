#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseGame
{
    [Serializable]
    public class PlayerVisuals : Ability, IUpdateLoop
    {
        [SerializeField, FixedString]
        private int characterPrefabId;

        private CharacterVisuals? visuals;
        private Spawnpoint? spawnpoint;

        public CharacterVisuals Visuals
        {
            get
            {
                if (visuals is null || !visuals)
                {
                    visuals = CreateVisuals();
                }

                return visuals;
            }
        }

        public Vector3 Position
        {
            get => Visuals.transform.position;
            set => Visuals.transform.position = value;
        }

        public PlayerVisuals(ID characterPrefabId, ID id, Spawnpoint? spawnpoint = null) : base(id)
        {
            this.characterPrefabId = characterPrefabId.GetHashCode();
            this.spawnpoint = spawnpoint;
        }

        public bool TryGetVisuals([NotNullWhen(true)] out CharacterVisuals? visuals)
        {
            visuals = this.visuals;
            return visuals is not null;
        }

        protected override void RemovedFrom(IPlayer player)
        {
            if (visuals != null)
            {
                Object.Destroy(visuals);
                visuals = null;
            }
        }

        void IUpdateLoop.OnUpdate(float delta)
        {
            if (visuals is null)
            {
                CreateVisuals();
            }
        }

        private CharacterVisuals CreateVisuals()
        {
            CharacterVisuals prefab = Prefabs.Get<CharacterVisuals>(characterPrefabId);
            Vector3 position = spawnpoint?.Position ?? default;
            Quaternion rotation = spawnpoint?.Rotation ?? default;
            if (Player is Component unityComponent)
            {
                visuals = Object.Instantiate(prefab, unityComponent.transform, false);
                visuals.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                visuals = Object.Instantiate(prefab, position, rotation);
            }

            visuals.Initialize(Player, spawnpoint);
            visuals.NetworkObject.SpawnWithOwnership(Player.OwnerClientId);
            return visuals;
        }
    }
}