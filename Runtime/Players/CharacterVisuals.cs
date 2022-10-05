#nullable enable
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame
{
    public class CharacterVisuals : NetworkBehaviour, IValidate
    {
        [SerializeField, HideInInspector]
        private Collider2D[] colliders2d = { };

        [SerializeField, HideInInspector]
        private Collider[] colliders = { };

        [SerializeField, HideInInspector]
        private Component? rb;

        public IEnumerable<Collider2D> Collider2Ds => colliders2d;
        public IEnumerable<Collider> Colliders => colliders;
        public Component? Rigidbody => rb;

        public Vector3 Position
        {
            get
            {
                if (rb is Rigidbody rigidbody)
                {
                    return rigidbody.position;
                }
                else if (rb is Rigidbody2D rigidbody2d)
                {
                    return rigidbody2d.position;
                }
                else
                {
                    return transform.position;
                }
            }
            set
            {
                if (rb is Rigidbody rigidbody)
                {
                    rigidbody.position = value;
                }
                else if (rb is Rigidbody2D rigidbody2d)
                {
                    rigidbody2d.position = value;
                }

                transform.position = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                if (rb is Rigidbody rigidbody)
                {
                    return rigidbody.velocity;
                }
                else if (rb is Rigidbody2D rigidbody2d)
                {
                    return rigidbody2d.velocity;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            set
            {
                if (rb is Rigidbody rigidbody)
                {
                    rigidbody.velocity = value;
                }
                else if (rb is Rigidbody2D rigidbody2d)
                {
                    rigidbody2d.velocity = value;
                }
            }
        }

        public Vector3 PhysicsGravity
        {
            get
            {
                if (rb is Rigidbody rigidbody)
                {
                    return rigidbody.useGravity ? Physics.gravity : Vector3.zero;
                }
                else if (rb is Rigidbody2D rigidbody2d)
                {
                    return Physics2D.gravity * rigidbody2d.gravityScale;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        private void OnValidate()
        {
            Validate();
        }

        protected virtual bool Validate()
        {
            bool changed = false;
            colliders = GetComponentsInChildren<Collider>();
            colliders2d = GetComponentsInChildren<Collider2D>();

            if (!rb)
            {
                rb = GetComponent<Rigidbody>();
                changed = rb != null;
            }

            if (!rb)
            {
                rb = GetComponent<Rigidbody2D>();
                changed = rb != null;
            }

            return changed;
        }

        public virtual void Initialize(IPlayer player, Spawnpoint? spawnpoint = null) { }

        bool IValidate.Validate()
        {
            return Validate();
        }
    }
}