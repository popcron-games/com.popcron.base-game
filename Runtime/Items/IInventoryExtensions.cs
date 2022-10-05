#nullable enable

namespace BaseGame
{
    public static class IInventoryExtensions
    {
        /// <summary>
        /// Checks if the inventory contains any item of this type.
        /// </summary>
        public static bool Contains<T>(this IInventory inventory)
        {
            int count = inventory.Count;
            for (int i = 0; i < count; i++)
            {
                if (inventory[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the inventory contains this exact item.
        /// </summary>
        public static bool Contains(this IInventory inventory, IItem item)
        {
            int count = inventory.Count;
            for (int i = 0; i < count; i++)
            {
                if (inventory[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the first item of this type.
        /// </summary>
        /// <exception cref="Exception">Thrown if the item is not found.</exception>
        public static T GetFirst<T>(this IInventory inventory)
        {
            if (inventory.TryGetFirst(out T? item))
            {
                return item;
            }
            else
            {
                throw ExceptionBuilder.Format("No item of type {0} found in {1}", typeof(T), inventory);
            }
        }
    }
}