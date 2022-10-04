#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace BaseGame
{
    public static class PlayerExtensions
    {
        public static bool TryGetFirstAbility<T>(this IPlayer? player, [NotNullWhen(true)] out T? ability)
        {
            if (player is not null && player.HasAbility<T>())
            {
                ability = player.GetFirstAbility<T>();
                return ability is not null;
            }

            ability = default;
            return false;
        }
    }
}