#nullable enable
using System.Collections.Generic;

namespace BaseGame
{
    public interface IPlayerSpawnInfo
    {
        FixedString PlayerPrefabName { get; }
        FixedString BehaviourTypeName { get; }
        ID CharacterPrefabID { get; }
        IEnumerable<ID> ItemPrefabIDs { get; }
    }
}