#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class Character : Item
    {
        [SerializeField]
        private string? displayName;

        [SerializeField, MustBeAssigned]
        private CharacterVisuals? visuals;

        public FixedString DisplayName => displayName ?? FixedString.Empty;
        public CharacterVisuals VisualsPrefab => visuals ?? throw ExceptionBuilder.Format("Character {0} does not have a {1} prefab assigned", this, typeof(CharacterVisuals));

        public Character(ID id) : base(id) { }
    }
}