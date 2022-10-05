#nullable enable
using UnityEngine;

namespace BaseGame
{
    [AddComponentMenu("Cinemachine/My Player Cinemachine Target")]
    public class MyPlayerCinemachineTarget : MonoBehaviour
    {
        protected override void OnUpdate(float delta)
        {
            if (User.MyUser is User user)
            {
                if (user.Player is Player player)
                {
                    if (player.TryGetVisuals(out PlayerVisuals? visuals))
                    {
                        transform.position = visuals.Position;
                    }
                    else
                    {
                        Log.LogErrorFormat("PlayerVisuals not found for {0}", player);
                    }
                }
            }
        }
    }
}