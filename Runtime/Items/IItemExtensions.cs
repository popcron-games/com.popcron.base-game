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
            int idHashCode = id?.GetHashCode() ?? ID.CreateRandom().GetHashCode();
            clone.SetFieldValue("id", idHashCode);
            clone.SetFieldValue("prefabId", itemPrefab.ID.GetHashCode());

            Debug.Log($"Cloned {itemPrefab} to create {clone}");
            return clone;
        }
    }
}