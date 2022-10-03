#nullable enable
using System;

namespace BaseGame
{
    [Serializable]
    [ItemCategory(nameof(Ability))]
    public class Ability : PlayerItem, IAbility
    {
        public Ability(ID id) : base(id) { }
    }
}
