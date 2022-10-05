#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace BaseGame
{
    public static class IPlayerExtensions
    {
        public static bool TryGetVisuals<T>(this IPlayer? player, [NotNullWhen(true)] out T? ability)
        {
            if (player is not null && player.Inventory.TryGetFirst(out ability))
            {
                return true;
            }
            else
            {
                ability = default;
                return false;
            }
        }
    }
}