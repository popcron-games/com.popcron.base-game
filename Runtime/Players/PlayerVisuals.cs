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

        private CharacterVisuals visuals = null!;
        private Spawnpoint? spawnpoint;

        public CharacterVisuals Visuals => visuals;

        public Vector3 Position
        {
            get => visuals.transform.position;
            set => visuals.transform.position = value;
        }

        public Vector3 Velocity
        {
            get
            {
                if (visuals.Rigidbody is Rigidbody rb)
                {
                    return rb.velocity;
                }
                else if (visuals.Rigidbody is Rigidbody2D rb2d)
                {
                    return rb2d.velocity;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            set
            {
                if (visuals.Rigidbody is Rigidbody rb)
                {
                    rb.velocity = value;
                }
                else if (visuals.Rigidbody is Rigidbody2D rb2d)
                {
                    rb2d.velocity = value;
                }
            }
        }

        public Vector3 PhysicsGravity
        {
            get
            {
                if (visuals.Rigidbody is Rigidbody rb)
                {
                    return rb.useGravity ? Physics.gravity : Vector3.zero;
                }
                else if (visuals.Rigidbody is Rigidbody2D rb2d)
                {
                    return Physics2D.gravity * rb2d.gravityScale;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        public PlayerVisuals(ID characterPrefabId, ID id, Spawnpoint? spawnpoint = null) : base(id)
        {
            this.characterPrefabId = characterPrefabId.GetHashCode();
            this.spawnpoint = spawnpoint;
        }

        public bool TryGetVisuals([NotNullWhen(true)] out CharacterVisuals? visuals)
        {
            visuals = this.visuals;
            return visuals != null;
        }

        protected override void AddedTo(IPlayer player)
        {
            visuals = CreateVisuals();
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
            Character character = Prefabs.Get<Character>(characterPrefabId);
            CharacterVisuals prefab = character.VisualsPrefab;
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