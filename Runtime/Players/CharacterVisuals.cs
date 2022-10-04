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
        private Rigidbody? rb;

        [SerializeField, HideInInspector]
        private Rigidbody2D? rb2d;

        public IEnumerable<Collider2D> Collider2Ds => colliders2d;
        public IEnumerable<Collider> Colliders => colliders;
        public Rigidbody? Rigidbody => rb;
        public Rigidbody2D? Rigidbody2D => rb2d;

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
                changed = rb is not null;
            }

            if (!rb2d)
            {
                rb2d = GetComponent<Rigidbody2D>();
                changed = rb2d is not null;
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