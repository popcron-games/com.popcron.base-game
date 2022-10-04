#nullable enable
using Unity.Netcode;
using System.Collections.Generic;

namespace BaseGame
{
    public readonly struct SpawnInfo : INetworkSerializeByMemcpy, IPlayerSpawnInfo
    {
        public readonly FixedString playerPrefabName;
        public readonly FixedString behaviourTypeName;
        public readonly ID characterPrefabId;
        public readonly ID[] itemPrefabIds;

        FixedString IPlayerSpawnInfo.PlayerPrefabName => playerPrefabName;
        FixedString IPlayerSpawnInfo.BehaviourTypeName => behaviourTypeName;
        ID IPlayerSpawnInfo.CharacterPrefabID => characterPrefabId;
        IEnumerable<ID> IPlayerSpawnInfo.ItemPrefabIDs => itemPrefabIds;

        public SpawnInfo(FixedString playerPrefabName, FixedString behaviourTypeName, ID characterPrefabId, ID[] itemPrefabs)
        {
            this.playerPrefabName = playerPrefabName;
            this.behaviourTypeName = behaviourTypeName;
            this.characterPrefabId = characterPrefabId;
            this.itemPrefabIds = itemPrefabs;
        }
    }
}