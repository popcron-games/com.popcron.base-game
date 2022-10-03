#nullable enable
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct SpawnInfo : INetworkSerializeByMemcpy
    {
        public readonly FixedString playerPrefabName;
        public readonly FixedString behaviourTypeName;
        public readonly ID characterPrefabId;
        public readonly ID[] itemPrefabIds;

        public SpawnInfo(FixedString playerPrefabName, FixedString behaviourTypeName, ID characterPrefabId, ID[] itemPrefabs)
        {
            this.playerPrefabName = playerPrefabName;
            this.behaviourTypeName = behaviourTypeName;
            this.characterPrefabId = characterPrefabId;
            this.itemPrefabIds = itemPrefabs;
        }
    }
}