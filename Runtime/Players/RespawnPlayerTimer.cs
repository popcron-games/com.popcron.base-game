#nullable enable
using UnityEngine;

namespace BaseGame
{
    [AddComponentMenu("Base Game/Respawn player")]
    public class RespawnPlayerTimer : MonoBehaviour, IValidate
    {
        [SerializeField]
        private float respawnDuration = 4f;

        private float respawnTimer;
        private User? user;

        bool IValidate.Validate()
        {
            bool changed = false;
            if (user is null)
            {
                user = GetComponent<User>();
                if (user is not null)
                {
                    changed = true;
                }
            }

            return changed;
        }

        protected override void OnUpdate(float delta)
        {
            if (user is not null && !user.IsAlive)
            {
                respawnTimer += delta;
                if (respawnTimer > respawnDuration)
                {
                    user.MakePlayerAlive();
                    respawnTimer = 0;
                }
            }
        }
    }
}