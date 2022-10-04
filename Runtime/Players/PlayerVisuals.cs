#nullable enable
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseGame
{
    [Serializable]
    public class PlayerVisuals : Ability, IUpdateLoop
    {
        [SerializeField, MustBeAssigned]
        private Character characterPrefab;

        private CharacterVisuals? visuals;
        private Spawnpoint? spawnpoint;

        public CharacterVisuals Visuals
        {
            get
            {
                if (visuals is null)
                {
                    CreateVisuals();
                }

                return visuals;
            }
        }

        public Vector3 Position
        {
            get => Visuals.transform.position;
            set => Visuals.transform.position = value;
        }

        public PlayerVisuals(Character characterPrefab, ID id, Spawnpoint? spawnpoint = null) : base(id)
        {
            this.characterPrefab = characterPrefab;
            this.spawnpoint = spawnpoint;
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
            CharacterVisuals prefab = characterPrefab.VisualsPrefab;
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