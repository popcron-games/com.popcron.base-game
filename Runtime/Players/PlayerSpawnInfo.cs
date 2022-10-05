#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGame
{
    [CreateAssetMenu]
    public class PlayerSpawnInfo : IdentifiableAsset, IPlayerSpawnInfo
    {
        [SerializeField, MustBeAssigned]
        private ItemAsset? characterAsset;

        [SerializeField]
        private GameObject? playerPrefab;

        [SerializeField, TypeFilter(assignableFrom: typeof(UserBehaviour))]
        private SerializedType behaviourType;

        [SerializeField]
        private List<ItemAsset> itemsToLoad = new();

        FixedString IPlayerSpawnInfo.PlayerPrefabName => playerPrefab?.name ?? FixedString.Empty;
        FixedString IPlayerSpawnInfo.BehaviourTypeName => behaviourType.Type.Name;
        ID IPlayerSpawnInfo.CharacterPrefabID
        {
            get
            {
                if (characterAsset != null)
                {
                    Prefabs.Add(characterAsset);
                    return characterAsset.ID;
                }
                else
                {
                    throw ExceptionBuilder.Format("Player spawn info {0} does not have a character asset assigned", this);
                }
            }
        }

        IEnumerable<ID> IPlayerSpawnInfo.ItemPrefabIDs
        {
            get
            {
                for (int i = 0; i < itemsToLoad.Count; i++)
                {
                    ItemAsset itemAsset = itemsToLoad[i];
                    IItem prefabItem = itemAsset.PrefabItem;
                    if (prefabItem is not null)
                    {
                        if (prefabItem.ID.IsEmpty)
                        {
                            Log.LogErrorFormat("Item {0} at index {1} does not have an ID", itemAsset, i);
                        }
                        else
                        {
                            Prefabs.Add(itemAsset);
                            yield return prefabItem.ID;
                        }
                    }
                    else
                    {
                        Log.LogErrorFormat("No item assigned to {0} at index {1}", itemAsset, i);
                    }
                }
            }
        }
    }
}