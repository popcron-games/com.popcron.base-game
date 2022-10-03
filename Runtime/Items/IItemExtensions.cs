#nullable enable

using UnityEngine;

namespace BaseGame
{
    public static class IItemExtensions
    {
        public static T Clone<T>(this T itemPrefab, ID? id = null) where T : IItem
        {
            string json = JsonUtility.ToJson(itemPrefab);
            T clone = (T)JsonUtility.FromJson(json, itemPrefab.GetType());
            clone.SetFieldValue("id", (id ?? ID.CreateRandom()).GetHashCode());
            clone.SetFieldValue("prefabId", itemPrefab.ID.GetHashCode());
            return clone;
        }
    }
}