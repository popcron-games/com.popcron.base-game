#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGame
{
    [CreateAssetMenu]
    public class PlayerSpawnInfo : IdentifiableAsset
    {
        [SerializeField]
        private ItemAsset? characterAsset;

        [SerializeField]
        private GameObject? playerPrefab;

        [SerializeField, TypeFilter(assignableFrom: typeof(UserBehaviour))]
        private SerializedType behaviourType;

        [SerializeField]
        private List<ItemAsset> itemsToLoad = new();

        public SpawnInfo GetSpawnInfo()
        {
            ID characterPrefabId = ID.Empty;
            if (characterAsset && characterAsset is not null)
            {
                characterPrefabId = characterAsset.ID;
                Items.AddPrefab(characterAsset);
            }
            else
            {
                Log.LogErrorFormat("No character asset assigned to {0}", this);
            }

            List<ID> itemPrefabIds = new();
            for (int i = 0; i < itemsToLoad.Count; i++)
            {
                ItemAsset itemAsset = itemsToLoad[i];
                IItem prefabItem = itemAsset.PrefabItem;
                if (prefabItem is not null)
                {
                    if (prefabItem.ID == ID.Empty)
                    {
                        Log.LogErrorFormat("Item {0} at index {1} does not have an ID", itemAsset, i);
                    }
                    else
                    {
                        itemPrefabIds.Add(prefabItem.ID);
                        Items.AddPrefab(itemAsset);
                    }
                }
                else
                {
                    Log.LogErrorFormat("No item assigned to {0}", itemAsset);
                }
            }

            FixedString playerPrefabName = playerPrefab?.name ?? FixedString.Empty;
            FixedString behaviourTypeName = FixedString.Empty;
            if (behaviourType.Type is Type type)
            {
                behaviourTypeName = type.Name;
            }

            return new SpawnInfo(playerPrefabName, behaviourTypeName, characterPrefabId, itemPrefabIds.ToArray());
        }
    }
}